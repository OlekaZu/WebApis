namespace ToDoListApi.Data.TaskItems;

public class TaskItemRepository : IRepository<TaskItem>
{
    private readonly TasksDb _context;

    public TaskItemRepository(TasksDb context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAllAsync()
        => await _context.TaskItems
        .Include(i => i.Doer)
        .Include(i => i.Group)
        .ToListAsync();

    public async Task<List<TaskItem>> GetByIdAsync(int userId)
        => await _context.TaskItems
        .Where(i => i.Doer != null && i.Doer.Id == userId)
        .Include(i => i.Doer)
        .Include(i => i.Group)
        .ToListAsync();

    public async Task<TaskItem?> GetBySpecifiedIdAsync(int userId, int specifiedId)
    {
        var list = await GetByIdAsync(userId);
        if (specifiedId > list.Count)
            return null;
        else
            return list[specifiedId - 1];
    }

    public async Task<bool> InsertAsync(TaskItem entity)
    {
        var check = _context.TaskItems.Count() == 0 ? true
            : await _context.TaskItems.AllAsync(u => u.Id != entity.Id);
        if (check)
            await _context.TaskItems.AddAsync(entity);
        return check;
    }

    public async Task<bool> UpdateAsync(TaskItem entity)
    {
        var taskItemFromDb = await _context.TaskItems.FindAsync(new object[] { entity.Id });
        if (taskItemFromDb == null)
            return false;
        taskItemFromDb.Name = entity.Name;
        taskItemFromDb.Description = entity.Description;
        taskItemFromDb.DoerId = entity.DoerId;
        taskItemFromDb.TaskGroupId = entity.TaskGroupId;
        taskItemFromDb.Begin = entity.Begin;
        taskItemFromDb.End = entity.End;
        taskItemFromDb.IsCompleted = entity.IsCompleted;
        taskItemFromDb.Priority = entity.Priority;
        return true;
    }

    public async Task<bool> DeleteByIdAsync(int userId)
    {
        var taskItemsByUserId = await GetByIdAsync(userId);
        if (taskItemsByUserId.Count == 0)
            return false;
        _context.TaskItems.RemoveRange(taskItemsByUserId);
        return true;
    }

    public async Task<bool> DeleteBySpecifiedIdAsync(int userId, int specifiedId)
    {
        var taskItemsByUserId = await GetByIdAsync(userId);
        if (specifiedId > taskItemsByUserId.Count)
            return false;
        _context.TaskItems.Remove(taskItemsByUserId[specifiedId - 1]);
        return true;
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    private bool _disposed = false;

    // disposing == false (runtime calls the method and only unmanaged resources are disposed)
    // disposing == true (user's code calls the method and both managed and unmanaged resources are disposed)
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
