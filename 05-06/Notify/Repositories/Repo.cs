using System;
using System.Threading.Tasks;
using Notify.Contexts;
using Notify.Interfaces;

namespace Notify.Repositories;

public abstract class Repo<K,T> : IRepo<K,T> where T : class
{
    protected readonly NotifyContext _context;
    public Repo(NotifyContext notifyContext)
    {
        _context = notifyContext;
    }

    public abstract Task<T> Get(K id);
    public abstract Task<ICollection<T>> GetAll();

    public async Task<T> Add(T item)
    {
        await _context.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<T> Update(K id, T item)
    {
        T? listitem = await Get(id);
        if (listitem == null) throw new Exception("No user found");
        typeof(T).GetProperty("Id")?.SetValue(item, id);
        _context.Entry(listitem).CurrentValues.SetValues(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<T> Delete(K id)
    {
        T? listitem = await Get(id);
        if (listitem == null) throw new Exception("No user found");
        _context.Remove(listitem);
        await _context.SaveChangesAsync();
        return listitem;
    }

}
