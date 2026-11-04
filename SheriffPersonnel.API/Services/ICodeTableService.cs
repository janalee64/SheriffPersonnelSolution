using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;

namespace SheriffPersonnel.API.Services
{
    public interface ICodeTableService
    {
        // Statuses
        Task<IEnumerable<StatusesDto>> GetStatusesAsync();
        Task<StatusesDto?> GetStatusByIdAsync(int id);
        Task<StatusesDto> CreateStatusAsync(StatusesDto dto);
        Task<StatusesDto?> UpdateStatusAsync(int id, StatusesDto dto);
        Task<bool> DeleteStatusAsync(int id);

        // Programs
        Task<IEnumerable<ProgramsDto>> GetProgramsAsync();
        Task<ProgramsDto?> GetProgramByIdAsync(int id);
        Task<ProgramsDto> CreateProgramAsync(ProgramsDto dto);
        Task<ProgramsDto?> UpdateProgramAsync(int id, ProgramsDto dto);
        Task<bool> DeleteProgramAsync(int id);

        // Units
        Task<IEnumerable<UnitsDto>> GetUnitsAsync();
        Task<UnitsDto?> GetUnitByIdAsync(int id);
        Task<UnitsDto> CreateUnitAsync(UnitsDto dto);
        Task<UnitsDto?> UpdateUnitAsync(int id, UnitsDto dto);
        Task<bool> DeleteUnitAsync(int id);

        // Ranks
        Task<IEnumerable<RanksDto>> GetRanksAsync();
        Task<RanksDto?> GetRankByIdAsync(int id);
        Task<RanksDto> CreateRankAsync(RanksDto dto);
        Task<RanksDto?> UpdateRankAsync(int id, RanksDto dto);
        Task<bool> DeleteRankAsync(int id);

        // Makes
        Task<IEnumerable<MakesDto>> GetMakesAsync();
        Task<MakesDto?> GetMakeByIdAsync(int id);
        Task<MakesDto> CreateMakeAsync(MakesDto dto);
        Task<MakesDto?> UpdateMakeAsync(int id, MakesDto dto);
        Task<bool> DeleteMakeAsync(int id);

        // Range Courses
        Task<IEnumerable<RangeCourseDTO>> GetRangeCoursesAsync();
        Task<RangeCourseDTO?> GetRangeCourseByIdAsync(int id);
        Task<RangeCourseDTO> CreateRangeCourseAsync(RangeCourseDTO dto);
        Task<RangeCourseDTO?> UpdateRangeCourseAsync(int id, RangeCourseDTO dto);
        Task<bool> DeleteRangeCourseAsync(int id);

        // Range Officers
        Task<IEnumerable<RangeOfficerDTO>> GetRangeOfficersAsync();
        Task<RangeOfficerDTO?> GetRangeOfficerByIdAsync(int id);
        Task<RangeOfficerDTO> CreateRangeOfficerAsync(RangeOfficerDTO dto);
        Task<RangeOfficerDTO?> UpdateRangeOfficerAsync(int id, RangeOfficerDTO dto);
        Task<bool> DeleteRangeOfficerAsync(int id);

        // Range Skills
        Task<IEnumerable<RangeSkillDTO>> GetRangeSkillsAsync();
        Task<RangeSkillDTO?> GetRangeSkillByIdAsync(int id);
        Task<RangeSkillDTO> CreateRangeSkillAsync(RangeSkillDTO dto);
        Task<RangeSkillDTO?> UpdateRangeSkillAsync(int id, RangeSkillDTO dto);
        Task<bool> DeleteRangeSkillAsync(int id);

        // Categories
        Task<IEnumerable<CategoriesDto>> GetCategoriesAsync();
        Task<CategoriesDto?> GetCategoryByIdAsync(int id);
        Task<CategoriesDto> CreateCategoryAsync(CategoriesDto dto);
        Task<CategoriesDto?> UpdateCategoryAsync(int id, CategoriesDto dto);
        Task<bool> DeleteCategoryAsync(int id);

        // Locations
        Task<IEnumerable<LocationsDto>> GetLocationsAsync();
        Task<LocationsDto?> GetLocationByIdAsync(int id);
        Task<LocationsDto> CreateLocationAsync(LocationsDto dto);
        Task<LocationsDto?> UpdateLocationAsync(int id, LocationsDto dto);
        Task<bool> DeleteLocationAsync(int id);

        // Suppliers
        Task<IEnumerable<SuppliersDto>> GetSuppliersAsync();
        Task<SuppliersDto?> GetSupplierByIdAsync(int id);
        Task<SuppliersDto> CreateSupplierAsync(SuppliersDto dto);
        Task<SuppliersDto?> UpdateSupplierAsync(int id, SuppliersDto dto);
        Task<bool> DeleteSupplierAsync(int id);

        // Training Schools
        Task<IEnumerable<TrainingSchoolsDto>> GetTrainingSchoolsAsync();
        Task<TrainingSchoolsDto?> GetTrainingSchoolByIdAsync(int id);
        Task<TrainingSchoolsDto> CreateTrainingSchoolAsync(TrainingSchoolsDto dto);
        Task<TrainingSchoolsDto?> UpdateTrainingSchoolAsync(int id, TrainingSchoolsDto dto);
        Task<bool> DeleteTrainingSchoolAsync(int id);

        // Document Types
        Task<IEnumerable<DocumentTypeDto>> GetDocumentTypesAsync();
        Task<DocumentTypeDto?> GetDocumentTypeByIdAsync(int id);
        Task<DocumentTypeDto> CreateDocumentTypeAsync(DocumentTypeDto dto);
        Task<DocumentTypeDto?> UpdateDocumentTypeAsync(int id, DocumentTypeDto dto);
        Task<bool> DeleteDocumentTypeAsync(int id);
    }
}

