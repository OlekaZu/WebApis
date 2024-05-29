namespace ToDoListApi.Data;

public interface IRepository<T> : IDisposable
{
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetByIdAsync(int id);
    Task<T?> GetBySpecifiedIdAsync(int id, int specifiedId);
    Task<bool> InsertAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteByIdAsync(int id);
    Task<bool> DeleteBySpecifiedIdAsync(int id, int specifiedId);
    Task SaveAsync();
}
