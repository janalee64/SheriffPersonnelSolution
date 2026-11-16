using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.Web.Services
{
    public class CodeTableService
    {
        private readonly IDownstreamApi _downstreamApi;
        private readonly MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        private readonly ILogger<CodeTableService> _logger;
        private readonly IMemoryCache? _cache;

        public CodeTableService(
            IDownstreamApi downstreamApi,
            MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler,
            ILogger<CodeTableService> logger,
            IMemoryCache? cache = null)
        {
            _downstreamApi = downstreamApi ?? throw new ArgumentNullException(nameof(downstreamApi));
            _consentHandler = consentHandler ?? throw new ArgumentNullException(nameof(consentHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache;
        }

        private void InvalidateCache(string? cacheKey)
        {
            if (!string.IsNullOrEmpty(cacheKey))
            {
                _cache?.Remove(cacheKey);
            }
        }

        private async Task<IEnumerable<T>> GetListAsync<T>(string relativePath)
        {
            try
            {
                var response = await _downstreamApi.CallApiForUserAsync<IEnumerable<T>>(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Get.Method;
                        options.RelativePath = relativePath;
                    });
                return response ?? Array.Empty<T>();
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting list from {RelativePath}", relativePath);
                throw;
            }
        }

        private async Task<TOutput?> PostAsync<TInput, TOutput>(string relativePath, TInput dto) where TOutput : class
        {
            try
            {
                return await _downstreamApi.CallApiForUserAsync<TInput, TOutput>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Post.Method;
                        options.RelativePath = relativePath;
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting to {RelativePath}", relativePath);
                throw;
            }
        }

        private async Task<TOutput?> PutAsync<TInput, TOutput>(string relativePath, TInput dto) where TOutput : class
        {
            try
            {
                return await _downstreamApi.CallApiForUserAsync<TInput, TOutput>(
                    "MyCustomApi",
                    dto,
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Put.Method;
                        options.RelativePath = relativePath;
                    });
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error putting to {RelativePath}", relativePath);
                throw;
            }
        }

        private async Task<bool> DeleteAsync(string relativePath)
        {
            try
            {
                var response = await _downstreamApi.CallApiForUserAsync(
                    "MyCustomApi",
                    options =>
                    {
                        options.HttpMethod = HttpMethod.Delete.Method;
                        options.RelativePath = relativePath;
                    });
                return response?.IsSuccessStatusCode ?? false;
            }
            catch (MicrosoftIdentityWebChallengeUserException ex)
            {
                _consentHandler.HandleException(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting from {RelativePath}", relativePath);
                throw;
            }
        }

        // Statuses
        public Task<IEnumerable<StatusesDto>> GetStatusesAsync() => GetListAsync<StatusesDto>("api/statuses");
        public async Task<StatusesDto?> CreateStatusAsync(StatusesDto dto) { var res = await PostAsync<StatusesDto, StatusesDto>("api/statuses", dto); InvalidateCache("SearchService_Statuses"); return res; }
        public async Task<StatusesDto?> UpdateStatusAsync(int id, StatusesDto dto) { var res = await PutAsync<StatusesDto, StatusesDto>($"api/statuses/{id}", dto); InvalidateCache("SearchService_Statuses"); return res; }
        public async Task<bool> DeleteStatusAsync(int id) { var res = await DeleteAsync($"api/statuses/{id}"); InvalidateCache("SearchService_Statuses"); return res; }

        // Programs
        public Task<IEnumerable<ProgramsDto>> GetProgramsAsync() => GetListAsync<ProgramsDto>("api/programs");
        public async Task<ProgramsDto?> CreateProgramAsync(ProgramsDto dto) { var res = await PostAsync<ProgramsDto, ProgramsDto>("api/programs", dto); InvalidateCache("SearchService_Programs"); return res; }
        public async Task<ProgramsDto?> UpdateProgramAsync(int id, ProgramsDto dto) { var res = await PutAsync<ProgramsDto, ProgramsDto>($"api/programs/{id}", dto); InvalidateCache("SearchService_Programs"); return res; }
        public async Task<bool> DeleteProgramAsync(int id) { var res = await DeleteAsync($"api/programs/{id}"); InvalidateCache("SearchService_Programs"); return res; }

        // Units
        public Task<IEnumerable<UnitsDto>> GetUnitsAsync() => GetListAsync<UnitsDto>("api/units");
        public async Task<UnitsDto?> CreateUnitAsync(UnitsDto dto) { var res = await PostAsync<UnitsDto, UnitsDto>("api/units", dto); InvalidateCache("SearchService_Units"); return res; }
        public async Task<UnitsDto?> UpdateUnitAsync(int id, UnitsDto dto) { var res = await PutAsync<UnitsDto, UnitsDto>($"api/units/{id}", dto); InvalidateCache("SearchService_Units"); return res; }
        public async Task<bool> DeleteUnitAsync(int id) { var res = await DeleteAsync($"api/units/{id}"); InvalidateCache("SearchService_Units"); return res; }

        // Ranks
        public Task<IEnumerable<RanksDto>> GetRanksAsync() => GetListAsync<RanksDto>("api/ranks");
        public async Task<RanksDto?> CreateRankAsync(RanksDto dto) { var res = await PostAsync<RanksDto, RanksDto>("api/ranks", dto); InvalidateCache("SearchService_Ranks"); return res; }
        public async Task<RanksDto?> UpdateRankAsync(int id, RanksDto dto) { var res = await PutAsync<RanksDto, RanksDto>($"api/ranks/{id}", dto); InvalidateCache("SearchService_Ranks"); return res; }
        public async Task<bool> DeleteRankAsync(int id) { var res = await DeleteAsync($"api/ranks/{id}"); InvalidateCache("SearchService_Ranks"); return res; }

        // Makes
        public Task<IEnumerable<MakesDto>> GetMakesAsync() => GetListAsync<MakesDto>("api/makes");
        public async Task<MakesDto?> CreateMakeAsync(MakesDto dto) { var res = await PostAsync<MakesDto, MakesDto>("api/makes", dto); InvalidateCache("SearchService_Makes"); return res; }
        public async Task<MakesDto?> UpdateMakeAsync(int id, MakesDto dto) { var res = await PutAsync<MakesDto, MakesDto>($"api/makes/{id}", dto); InvalidateCache("SearchService_Makes"); return res; }
        public async Task<bool> DeleteMakeAsync(int id) { var res = await DeleteAsync($"api/makes/{id}"); InvalidateCache("SearchService_Makes"); return res; }

        // Range Courses
        public Task<IEnumerable<RangeCourseDTO>> GetRangeCoursesAsync() => GetListAsync<RangeCourseDTO>("api/rangecourses");
        public async Task<RangeCourseDTO?> CreateRangeCourseAsync(RangeCourseDTO dto) { var res = await PostAsync<RangeCourseDTO, RangeCourseDTO>("api/rangecourses", dto); InvalidateCache("SearchService_RangeCourses"); return res; }
        public async Task<RangeCourseDTO?> UpdateRangeCourseAsync(int id, RangeCourseDTO dto) { var res = await PutAsync<RangeCourseDTO, RangeCourseDTO>($"api/rangecourses/{id}", dto); InvalidateCache("SearchService_RangeCourses"); return res; }
        public async Task<bool> DeleteRangeCourseAsync(int id) { var res = await DeleteAsync($"api/rangecourses/{id}"); InvalidateCache("SearchService_RangeCourses"); return res; }

        // Range Officers
        public Task<IEnumerable<RangeOfficerDTO>> GetRangeOfficersAsync() => GetListAsync<RangeOfficerDTO>("api/rangeofficers");
        public Task<RangeOfficerDTO?> CreateRangeOfficerAsync(RangeOfficerDTO dto) => PostAsync<RangeOfficerDTO, RangeOfficerDTO>("api/rangeofficers", dto);
        public Task<RangeOfficerDTO?> UpdateRangeOfficerAsync(int id, RangeOfficerDTO dto) => PutAsync<RangeOfficerDTO, RangeOfficerDTO>($"api/rangeofficers/{id}", dto);
        public Task<bool> DeleteRangeOfficerAsync(int id) => DeleteAsync($"api/rangeofficers/{id}");

        // Range Skills
        public Task<IEnumerable<RangeSkillDTO>> GetRangeSkillsAsync() => GetListAsync<RangeSkillDTO>("api/rangeskills");
        public Task<RangeSkillDTO?> CreateRangeSkillAsync(RangeSkillDTO dto) => PostAsync<RangeSkillDTO, RangeSkillDTO>("api/rangeskills", dto);
        public Task<RangeSkillDTO?> UpdateRangeSkillAsync(int id, RangeSkillDTO dto) => PutAsync<RangeSkillDTO, RangeSkillDTO>($"api/rangeskills/{id}", dto);
        public Task<bool> DeleteRangeSkillAsync(int id) => DeleteAsync($"api/rangeskills/{id}");

        // Categories
        public Task<IEnumerable<CategoriesDto>> GetCategoriesAsync() => GetListAsync<CategoriesDto>("api/categories");
        public Task<CategoriesDto?> CreateCategoryAsync(CategoriesDto dto) => PostAsync<CategoriesDto, CategoriesDto>("api/categories", dto);
        public Task<CategoriesDto?> UpdateCategoryAsync(int id, CategoriesDto dto) => PutAsync<CategoriesDto, CategoriesDto>($"api/categories/{id}", dto);
        public Task<bool> DeleteCategoryAsync(int id) => DeleteAsync($"api/categories/{id}");

        // Locations
        public Task<IEnumerable<LocationsDto>> GetLocationsAsync() => GetListAsync<LocationsDto>("api/locations");
        public Task<LocationsDto?> CreateLocationAsync(LocationsDto dto) => PostAsync<LocationsDto, LocationsDto>("api/locations", dto);
        public Task<LocationsDto?> UpdateLocationAsync(int id, LocationsDto dto) => PutAsync<LocationsDto, LocationsDto>($"api/locations/{id}", dto);
        public Task<bool> DeleteLocationAsync(int id) => DeleteAsync($"api/locations/{id}");

        // Suppliers
        public Task<IEnumerable<SuppliersDto>> GetSuppliersAsync() => GetListAsync<SuppliersDto>("api/suppliers");
        public Task<SuppliersDto?> CreateSupplierAsync(SuppliersDto dto) => PostAsync<SuppliersDto, SuppliersDto>("api/suppliers", dto);
        public Task<SuppliersDto?> UpdateSupplierAsync(int id, SuppliersDto dto) => PutAsync<SuppliersDto, SuppliersDto>($"api/suppliers/{id}", dto);
        public Task<bool> DeleteSupplierAsync(int id) => DeleteAsync($"api/suppliers/{id}");

        // Training Schools
        public Task<IEnumerable<TrainingSchoolsDto>> GetTrainingSchoolsAsync() => GetListAsync<TrainingSchoolsDto>("api/trainingschools");
        public Task<TrainingSchoolsDto?> CreateTrainingSchoolAsync(TrainingSchoolsDto dto) => PostAsync<TrainingSchoolsDto, TrainingSchoolsDto>("api/trainingschools", dto);
        public Task<TrainingSchoolsDto?> UpdateTrainingSchoolAsync(int id, TrainingSchoolsDto dto) => PutAsync<TrainingSchoolsDto, TrainingSchoolsDto>($"api/trainingschools/{id}", dto);
        public Task<bool> DeleteTrainingSchoolAsync(int id) => DeleteAsync($"api/trainingschools/{id}");

        // Document Types
        public Task<IEnumerable<DocumentTypeDto>> GetDocumentTypesAsync() => GetListAsync<DocumentTypeDto>("api/documents/types");
        public async Task<DocumentTypeDto?> CreateDocumentTypeAsync(DocumentTypeDto dto) { var res = await PostAsync<DocumentTypeDto, DocumentTypeDto>("api/documents/types", dto); InvalidateCache("SearchService_DocumentTypes"); return res; }
        public async Task<DocumentTypeDto?> UpdateDocumentTypeAsync(int id, DocumentTypeDto dto) { var res = await PutAsync<DocumentTypeDto, DocumentTypeDto>($"api/documents/types/{id}", dto); InvalidateCache("SearchService_DocumentTypes"); return res; }
        public async Task<bool> DeleteDocumentTypeAsync(int id) { var res = await DeleteAsync($"api/documents/types/{id}"); InvalidateCache("SearchService_DocumentTypes"); return res; }
    }
}
