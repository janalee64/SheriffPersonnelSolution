using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SheriffPersonnel.API.Repositories;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _repository;
        private readonly IUserRepository _userRepository;

        public DocumentService(
            IDocumentRepository repository,
            IUserRepository userRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        private async Task<int> ResolveValidUserIdAsync(int userId)
        {
            if (userId > 0)
            {
                var existingUser = await _userRepository.GetUserByIdAsync(userId);
                if (existingUser != null)
                {
                    return userId;
                }
            }

            var users = await _userRepository.GetAllUsersAsync();
            var firstUser = users.FirstOrDefault();
            if (firstUser != null)
            {
                return firstUser.UserID;
            }

            return 1;
        }

        public async Task<IEnumerable<EmployeeDocumentDto>> GetDocumentsByEmployeeIdAsync(int employeeId)
        {
            var entities = await _repository.GetDocumentsByEmployeeIdAsync(employeeId);
            return entities.Select(MapToDto);
        }

        public async Task<EmployeeDocumentDto?> GetDocumentByIdAsync(int id)
        {
            var entity = await _repository.GetDocumentByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<Document?> GetDocumentEntityByIdAsync(int id)
        {
            return await _repository.GetDocumentByIdAsync(id);
        }

        public async Task<EmployeeDocumentDto> CreateDocumentAsync(EmployeeDocumentDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            int validUserId = await ResolveValidUserIdAsync(dto.UploadedBy);

            var entity = new Document
            {
                EmployeeID = dto.EmployeeID,
                Title = dto.Title,
                DocumentTypeID = dto.DocumentTypeID,
                FileName = dto.FileName,
                FileSizeBytes = dto.FileSizeBytes,
                MimeType = dto.MimeType,
                FileData = dto.FileData ?? Array.Empty<byte>(),
                UploadedBy = validUserId,
                UploadDate = dto.UploadDate ?? DateTime.Now
            };

            var created = await _repository.AddDocumentAsync(entity);
            var reloaded = await _repository.GetDocumentByIdAsync(created.DocumentID);
            return MapToDto(reloaded ?? created);
        }

        public async Task<EmployeeDocumentDto?> UpdateDocumentAsync(int id, EmployeeDocumentDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existing = await _repository.GetDocumentByIdAsync(id);
            if (existing == null) return null;

            existing.Title = dto.Title;
            existing.DocumentTypeID = dto.DocumentTypeID;
            if (!string.IsNullOrEmpty(dto.FileName)) existing.FileName = dto.FileName;
            if (dto.FileSizeBytes > 0) existing.FileSizeBytes = dto.FileSizeBytes;
            if (!string.IsNullOrEmpty(dto.MimeType)) existing.MimeType = dto.MimeType;
            if (dto.FileData != null && dto.FileData.Length > 0) existing.FileData = dto.FileData;

            var updated = await _repository.UpdateDocumentAsync(existing);
            var reloaded = await _repository.GetDocumentByIdAsync(updated.DocumentID);
            return MapToDto(reloaded ?? updated);
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            return await _repository.DeleteDocumentAsync(id);
        }

        public async Task<IEnumerable<DocumentTypeDto>> GetDocumentTypesAsync()
        {
            var types = await _repository.GetDocumentTypesAsync();
            return types.Select(dt => new DocumentTypeDto
            {
                DocumentTypeID = dt.DocumentTypeID,
                DocumentType = dt.DocumentTypeName
            });
        }

        public async Task<DocumentTypeDto?> GetDocumentTypeByIdAsync(int id)
        {
            var entity = await _repository.GetDocumentTypeByIdAsync(id);
            return entity == null ? null : new DocumentTypeDto
            {
                DocumentTypeID = entity.DocumentTypeID,
                DocumentType = entity.DocumentTypeName
            };
        }

        public async Task<DocumentTypeDto> CreateDocumentTypeAsync(DocumentTypeDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var entity = new DocumentType
            {
                DocumentTypeName = dto.DocumentType
            };

            var created = await _repository.AddDocumentTypeAsync(entity);
            return new DocumentTypeDto
            {
                DocumentTypeID = created.DocumentTypeID,
                DocumentType = created.DocumentTypeName
            };
        }

        public async Task<DocumentTypeDto?> UpdateDocumentTypeAsync(int id, DocumentTypeDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var existing = await _repository.GetDocumentTypeByIdAsync(id);
            if (existing == null) return null;

            existing.DocumentTypeName = dto.DocumentType;
            var updated = await _repository.UpdateDocumentTypeAsync(existing);
            return new DocumentTypeDto
            {
                DocumentTypeID = updated.DocumentTypeID,
                DocumentType = updated.DocumentTypeName
            };
        }

        public async Task<bool> DeleteDocumentTypeAsync(int id)
        {
            return await _repository.DeleteDocumentTypeAsync(id);
        }

        private static EmployeeDocumentDto MapToDto(Document entity)
        {
            return new EmployeeDocumentDto
            {
                DocumentID = entity.DocumentID,
                EmployeeID = entity.EmployeeID,
                Title = entity.Title,
                DocumentTypeID = entity.DocumentTypeID,
                DocumentTypeName = entity.DocumentType?.DocumentTypeName,
                FileName = entity.FileName,
                FileSizeBytes = entity.FileSizeBytes,
                MimeType = entity.MimeType,
                UploadedBy = entity.UploadedBy,
                UploadedByName = entity.UploadedByUser?.DisplayName ?? entity.UploadedByUser?.UserPrincipalName,
                UploadDate = entity.UploadDate,
                FileData = entity.FileData
            };
        }
    }
}

