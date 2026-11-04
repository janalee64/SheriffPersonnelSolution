using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.Models;
using ProgramModel = SheriffPersonnel.Shared.Models.Program;

namespace SheriffPersonnel.API.Repositories
{
    public class CodeTableRepository : ICodeTableRepository
    {
        private readonly DBContext _dbContext;

        public CodeTableRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // Statuses
        public async Task<IEnumerable<Status>> GetStatusesAsync()
        {
            return await _dbContext.Statuses.OrderBy(s => s.StatusCode).ToListAsync();
        }

        public async Task<Status?> GetStatusByIdAsync(int id)
        {
            return await _dbContext.Statuses.FindAsync(id);
        }

        public async Task<Status> AddStatusAsync(Status status)
        {
            if (status == null) throw new ArgumentNullException(nameof(status));
            await _dbContext.Statuses.AddAsync(status);
            await _dbContext.SaveChangesAsync();
            return status;
        }

        public async Task<Status> UpdateStatusAsync(Status status)
        {
            if (status == null) throw new ArgumentNullException(nameof(status));
            var entry = _dbContext.Entry(status);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Statuses.Update(status);
            }
            await _dbContext.SaveChangesAsync();
            return status;
        }

        public async Task<bool> DeleteStatusAsync(int id)
        {
            var entity = await _dbContext.Statuses.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Statuses.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Programs
        public async Task<IEnumerable<ProgramModel>> GetProgramsAsync()
        {
            return await _dbContext.Programs.OrderBy(p => p.ProgramName).ToListAsync();
        }

        public async Task<ProgramModel?> GetProgramByIdAsync(int id)
        {
            return await _dbContext.Programs.FindAsync(id);
        }

        public async Task<ProgramModel> AddProgramAsync(ProgramModel program)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            await _dbContext.Programs.AddAsync(program);
            await _dbContext.SaveChangesAsync();
            return program;
        }

