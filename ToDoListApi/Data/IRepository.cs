namespace ToDoListApi.Data;

public interface IRepository<T> : IDisposable
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<bool> InsertAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteByIdAsync(int id);
    Task SaveAsync();
}
