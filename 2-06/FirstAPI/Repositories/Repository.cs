using System;
using FirstAPI.Contexts;
using FirstAPI.Imterfaces;

namespace FirstAPI.Repositories;

public abstract class Repository<K, T> : IRepository<K, T> where T : class
{
    protected readonly ClinicContext _clinicContext;

    public Repository(ClinicContext clinicContext)
    {
        _clinicContext = clinicContext;
    }

    public async Task<T> Add(T item)
    {
        _clinicContext.Add(item);
        await _clinicContext.SaveChangesAsync();
        return item;
    }

    public async Task<T> Delete(K key)
    {
        var item = await Get(key);
        if (item == null)
        {
            throw new Exception("No items found");
        }
        _clinicContext.Remove(item);
        await _clinicContext.SaveChangesAsync();
        return item;
    }

    public abstract Task<T> Get(K key);

    public abstract Task<IEnumerable<T>> GetAll();

    public async Task<T> Update(K key, T item)
    {
        var myItem = await Get(key);
        if (myItem == null)
        {
            throw new Exception("No data found");
        }
        _clinicContext.Entry(myItem).CurrentValues.SetValues(item);
        await _clinicContext.SaveChangesAsync();
        return item;
    }
}
