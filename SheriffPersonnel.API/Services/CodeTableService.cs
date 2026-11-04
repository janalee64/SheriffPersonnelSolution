using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;
using ProgramModel = SheriffPersonnel.Shared.Models.Program;

namespace SheriffPersonnel.API.Services
{
    public class CodeTableService : ICodeTableService
    {
        private readonly ICodeTableRepository _repository;
        private readonly ILogger<CodeTableService> _logger;

        public CodeTableService(ICodeTableRepository repository, ILogger<CodeTableService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Statuses
        public async Task<IEnumerable<StatusesDto>> GetStatusesAsync()
        {
            var entities = await _repository.GetStatusesAsync();
            return entities.Select(s => new StatusesDto
            {
                StatusID = s.StatusID,
                StatusCode = s.StatusCode,
                StatusDescription = s.StatusDescription
            });
        }

        public async Task<StatusesDto?> GetStatusByIdAsync(int id)
        {
            var entity = await _repository.GetStatusByIdAsync(id);
            if (entity == null) return null;
            return new StatusesDto
            {
                StatusID = entity.StatusID,
                StatusCode = entity.StatusCode,
                StatusDescription = entity.StatusDescription
            };
        }

        public async Task<StatusesDto> CreateStatusAsync(StatusesDto dto)
        {
            var entity = new Status
            {
                StatusCode = dto.StatusCode,
                StatusDescription = dto.StatusDescription
            };
            var created = await _repository.AddStatusAsync(entity);
            dto.StatusID = created.StatusID;
            return dto;
        }

        public async Task<StatusesDto?> UpdateStatusAsync(int id, StatusesDto dto)
        {
            var existing = await _repository.GetStatusByIdAsync(id);
            if (existing == null) return null;

            existing.StatusCode = dto.StatusCode;
            existing.StatusDescription = dto.StatusDescription;

            await _repository.UpdateStatusAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteStatusAsync(int id)
        {
            return await _repository.DeleteStatusAsync(id);
        }

        // Programs
        public async Task<IEnumerable<ProgramsDto>> GetProgramsAsync()
        {
            var entities = await _repository.GetProgramsAsync();
            return entities.Select(p => new ProgramsDto
            {
                ProgramID = p.ProgramID,
                ProgramName = p.ProgramName,
                Description = p.Description,
                IsActive = p.IsActive ?? true
            });
        }

        public async Task<ProgramsDto?> GetProgramByIdAsync(int id)
        {
            var entity = await _repository.GetProgramByIdAsync(id);
            if (entity == null) return null;
            return new ProgramsDto
            {
                ProgramID = entity.ProgramID,
                ProgramName = entity.ProgramName,
                Description = entity.Description,
                IsActive = entity.IsActive ?? true
            };
        }

        public async Task<ProgramsDto> CreateProgramAsync(ProgramsDto dto)
        {
            var entity = new ProgramModel
            {
                ProgramName = dto.ProgramName,
                Description = dto.Description,
                IsActive = dto.IsActive
            };
            var created = await _repository.AddProgramAsync(entity);
            dto.ProgramID = created.ProgramID;
            return dto;
        }

        public async Task<ProgramsDto?> UpdateProgramAsync(int id, ProgramsDto dto)
        {
            var existing = await _repository.GetProgramByIdAsync(id);
            if (existing == null) return null;

            existing.ProgramName = dto.ProgramName;
            existing.Description = dto.Description;
            existing.IsActive = dto.IsActive;

            await _repository.UpdateProgramAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteProgramAsync(int id)
        {
            return await _repository.DeleteProgramAsync(id);
        }

        // Units
        public async Task<IEnumerable<UnitsDto>> GetUnitsAsync()
        {
            var entities = await _repository.GetUnitsAsync();
            return entities.Select(u => new UnitsDto
            {
                UnitID = u.UnitID,
                UnitName = u.UnitName,
                Notes = u.Notes,
                IsActive = u.IsActive ?? true
            });
        }

        public async Task<UnitsDto?> GetUnitByIdAsync(int id)
        {
            var entity = await _repository.GetUnitByIdAsync(id);
            if (entity == null) return null;
            return new UnitsDto
            {
                UnitID = entity.UnitID,
                UnitName = entity.UnitName,
                Notes = entity.Notes,
                IsActive = entity.IsActive ?? true
            };
        }

        public async Task<UnitsDto> CreateUnitAsync(UnitsDto dto)
        {
            var entity = new Unit
            {
                UnitName = dto.UnitName,
                Notes = dto.Notes,
                IsActive = dto.IsActive
            };
            var created = await _repository.AddUnitAsync(entity);
            dto.UnitID = created.UnitID;
            return dto;
        }

        public async Task<UnitsDto?> UpdateUnitAsync(int id, UnitsDto dto)
        {
            var existing = await _repository.GetUnitByIdAsync(id);
            if (existing == null) return null;

            existing.UnitName = dto.UnitName;
            existing.Notes = dto.Notes;
            existing.IsActive = dto.IsActive;

            await _repository.UpdateUnitAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteUnitAsync(int id)
        {
            return await _repository.DeleteUnitAsync(id);
        }

        // Ranks
        public async Task<IEnumerable<RanksDto>> GetRanksAsync()
        {
            var entities = await _repository.GetRanksAsync();
            return entities.Select(r => new RanksDto
            {
                RankID = r.RankID,
                RankName = r.RankName,
                IsActive = r.IsActive ?? true
            });
        }

        public async Task<RanksDto?> GetRankByIdAsync(int id)
        {
            var entity = await _repository.GetRankByIdAsync(id);
            if (entity == null) return null;
            return new RanksDto
            {
                RankID = entity.RankID,
                RankName = entity.RankName,
                IsActive = entity.IsActive ?? true
            };
        }

        public async Task<RanksDto> CreateRankAsync(RanksDto dto)
        {
            var entity = new Rank
            {
                RankName = dto.RankName,
                IsActive = dto.IsActive
            };
            var created = await _repository.AddRankAsync(entity);
            dto.RankID = created.RankID;
            return dto;
        }

        public async Task<RanksDto?> UpdateRankAsync(int id, RanksDto dto)
        {
            var existing = await _repository.GetRankByIdAsync(id);
            if (existing == null) return null;

            existing.RankName = dto.RankName;
            existing.IsActive = dto.IsActive;

            await _repository.UpdateRankAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteRankAsync(int id)
        {
            return await _repository.DeleteRankAsync(id);
        }

        // Makes
        public async Task<IEnumerable<MakesDto>> GetMakesAsync()
        {
            var entities = await _repository.GetMakesAsync();
            return entities.Select(m => new MakesDto
            {
                MakeID = m.MakeID,
                MakeName = m.MakeName
            });
        }

        public async Task<MakesDto?> GetMakeByIdAsync(int id)
        {
            var entity = await _repository.GetMakeByIdAsync(id);
            if (entity == null) return null;
            return new MakesDto
            {
                MakeID = entity.MakeID,
                MakeName = entity.MakeName
            };
        }

        public async Task<MakesDto> CreateMakeAsync(MakesDto dto)
        {
            var entity = new Make
            {
                MakeName = dto.MakeName
            };
            var created = await _repository.AddMakeAsync(entity);
            dto.MakeID = created.MakeID;
            return dto;
        }

        public async Task<MakesDto?> UpdateMakeAsync(int id, MakesDto dto)
        {
            var existing = await _repository.GetMakeByIdAsync(id);
            if (existing == null) return null;

            existing.MakeName = dto.MakeName;

            await _repository.UpdateMakeAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteMakeAsync(int id)
        {
            return await _repository.DeleteMakeAsync(id);
        }

        // Range Courses
        public async Task<IEnumerable<RangeCourseDTO>> GetRangeCoursesAsync()
        {
            var entities = await _repository.GetRangeCoursesAsync();
            return entities.Select(rc => new RangeCourseDTO
            {
                CourseID = rc.CourseID,
                CourseCode = rc.CourseCode,
                CourseName = rc.CourseName,
                Notes = rc.Notes,
                IsActive = rc.IsActive ?? true
            });
        }

        public async Task<RangeCourseDTO?> GetRangeCourseByIdAsync(int id)
        {
            var entity = await _repository.GetRangeCourseByIdAsync(id);
            if (entity == null) return null;
            return new RangeCourseDTO
            {
                CourseID = entity.CourseID,
                CourseCode = entity.CourseCode,
                CourseName = entity.CourseName,
                Notes = entity.Notes,
                IsActive = entity.IsActive ?? true
            };
        }

        public async Task<RangeCourseDTO> CreateRangeCourseAsync(RangeCourseDTO dto)
        {
            var entity = new RangeCourse
            {
                CourseCode = dto.CourseCode,
                CourseName = dto.CourseName,
                Notes = dto.Notes,
                IsActive = dto.IsActive
            };
            var created = await _repository.AddRangeCourseAsync(entity);
            dto.CourseID = created.CourseID;
            return dto;
        }

        public async Task<RangeCourseDTO?> UpdateRangeCourseAsync(int id, RangeCourseDTO dto)
        {
            var existing = await _repository.GetRangeCourseByIdAsync(id);
            if (existing == null) return null;

            existing.CourseCode = dto.CourseCode;
            existing.CourseName = dto.CourseName;
            existing.Notes = dto.Notes;
            existing.IsActive = dto.IsActive;

            await _repository.UpdateRangeCourseAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteRangeCourseAsync(int id)
        {
            return await _repository.DeleteRangeCourseAsync(id);
        }

        // Range Officers
        public async Task<IEnumerable<RangeOfficerDTO>> GetRangeOfficersAsync()
        {
            var entities = await _repository.GetRangeOfficersAsync();
            return entities.Select(ro => new RangeOfficerDTO
            {
                RangeOfficerID = ro.RangeOfficerID,
                OfficerName = ro.OfficerName,
                IsActive = ro.IsActive ?? true
            });
        }

        public async Task<RangeOfficerDTO?> GetRangeOfficerByIdAsync(int id)
        {
            var entity = await _repository.GetRangeOfficerByIdAsync(id);
            if (entity == null) return null;
            return new RangeOfficerDTO
            {
                RangeOfficerID = entity.RangeOfficerID,
                OfficerName = entity.OfficerName,
                IsActive = entity.IsActive ?? true
            };
        }

        public async Task<RangeOfficerDTO> CreateRangeOfficerAsync(RangeOfficerDTO dto)
        {
            var entity = new RangeOfficer
            {
                OfficerName = dto.OfficerName,
                IsActive = dto.IsActive
            };
            var created = await _repository.AddRangeOfficerAsync(entity);
            dto.RangeOfficerID = created.RangeOfficerID;
            return dto;
        }

        public async Task<RangeOfficerDTO?> UpdateRangeOfficerAsync(int id, RangeOfficerDTO dto)
        {
            var existing = await _repository.GetRangeOfficerByIdAsync(id);
            if (existing == null) return null;

            existing.OfficerName = dto.OfficerName;
            existing.IsActive = dto.IsActive;

            await _repository.UpdateRangeOfficerAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteRangeOfficerAsync(int id)
        {
            return await _repository.DeleteRangeOfficerAsync(id);
        }

        // Range Skills
        public async Task<IEnumerable<RangeSkillDTO>> GetRangeSkillsAsync()
        {
            var entities = await _repository.GetRangeSkillsAsync();
            return entities.Select(rs => new RangeSkillDTO
            {
                RangeSkillID = rs.RangeSkillID,
                SkillName = rs.SkillName
            });
        }

        public async Task<RangeSkillDTO?> GetRangeSkillByIdAsync(int id)
        {
            var entity = await _repository.GetRangeSkillByIdAsync(id);
            if (entity == null) return null;
            return new RangeSkillDTO
            {
                RangeSkillID = entity.RangeSkillID,
                SkillName = entity.SkillName
            };
        }

        public async Task<RangeSkillDTO> CreateRangeSkillAsync(RangeSkillDTO dto)
        {
            var entity = new RangeSkill
            {
                SkillName = dto.SkillName
            };
            var created = await _repository.AddRangeSkillAsync(entity);
            dto.RangeSkillID = created.RangeSkillID;
            return dto;
        }

        public async Task<RangeSkillDTO?> UpdateRangeSkillAsync(int id, RangeSkillDTO dto)
        {
            var existing = await _repository.GetRangeSkillByIdAsync(id);
            if (existing == null) return null;

            existing.SkillName = dto.SkillName;

            await _repository.UpdateRangeSkillAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteRangeSkillAsync(int id)
        {
            return await _repository.DeleteRangeSkillAsync(id);
        }

        // Categories
        public async Task<IEnumerable<CategoriesDto>> GetCategoriesAsync()
        {
            var entities = await _repository.GetCategoriesAsync();
            return entities.Select(c => new CategoriesDto
            {
                CategoryID = c.CategoryID,
                CategoryName = c.CategoryName,
                IsActive = c.IsActive ?? true
            });
        }

        public async Task<CategoriesDto?> GetCategoryByIdAsync(int id)
        {
            var entity = await _repository.GetCategoryByIdAsync(id);
            if (entity == null) return null;
            return new CategoriesDto
            {
                CategoryID = entity.CategoryID,
                CategoryName = entity.CategoryName,
                IsActive = entity.IsActive ?? true
            };
        }

        public async Task<CategoriesDto> CreateCategoryAsync(CategoriesDto dto)
        {
            var entity = new Category
            {
                CategoryName = dto.CategoryName,
                IsActive = dto.IsActive
            };
            var created = await _repository.AddCategoryAsync(entity);
            dto.CategoryID = created.CategoryID;
            return dto;
        }

        public async Task<CategoriesDto?> UpdateCategoryAsync(int id, CategoriesDto dto)
        {
            var existing = await _repository.GetCategoryByIdAsync(id);
            if (existing == null) return null;

            existing.CategoryName = dto.CategoryName;
            existing.IsActive = dto.IsActive;

            await _repository.UpdateCategoryAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _repository.DeleteCategoryAsync(id);
        }

        // Locations
        public async Task<IEnumerable<LocationsDto>> GetLocationsAsync()
        {
            var entities = await _repository.GetLocationsAsync();
            return entities.Select(l => new LocationsDto
            {
                LocationID = l.LocationID,
                LocationName = l.LocationName,
                Notes = l.Notes
            });
        }

        public async Task<LocationsDto?> GetLocationByIdAsync(int id)
        {
            var entity = await _repository.GetLocationByIdAsync(id);
            if (entity == null) return null;
            return new LocationsDto
            {
                LocationID = entity.LocationID,
                LocationName = entity.LocationName,
                Notes = entity.Notes
            };
        }

        public async Task<LocationsDto> CreateLocationAsync(LocationsDto dto)
        {
            var entity = new Location
            {
                LocationName = dto.LocationName,
                Notes = dto.Notes
            };
            var created = await _repository.AddLocationAsync(entity);
            dto.LocationID = created.LocationID;
            return dto;
        }

        public async Task<LocationsDto?> UpdateLocationAsync(int id, LocationsDto dto)
        {
            var existing = await _repository.GetLocationByIdAsync(id);
            if (existing == null) return null;

            existing.LocationName = dto.LocationName;
            existing.Notes = dto.Notes;

            await _repository.UpdateLocationAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteLocationAsync(int id)
        {
            return await _repository.DeleteLocationAsync(id);
        }

        // Suppliers
        public async Task<IEnumerable<SuppliersDto>> GetSuppliersAsync()
        {
            var entities = await _repository.GetSuppliersAsync();
            return entities.Select(s => new SuppliersDto
            {
                SupplierID = s.SupplierID,
                SupplierName = s.SupplierName,
                Notes = s.Notes
            });
        }

        public async Task<SuppliersDto?> GetSupplierByIdAsync(int id)
        {
            var entity = await _repository.GetSupplierByIdAsync(id);
            if (entity == null) return null;
            return new SuppliersDto
            {
                SupplierID = entity.SupplierID,
                SupplierName = entity.SupplierName,
                Notes = entity.Notes
            };
        }

        public async Task<SuppliersDto> CreateSupplierAsync(SuppliersDto dto)
        {
            var entity = new Supplier
            {
                SupplierName = dto.SupplierName,
                Notes = dto.Notes
            };
            var created = await _repository.AddSupplierAsync(entity);
            dto.SupplierID = created.SupplierID;
            return dto;
        }

        public async Task<SuppliersDto?> UpdateSupplierAsync(int id, SuppliersDto dto)
        {
            var existing = await _repository.GetSupplierByIdAsync(id);
            if (existing == null) return null;

            existing.SupplierName = dto.SupplierName;
            existing.Notes = dto.Notes;

            await _repository.UpdateSupplierAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            return await _repository.DeleteSupplierAsync(id);
        }

        // Training Schools
        public async Task<IEnumerable<TrainingSchoolsDto>> GetTrainingSchoolsAsync()
        {
            var entities = await _repository.GetTrainingSchoolsAsync();
            return entities.Select(ts => new TrainingSchoolsDto
            {
                TrainingSchoolID = ts.TrainingSchoolID,
                SchoolName = ts.SchoolName,
                Address = ts.Address,
                IsActive = ts.IsActive ?? true
            });
        }

        public async Task<TrainingSchoolsDto?> GetTrainingSchoolByIdAsync(int id)
        {
            var entity = await _repository.GetTrainingSchoolByIdAsync(id);
            if (entity == null) return null;
            return new TrainingSchoolsDto
            {
                TrainingSchoolID = entity.TrainingSchoolID,
                SchoolName = entity.SchoolName,
                Address = entity.Address,
                IsActive = entity.IsActive ?? true
            };
        }

        public async Task<TrainingSchoolsDto> CreateTrainingSchoolAsync(TrainingSchoolsDto dto)
        {
            var entity = new TrainingSchool
            {
                SchoolName = dto.SchoolName,
                Address = dto.Address,
                IsActive = dto.IsActive
            };
            var created = await _repository.AddTrainingSchoolAsync(entity);
            dto.TrainingSchoolID = created.TrainingSchoolID;
            return dto;
        }

        public async Task<TrainingSchoolsDto?> UpdateTrainingSchoolAsync(int id, TrainingSchoolsDto dto)
        {
            var existing = await _repository.GetTrainingSchoolByIdAsync(id);
            if (existing == null) return null;

            existing.SchoolName = dto.SchoolName;
            existing.Address = dto.Address;
            existing.IsActive = dto.IsActive;

            await _repository.UpdateTrainingSchoolAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteTrainingSchoolAsync(int id)
        {
            return await _repository.DeleteTrainingSchoolAsync(id);
        }

        // Document Types
        public async Task<IEnumerable<DocumentTypeDto>> GetDocumentTypesAsync()
        {
            var entities = await _repository.GetDocumentTypesAsync();
            return entities.Select(dt => new DocumentTypeDto
            {
                DocumentTypeID = dt.DocumentTypeID,
                DocumentType = dt.DocumentTypeName
            });
        }

        public async Task<DocumentTypeDto?> GetDocumentTypeByIdAsync(int id)
        {
            var entity = await _repository.GetDocumentTypeByIdAsync(id);
            if (entity == null) return null;
            return new DocumentTypeDto
            {
                DocumentTypeID = entity.DocumentTypeID,
                DocumentType = entity.DocumentTypeName
            };
        }

        public async Task<DocumentTypeDto> CreateDocumentTypeAsync(DocumentTypeDto dto)
        {
            var entity = new DocumentType
            {
                DocumentTypeName = dto.DocumentType
            };
            var created = await _repository.AddDocumentTypeAsync(entity);
            dto.DocumentTypeID = created.DocumentTypeID;
            return dto;
        }

        public async Task<DocumentTypeDto?> UpdateDocumentTypeAsync(int id, DocumentTypeDto dto)
        {
            var existing = await _repository.GetDocumentTypeByIdAsync(id);
            if (existing == null) return null;

            existing.DocumentTypeName = dto.DocumentType;
            await _repository.UpdateDocumentTypeAsync(existing);
            return dto;
        }

        public async Task<bool> DeleteDocumentTypeAsync(int id)
        {
            return await _repository.DeleteDocumentTypeAsync(id);
        }
    }
}

