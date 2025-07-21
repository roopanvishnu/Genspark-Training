using System;
using System.Threading.Tasks;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.DTOs;

namespace Notify.Services;

public class DocumentService
{
    private readonly IRepo<string, Document> _docRepo;
    public DocumentService(IRepo<string, Document> docRepo)
    {
        _docRepo = docRepo;
    }
    public async Task<Document> AddDocument(Document dto)
    {
        dto = await _docRepo.Add(dto);
        return dto;
    }
    public async Task<ICollection<Document>> GetAll()
    {
        var docs = await _docRepo.GetAll();
        if (docs == null) throw new Exception("No document found");
        return docs;
    }
    public async Task<Document> GetDocument(string id)
    {
        var doc = await _docRepo.Get(id);
        if (doc == null) throw new Exception("No document found");
        return doc;
    }
}
