using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;  
 
namespace LeaveManagementSystem.Repositories
{
    public abstract class Repository<K, T> : IRepository<K, T> where T:class
    {
        protected readonly ApplicationDbContext _applicationDbContext;

        public Repository(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        
        public async virtual Task<T> Add(T item)
        {
            _applicationDbContext.Add(item);
            await _applicationDbContext.SaveChangesAsync();
            return item;
        }

        public async Task<T> Delete(K key)
        {
            var item = await Get(key);
            if (item == null)
                throw new Exception("No such item found for deleting");

            var type = typeof(T);
            var isDeletedProp = type.GetProperty("IsDeleted");
            var deletedAtProp = type.GetProperty("DeletedAt");

            if (isDeletedProp != null && isDeletedProp.PropertyType == typeof(bool))
            {
                var isDeleted = (bool)(isDeletedProp.GetValue(item) ?? false);
                if (isDeleted)
                throw new Exception("Item is already deleted.");

                isDeletedProp.SetValue(item, true);

                if (deletedAtProp != null && deletedAtProp.PropertyType == typeof(DateTime?))
                    deletedAtProp.SetValue(item, DateTime.UtcNow);

                _applicationDbContext.Update(item);
            }
            else
            {
                _applicationDbContext.Remove(item);
            }

            await _applicationDbContext.SaveChangesAsync();
            return item;
        }


        public abstract Task<T> Get(K key);


        public abstract Task<IEnumerable<T>> GetAll();


        public async Task<T> Update(K key, T item)
        {
            var myItem = await Get(key);
            if (myItem != null)
            {
                _applicationDbContext.Entry(myItem).CurrentValues.SetValues(item);
                await _applicationDbContext.SaveChangesAsync();
                return item;
            }
            throw new Exception("No such item found for updation");
        }
    }
}

