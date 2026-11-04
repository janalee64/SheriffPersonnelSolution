using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.DTOs;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<EmployeeDocumentDto>> GetDocumentsByEmployeeIdAsync(int employeeId);
        Task<EmployeeDocumentDto?> GetDocumentByIdAsync(int id);
        Task<Document?> GetDocumentEntityByIdAsync(int id);
        Task<EmployeeDocumentDto> CreateDocumentAsync(EmployeeDocumentDto dto);
        Task<EmployeeDocumentDto?> UpdateDocumentAsync(int id, EmployeeDocumentDto dto);
        Task<bool> DeleteDocumentAsync(int id);
        Task<IEnumerable<DocumentTypeDto>> GetDocumentTypesAsync();
        Task<DocumentTypeDto?> GetDocumentTypeByIdAsync(int id);
        Task<DocumentTypeDto> CreateDocumentTypeAsync(DocumentTypeDto dto);
        Task<DocumentTypeDto?> UpdateDocumentTypeAsync(int id, DocumentTypeDto dto);
        Task<bool> DeleteDocumentTypeAsync(int id);
    }
}

