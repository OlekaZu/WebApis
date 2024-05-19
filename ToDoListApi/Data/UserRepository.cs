
namespace ToDoListApi.Data
{
    public class UserRepository : IRepository<User>
    {
        private readonly TasksDb _context;

        public UserRepository(TasksDb context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task<List<User>> GetByIdAsync(int id)
            => await _context.Users.Where(u => u.Id == id).ToListAsync();

        public async Task<User?> GetBySpecifiedIdAsync(int id, int specifiedId)
            => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<bool> InsertAsync(User entity)
        {
            var check = await _context.Users.AnyAsync(u => u.Id == entity.Id
                || u.UserName == entity.UserName);
            if (!check)
                await _context.Users.AddAsync(entity);
            return check;
        }

        public async Task<bool> UpdateAsync(User entity)
        {
            var userFromDb = await _context.Users.FindAsync(new object[] { entity.Id });
            if (userFromDb == null)
                return false;
            userFromDb.UserName = entity.UserName;
            userFromDb.Password = entity.Password;
            userFromDb.Role = entity.Role;
            return true;
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var userFromDb = await _context.Users.FindAsync(new object[] { id });
            if (userFromDb == null)
                return false;
            _context.Users.Remove(userFromDb);
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
