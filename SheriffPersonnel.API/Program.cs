using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using System.Security.Claims;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.API.Services;
using SheriffPersonnel.API.Middleware;
using Serilog;
using System.Diagnostics;

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
    Log.Information("Starting SheriffPersonnel.API application...");

    var builder = WebApplication.CreateBuilder(args);

    // Configure Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Add Global Exception Handler & ProblemDetails
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Add DbContext
    builder.Services.AddDbContext<DBContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Validate Entra Token & Inject Database Roles dynamically
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(options =>
        {
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var identity = context.Principal?.Identity as ClaimsIdentity;
                    if (identity == null) return;

                    var oid = context.Principal?.Claims.FirstOrDefault(c =>
                        c.Type == "oid" ||
                        c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier" ||
                        c.Type.EndsWith("/objectidentifier"))?.Value;

                    var email = context.Principal?.Claims.FirstOrDefault(c =>
                        c.Type == "preferred_username" ||
                        c.Type == ClaimTypes.Upn ||
                        c.Type == ClaimTypes.Email ||
                        c.Type.EndsWith("/upn") ||
                        c.Type.EndsWith("/emailaddress"))?.Value;

                    if (!string.IsNullOrEmpty(oid) || !string.IsNullOrEmpty(email))
                    {
                        var db = context.HttpContext.RequestServices.GetRequiredService<DBContext>();

                        var query = db.Users.AsNoTracking();
                        if (!string.IsNullOrEmpty(oid) && !string.IsNullOrEmpty(email))
                        {
                            query = query.Where(u => u.EntraObjectID == oid || u.UserPrincipalName == email);
                        }
                        else if (!string.IsNullOrEmpty(oid))
                        {
                            query = query.Where(u => u.EntraObjectID == oid);
                        }
                        else
                        {
                            query = query.Where(u => u.UserPrincipalName == email);
                        }

                        var user = await query
                            .Select(u => new
                            {
                                u.UserID,
                                u.DisplayName,
                                u.UserPrincipalName,
                                Roles = u.UserRoles.Where(ur => ur.Role != null).Select(ur => ur.Role!.RoleName).ToList()
                            })
                            .FirstOrDefaultAsync();

                        if (user != null)
                        {
                            // Inject custom database roles and UserID directly into the security identity context
                            var appIdentity = new ClaimsIdentity();
                            appIdentity.AddClaim(new Claim("UserID", user.UserID.ToString()));
                            appIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()));

                            foreach (var role in user.Roles)
                            {
                                if (!string.IsNullOrEmpty(role))
                                {
                                    appIdentity.AddClaim(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));
                                }
                            }
                            context.Principal?.AddIdentity(appIdentity);
                        }

                        if (context.Principal != null)
                        {
                            foreach (var role in context.Principal.Claims.Where(c => c.Type == ClaimTypes.Role))
                            {
                                Debug.WriteLine($"Role: {role.Value}");
                            }
                        }
                    }
                }
            };
        }, options => { builder.Configuration.Bind("AzureAd", options); });

    // Register Repository and Service layer dependencies
    builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
    builder.Services.AddScoped<IEmployeeSearchService, EmployeeSearchService>();
    builder.Services.AddScoped<IEmployeeChangeRepository, EmployeeChangeRepository>();
    builder.Services.AddScoped<IEmployeeChangeService, EmployeeChangeService>();
    builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
    builder.Services.AddScoped<IInventoryService, InventoryService>();
    builder.Services.AddScoped<IFirearmRepository, FirearmRepository>();
    builder.Services.AddScoped<IFirearmService, FirearmService>();
    builder.Services.AddScoped<IRangeStatisticRepository, RangeStatisticRepository>();
    builder.Services.AddScoped<IRangeStatisticService, RangeStatisticService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ITrainingRepository, TrainingRepository>();
    builder.Services.AddScoped<ITrainingService, TrainingService>();
    builder.Services.AddScoped<ICodeTableRepository, CodeTableRepository>();
    builder.Services.AddScoped<ICodeTableService, CodeTableService>();
    builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
    builder.Services.AddScoped<IDocumentService, DocumentService>();


    var app = builder.Build();

    // Global Exception Handling & Serilog Request Logging middleware
    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SheriffPersonnel.API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