        public async Task<ProgramModel> UpdateProgramAsync(ProgramModel program)
        {
            if (program == null) throw new ArgumentNullException(nameof(program));
            var entry = _dbContext.Entry(program);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Programs.Update(program);
            }
            await _dbContext.SaveChangesAsync();
            return program;
        }

        public async Task<bool> DeleteProgramAsync(int id)
        {
            var entity = await _dbContext.Programs.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Programs.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Units
        public async Task<IEnumerable<Unit>> GetUnitsAsync()
        {
            return await _dbContext.Units.OrderBy(u => u.UnitName).ToListAsync();
        }

        public async Task<Unit?> GetUnitByIdAsync(int id)
        {
            return await _dbContext.Units.FindAsync(id);
        }

        public async Task<Unit> AddUnitAsync(Unit unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));
            await _dbContext.Units.AddAsync(unit);
            await _dbContext.SaveChangesAsync();
            return unit;
        }

        public async Task<Unit> UpdateUnitAsync(Unit unit)
        {
            if (unit == null) throw new ArgumentNullException(nameof(unit));
            var entry = _dbContext.Entry(unit);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Units.Update(unit);
            }
            await _dbContext.SaveChangesAsync();
            return unit;
        }

        public async Task<bool> DeleteUnitAsync(int id)
        {
            var entity = await _dbContext.Units.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Units.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Ranks
        public async Task<IEnumerable<Rank>> GetRanksAsync()
        {
            return await _dbContext.Ranks.OrderBy(r => r.RankName).ToListAsync();
        }

        public async Task<Rank?> GetRankByIdAsync(int id)
        {
            return await _dbContext.Ranks.FindAsync(id);
        }

        public async Task<Rank> AddRankAsync(Rank rank)
        {
            if (rank == null) throw new ArgumentNullException(nameof(rank));
            await _dbContext.Ranks.AddAsync(rank);
            await _dbContext.SaveChangesAsync();
            return rank;
        }

        public async Task<Rank> UpdateRankAsync(Rank rank)
        {
            if (rank == null) throw new ArgumentNullException(nameof(rank));
            var entry = _dbContext.Entry(rank);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Ranks.Update(rank);
            }
            await _dbContext.SaveChangesAsync();
            return rank;
        }

        public async Task<bool> DeleteRankAsync(int id)
        {
            var entity = await _dbContext.Ranks.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Ranks.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Makes
        public async Task<IEnumerable<Make>> GetMakesAsync()
        {
            return await _dbContext.Makes.OrderBy(m => m.MakeName).ToListAsync();
        }

        public async Task<Make?> GetMakeByIdAsync(int id)
        {
            return await _dbContext.Makes.FindAsync(id);
        }

        public async Task<Make> AddMakeAsync(Make make)
        {
            if (make == null) throw new ArgumentNullException(nameof(make));
            await _dbContext.Makes.AddAsync(make);
            await _dbContext.SaveChangesAsync();
            return make;
        }

        public async Task<Make> UpdateMakeAsync(Make make)
        {
            if (make == null) throw new ArgumentNullException(nameof(make));
            var entry = _dbContext.Entry(make);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Makes.Update(make);
            }
            await _dbContext.SaveChangesAsync();
            return make;
        }

        public async Task<bool> DeleteMakeAsync(int id)
        {
            var entity = await _dbContext.Makes.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Makes.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Range Courses
        public async Task<IEnumerable<RangeCourse>> GetRangeCoursesAsync()
        {
            return await _dbContext.RangeCourses.OrderBy(rc => rc.CourseName).ToListAsync();
        }

        public async Task<RangeCourse?> GetRangeCourseByIdAsync(int id)
        {
            return await _dbContext.RangeCourses.FindAsync(id);
        }

        public async Task<RangeCourse> AddRangeCourseAsync(RangeCourse course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            await _dbContext.RangeCourses.AddAsync(course);
            await _dbContext.SaveChangesAsync();
            return course;
        }

        public async Task<RangeCourse> UpdateRangeCourseAsync(RangeCourse course)
        {
            if (course == null) throw new ArgumentNullException(nameof(course));
            var entry = _dbContext.Entry(course);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.RangeCourses.Update(course);
            }
            await _dbContext.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteRangeCourseAsync(int id)
        {
            var entity = await _dbContext.RangeCourses.FindAsync(id);
            if (entity == null) return false;
            _dbContext.RangeCourses.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Range Officers
        public async Task<IEnumerable<RangeOfficer>> GetRangeOfficersAsync()
        {
            return await _dbContext.RangeOfficers.OrderBy(ro => ro.OfficerName).ToListAsync();
        }

        public async Task<RangeOfficer?> GetRangeOfficerByIdAsync(int id)
        {
            return await _dbContext.RangeOfficers.FindAsync(id);
        }

        public async Task<RangeOfficer> AddRangeOfficerAsync(RangeOfficer officer)
        {
            if (officer == null) throw new ArgumentNullException(nameof(officer));
            await _dbContext.RangeOfficers.AddAsync(officer);
            await _dbContext.SaveChangesAsync();
            return officer;
        }

        public async Task<RangeOfficer> UpdateRangeOfficerAsync(RangeOfficer officer)
        {
            if (officer == null) throw new ArgumentNullException(nameof(officer));
            var entry = _dbContext.Entry(officer);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.RangeOfficers.Update(officer);
            }
            await _dbContext.SaveChangesAsync();
            return officer;
        }

        public async Task<bool> DeleteRangeOfficerAsync(int id)
        {
            var entity = await _dbContext.RangeOfficers.FindAsync(id);
            if (entity == null) return false;
            _dbContext.RangeOfficers.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Range Skills
        public async Task<IEnumerable<RangeSkill>> GetRangeSkillsAsync()
        {
            return await _dbContext.RangeSkills.OrderBy(rs => rs.SkillName).ToListAsync();
        }

        public async Task<RangeSkill?> GetRangeSkillByIdAsync(int id)
        {
            return await _dbContext.RangeSkills.FindAsync(id);
        }

        public async Task<RangeSkill> AddRangeSkillAsync(RangeSkill skill)
        {
            if (skill == null) throw new ArgumentNullException(nameof(skill));
            await _dbContext.RangeSkills.AddAsync(skill);
            await _dbContext.SaveChangesAsync();
            return skill;
        }

        public async Task<RangeSkill> UpdateRangeSkillAsync(RangeSkill skill)
        {
            if (skill == null) throw new ArgumentNullException(nameof(skill));
            var entry = _dbContext.Entry(skill);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.RangeSkills.Update(skill);
            }
            await _dbContext.SaveChangesAsync();
            return skill;
        }

        public async Task<bool> DeleteRangeSkillAsync(int id)
        {
            var entity = await _dbContext.RangeSkills.FindAsync(id);
            if (entity == null) return false;
            _dbContext.RangeSkills.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Categories
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _dbContext.Categories.OrderBy(c => c.CategoryName).ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _dbContext.Categories.FindAsync(id);
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));
            var entry = _dbContext.Entry(category);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Categories.Update(category);
            }
            await _dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var entity = await _dbContext.Categories.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Categories.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Locations
        public async Task<IEnumerable<Location>> GetLocationsAsync()
        {
            return await _dbContext.Locations.OrderBy(l => l.LocationName).ToListAsync();
        }

        public async Task<Location?> GetLocationByIdAsync(int id)
        {
            return await _dbContext.Locations.FindAsync(id);
        }

        public async Task<Location> AddLocationAsync(Location location)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));
            await _dbContext.Locations.AddAsync(location);
            await _dbContext.SaveChangesAsync();
            return location;
        }

        public async Task<Location> UpdateLocationAsync(Location location)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));
            var entry = _dbContext.Entry(location);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Locations.Update(location);
            }
            await _dbContext.SaveChangesAsync();
            return location;
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            var entity = await _dbContext.Locations.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Locations.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Suppliers
        public async Task<IEnumerable<Supplier>> GetSuppliersAsync()
        {
            return await _dbContext.Suppliers.OrderBy(s => s.SupplierName).ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _dbContext.Suppliers.FindAsync(id);
        }

        public async Task<Supplier> AddSupplierAsync(Supplier supplier)
        {
            if (supplier == null) throw new ArgumentNullException(nameof(supplier));
            await _dbContext.Suppliers.AddAsync(supplier);
            await _dbContext.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            if (supplier == null) throw new ArgumentNullException(nameof(supplier));
            var entry = _dbContext.Entry(supplier);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Suppliers.Update(supplier);
            }
            await _dbContext.SaveChangesAsync();
            return supplier;
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var entity = await _dbContext.Suppliers.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Suppliers.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Training Schools
        public async Task<IEnumerable<TrainingSchool>> GetTrainingSchoolsAsync()
        {
            return await _dbContext.TrainingSchools.OrderBy(ts => ts.SchoolName).ToListAsync();
        }

        public async Task<TrainingSchool?> GetTrainingSchoolByIdAsync(int id)
        {
            return await _dbContext.TrainingSchools.FindAsync(id);
        }

        public async Task<TrainingSchool> AddTrainingSchoolAsync(TrainingSchool school)
        {
            if (school == null) throw new ArgumentNullException(nameof(school));
            await _dbContext.TrainingSchools.AddAsync(school);
            await _dbContext.SaveChangesAsync();
            return school;
        }

        public async Task<TrainingSchool> UpdateTrainingSchoolAsync(TrainingSchool school)
        {
            if (school == null) throw new ArgumentNullException(nameof(school));
            var entry = _dbContext.Entry(school);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.TrainingSchools.Update(school);
            }
            await _dbContext.SaveChangesAsync();
            return school;
        }

        public async Task<bool> DeleteTrainingSchoolAsync(int id)
        {
            var entity = await _dbContext.TrainingSchools.FindAsync(id);
            if (entity == null) return false;
            _dbContext.TrainingSchools.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Document Types
        public async Task<IEnumerable<DocumentType>> GetDocumentTypesAsync()
        {
            return await _dbContext.DocumentTypes.OrderBy(dt => dt.DocumentTypeName).ToListAsync();
        }

        public async Task<DocumentType?> GetDocumentTypeByIdAsync(int id)
        {
            return await _dbContext.DocumentTypes.FindAsync(id);
        }

        public async Task<DocumentType> AddDocumentTypeAsync(DocumentType documentType)
        {
            if (documentType == null) throw new ArgumentNullException(nameof(documentType));
            await _dbContext.DocumentTypes.AddAsync(documentType);
            await _dbContext.SaveChangesAsync();
            return documentType;
        }

        public async Task<DocumentType> UpdateDocumentTypeAsync(DocumentType documentType)
        {
            if (documentType == null) throw new ArgumentNullException(nameof(documentType));
            var entry = _dbContext.Entry(documentType);
            if (entry.State == EntityState.Detached)
            {
                _dbContext.DocumentTypes.Update(documentType);
            }
            await _dbContext.SaveChangesAsync();
            return documentType;
        }

        public async Task<bool> DeleteDocumentTypeAsync(int id)
        {
            var entity = await _dbContext.DocumentTypes.FindAsync(id);
            if (entity == null) return false;
            _dbContext.DocumentTypes.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

