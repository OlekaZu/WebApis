namespace ToDoListApi.Data
{
    public class TaskGroupRepository : IRepository<TaskGroup>
    {
        private readonly TasksDb _context;

        public TaskGroupRepository(TasksDb context)
        {
            _context = context;
        }

        public async Task<List<TaskGroup>> GetAllAsync()
            => await _context.TaskGroups.ToListAsync();

        public async Task<List<TaskGroup>> GetByIdAsync(int id)
            => await _context.TaskGroups.Where(u => u.Id == id).ToListAsync();

        public async Task<TaskGroup?> GetBySpecifiedIdAsync(int id, int specifiedId)
            => await _context.TaskGroups.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<bool> InsertAsync(TaskGroup entity)
        {
            var check = await _context.TaskGroups.AnyAsync(u => u.Id == entity.Id
                || u.Name == entity.Name);
            if (!check)
                await _context.TaskGroups.AddAsync(entity);
            return check;
        }

        public async Task<bool> UpdateAsync(TaskGroup entity)
        {
            var groupFromDb = await _context.TaskGroups.FindAsync(new object[] { entity.Id });
            if (groupFromDb == null)
                return false;
            groupFromDb.Name = entity.Name;
            groupFromDb.Description = entity.Description;
            return true;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var groupFromDb = await _context.TaskGroups.FindAsync(new object[] { id });
            if (groupFromDb == null)
                return false;
            _context.TaskGroups.Remove(groupFromDb);
            return true;
        }

        public async Task<bool> DeleteBySpecifiedIdAsync(int id, int specifiedId)
            => await DeleteByIdAsync(id);

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
}
