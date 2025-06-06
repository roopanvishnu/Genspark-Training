using System;

namespace Notify.Interfaces;

public interface IRepo<K, T> where T : class
{
    Task<T> Get(K id);
    Task<ICollection<T>> GetAll();
    Task<T> Add(T item);
    Task<T> Update(K id, T item);
    Task<T> Delete(K id);
}
