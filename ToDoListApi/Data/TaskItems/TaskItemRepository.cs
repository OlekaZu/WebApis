namespace ToDoListApi.Data.TaskItems;

public class TaskItemRepository : IRepository<TaskItem>, IUserTaskItemRepository
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

    public async Task<TaskItem?> GetByIdAsync(int id)
        => await _context.TaskItems
        .Include(i => i.Doer)
        .Include(i => i.Group)
        .FirstOrDefaultAsync(i => i.Id == id);

    public async Task<bool> InsertAsync(TaskItem taskItem)
    {
        if (await GetByIdAsync(taskItem.Id) != null)
            return false;
        await _context.TaskItems.AddAsync(taskItem);
        return true;
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

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var taskFromDb = await _context.TaskItems.FindAsync(new object[] { id });
        if (taskFromDb == null)
            return false;
        _context.TaskItems.Remove(taskFromDb);
        return true;
    }

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    public async Task<List<TaskItem>> GetAllUserTaskItemsAsync(int idUser) =>
        await _context.TaskItems
        .Include(i => i.Group)
        .Where(i => i.DoerId == idUser)
        .ToListAsync();

    public async Task<TaskItem?> GetUserTaskItemAsync(int idUser, int idTask) =>
        await _context.TaskItems.FirstOrDefaultAsync(i => i.DoerId == idUser && i.Id == idTask);

    public async Task<bool> InsertUserTaskItemAsync(int idUser, TaskItem taskItem)
    {
        if (taskItem.DoerId != idUser || await GetByIdAsync(taskItem.Id) != null)
            return false;
        _context.TaskItems.Add(taskItem);
        return true;
    }

    public async Task<bool> UpdateUserTaskItemAsync(int idUser, TaskItem entity)
    {
        var taskItemFromDb = await _context.TaskItems
            .FirstOrDefaultAsync(i => i.DoerId == idUser && i.Id == entity.Id);
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

    public async Task<bool> DeleteAllUserTaskItemsAsync(int idUser)
    {
        var listFromDb = await GetAllUserTaskItemsAsync(idUser);
        if (listFromDb.Count == 0)
            return false;
        _context.TaskItems.RemoveRange(listFromDb);
        return true;
    }

    public async Task<bool> DeleteUserTaskItemAsync(int idUser, int idTask)
    {
        var taskItemFromDb = await GetUserTaskItemAsync(idUser, idTask);
        if (taskItemFromDb == null)
            return false;
        _context.TaskItems.Remove(taskItemFromDb);
        return true;
    }

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
