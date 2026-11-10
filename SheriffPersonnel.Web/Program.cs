using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using MudBlazor.Services;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Web.Components;
using SheriffPersonnel.Web.Middleware;
using SheriffPersonnel.Web.Services;
using Serilog;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build())
    .Enrich.FromLogContext()
    .CreateLogger();

try
{
    Log.Information("Starting SheriffPersonnel.Web application...");

    var builder = WebApplication.CreateBuilder(args);

    // Add this configuration:
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        // Only loopback proxies are trusted by default. Clear the list to trust IIS on local network
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();
    });

    // Configure Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Register Global Exception Handler
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    // Add MudBlazor services
    builder.Services.AddMudServices();

    // Add DbContext
    builder.Services.AddDbContext<DBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Add Microsoft Identity Web authentication
    builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
        .EnableTokenAcquisitionToCallDownstreamApi()
        .AddDownstreamApi("MyCustomApi", builder.Configuration.GetSection("MyCustomApi"))
        .AddInMemoryTokenCaches();

    // Add the diagnostic logger to intercept and print OIDC failures
    builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.Events.OnRemoteFailure = context =>
        {
            context.Response.ContentType = "text/plain";
            
            string errorMessage = $"Authentication Failure: {context.Failure?.Message}\n\n" +
                                $"Details:\n{context.Failure?.ToString()}";
                                
            context.Response.WriteAsync(errorMessage);
            context.HandleResponse(); 
            
            return Task.CompletedTask;
        };
    });

    builder.Services.AddControllersWithViews()
        .AddMicrosoftIdentityUI();

    builder.Services.AddMicrosoftIdentityConsentHandler();
    builder.Services.AddCascadingAuthenticationState();
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("CanEditPersonnel", policy =>
            policy.RequireAssertion(context =>
            {
                var user = context.User;
                if (user == null) return false;
                bool hasView = user.IsInRole("Personnel View") || user.IsInRole("Employee View");
                bool hasEdit = user.IsInRole("Personnel Edit") || user.IsInRole("Employee Edit");
                return hasView && hasEdit;
            }));
    });

    // Register HttpClient for HTTP communication with API endpoints
    builder.Services.AddHttpClient();

    // Add services to the container.
    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddScoped<IClaimsTransformation, UserClaimsTransformer>();
    builder.Services.AddScoped<SearchService>();
    builder.Services.AddScoped<SearchStateContainer>();
    builder.Services.AddScoped<UserAdminService>();
    builder.Services.AddScoped<CodeTableService>();
    builder.Services.AddScoped<ExcelExportService>();

    var app = builder.Build();

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

    app.UseForwardedHeaders();
    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();

    app.MapStaticAssets();
    app.MapControllers();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SheriffPersonnel.Web terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
