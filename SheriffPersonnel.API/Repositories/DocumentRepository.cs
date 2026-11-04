using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SheriffPersonnel.Shared.Data;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.API.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DBContext _dbContext;

        public DocumentRepository(DBContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<Document>> GetDocumentsByEmployeeIdAsync(int employeeId)
        {
            return await _dbContext.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.UploadedByUser)
                .Where(d => d.EmployeeID == employeeId)
                .OrderByDescending(d => d.UploadDate)
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(int id)
        {
            return await _dbContext.Documents
                .Include(d => d.DocumentType)
                .Include(d => d.UploadedByUser)
                .FirstOrDefaultAsync(d => d.DocumentID == id);
        }

        public async Task<Document> AddDocumentAsync(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            await _dbContext.Documents.AddAsync(document);
            await _dbContext.SaveChangesAsync();
            return document;
        }

        public async Task<Document> UpdateDocumentAsync(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            _dbContext.Documents.Update(document);
            await _dbContext.SaveChangesAsync();
            return document;
        }

        public async Task<bool> DeleteDocumentAsync(int id)
        {
            var entity = await _dbContext.Documents.FindAsync(id);
            if (entity == null) return false;
            _dbContext.Documents.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DocumentType>> GetDocumentTypesAsync()
        {
            return await _dbContext.DocumentTypes
                .OrderBy(dt => dt.DocumentTypeName)
                .ToListAsync();
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
            _dbContext.DocumentTypes.Update(documentType);
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

