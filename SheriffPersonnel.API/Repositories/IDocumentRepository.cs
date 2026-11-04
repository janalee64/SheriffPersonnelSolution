using System.Collections.Generic;
using System.Threading.Tasks;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<Document>> GetDocumentsByEmployeeIdAsync(int employeeId);
        Task<Document?> GetDocumentByIdAsync(int id);
        Task<Document> AddDocumentAsync(Document document);
        Task<Document> UpdateDocumentAsync(Document document);
        Task<bool> DeleteDocumentAsync(int id);
        Task<IEnumerable<DocumentType>> GetDocumentTypesAsync();
        Task<DocumentType?> GetDocumentTypeByIdAsync(int id);
        Task<DocumentType> AddDocumentTypeAsync(DocumentType documentType);
        Task<DocumentType> UpdateDocumentTypeAsync(DocumentType documentType);
        Task<bool> DeleteDocumentTypeAsync(int id);
    }
}

