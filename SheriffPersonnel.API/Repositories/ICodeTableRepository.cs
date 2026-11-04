using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.Models;
using ProgramModel = SheriffPersonnel.Shared.Models.Program;

namespace SheriffPersonnel.API.Repositories
{
    public interface ICodeTableRepository
    {
        // Statuses
        Task<IEnumerable<Status>> GetStatusesAsync();
        Task<Status?> GetStatusByIdAsync(int id);
        Task<Status> AddStatusAsync(Status status);
        Task<Status> UpdateStatusAsync(Status status);
        Task<bool> DeleteStatusAsync(int id);

        // Programs
        Task<IEnumerable<ProgramModel>> GetProgramsAsync();
        Task<ProgramModel?> GetProgramByIdAsync(int id);
        Task<ProgramModel> AddProgramAsync(ProgramModel program);
        Task<ProgramModel> UpdateProgramAsync(ProgramModel program);
        Task<bool> DeleteProgramAsync(int id);

        // Units
        Task<IEnumerable<Unit>> GetUnitsAsync();
        Task<Unit?> GetUnitByIdAsync(int id);
        Task<Unit> AddUnitAsync(Unit unit);
        Task<Unit> UpdateUnitAsync(Unit unit);
        Task<bool> DeleteUnitAsync(int id);

        // Ranks
        Task<IEnumerable<Rank>> GetRanksAsync();
        Task<Rank?> GetRankByIdAsync(int id);
        Task<Rank> AddRankAsync(Rank rank);
        Task<Rank> UpdateRankAsync(Rank rank);
        Task<bool> DeleteRankAsync(int id);

        // Makes
        Task<IEnumerable<Make>> GetMakesAsync();
        Task<Make?> GetMakeByIdAsync(int id);
        Task<Make> AddMakeAsync(Make make);
        Task<Make> UpdateMakeAsync(Make make);
        Task<bool> DeleteMakeAsync(int id);

        // Range Courses
        Task<IEnumerable<RangeCourse>> GetRangeCoursesAsync();
        Task<RangeCourse?> GetRangeCourseByIdAsync(int id);
        Task<RangeCourse> AddRangeCourseAsync(RangeCourse course);
        Task<RangeCourse> UpdateRangeCourseAsync(RangeCourse course);
        Task<bool> DeleteRangeCourseAsync(int id);

        // Range Officers
        Task<IEnumerable<RangeOfficer>> GetRangeOfficersAsync();
        Task<RangeOfficer?> GetRangeOfficerByIdAsync(int id);
        Task<RangeOfficer> AddRangeOfficerAsync(RangeOfficer officer);
        Task<RangeOfficer> UpdateRangeOfficerAsync(RangeOfficer officer);
        Task<bool> DeleteRangeOfficerAsync(int id);

        // Range Skills
        Task<IEnumerable<RangeSkill>> GetRangeSkillsAsync();
        Task<RangeSkill?> GetRangeSkillByIdAsync(int id);
        Task<RangeSkill> AddRangeSkillAsync(RangeSkill skill);
        Task<RangeSkill> UpdateRangeSkillAsync(RangeSkill skill);
        Task<bool> DeleteRangeSkillAsync(int id);

        // Categories
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> AddCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);

        // Locations
        Task<IEnumerable<Location>> GetLocationsAsync();
        Task<Location?> GetLocationByIdAsync(int id);
        Task<Location> AddLocationAsync(Location location);
        Task<Location> UpdateLocationAsync(Location location);
        Task<bool> DeleteLocationAsync(int id);

        // Suppliers
        Task<IEnumerable<Supplier>> GetSuppliersAsync();
        Task<Supplier?> GetSupplierByIdAsync(int id);
        Task<Supplier> AddSupplierAsync(Supplier supplier);
        Task<Supplier> UpdateSupplierAsync(Supplier supplier);
        Task<bool> DeleteSupplierAsync(int id);

        // Training Schools
        Task<IEnumerable<TrainingSchool>> GetTrainingSchoolsAsync();
        Task<TrainingSchool?> GetTrainingSchoolByIdAsync(int id);
        Task<TrainingSchool> AddTrainingSchoolAsync(TrainingSchool school);
        Task<TrainingSchool> UpdateTrainingSchoolAsync(TrainingSchool school);
        Task<bool> DeleteTrainingSchoolAsync(int id);

        // Document Types
        Task<IEnumerable<DocumentType>> GetDocumentTypesAsync();
        Task<DocumentType?> GetDocumentTypeByIdAsync(int id);
        Task<DocumentType> AddDocumentTypeAsync(DocumentType documentType);
        Task<DocumentType> UpdateDocumentTypeAsync(DocumentType documentType);
        Task<bool> DeleteDocumentTypeAsync(int id);
    }
}

