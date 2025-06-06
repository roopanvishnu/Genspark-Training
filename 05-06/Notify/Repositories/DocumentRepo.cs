using System;
using Microsoft.EntityFrameworkCore;
using Notify.Contexts;
using Notify.Models;

namespace Notify.Repositories;

public class DocumentRepo : Repo<string, Document>
{
    public DocumentRepo(NotifyContext notifyContext) : base(notifyContext)
    {
    }


    public override async Task<Document> Get(string id)
    {
        Document? document = await _context.documents.FindAsync(id);
        if (document == null) throw new Exception("Document not found");
        return document;
    }

    public override async Task<ICollection<Document>> GetAll()
    {
        var documents = _context.documents;
        if (documents == null) throw new Exception("No documents found");
        return await documents.ToListAsync();
    }
}
