namespace ToDoListApi.Data.Auth;

public class UserRepository : IRepository<User>, IAuth
{
    private const string _defaultUserRole = "user";
    private readonly TasksDb _context;

    public UserRepository(TasksDb context)
    {
        _context = context;
    }

    public async Task<User?> GetUser(UserDto input) => await _context.Users
        .FirstOrDefaultAsync(u => u.UserName.Equals(input.UserName)
        && u.Password.Equals(input.Password));

    public async Task<bool> RegisterNewUser(UserDto input)
    {
        if (await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(input.UserName)
         || u.Password.Equals(input.Password)) is not null)
            return false;
        await _context.Users.AddAsync(new User
        {
            UserName = input.UserName,
            Password = input.Password,
            Role = _defaultUserRole
        });
        return true;
    }

    public async Task<bool> UpdateUserPassword(int userId, string newPassword)
    {
        var userFromDb = await _context.Users.FindAsync(new object[] { userId });
        if (userFromDb == null)
            return false;
        userFromDb.Password = newPassword;
        return true;
    }

    public async Task<bool> UpdateUserRole(int userId, string newRole)
    {
        var userFromDb = await _context.Users.FindAsync(new object[] { userId });
        if (userFromDb == null)
            return false;
        userFromDb.Role = newRole;
        return true;
    }

    public async Task<List<User>> GetAllAsync() => await _context.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<bool> InsertAsync(User entity)
    {
        if (_context.Users.Count() != 0 && await _context.Users.AnyAsync(u => u.Id == entity.Id
            || u.UserName == entity.UserName))
            return false;
        await _context.Users.AddAsync(entity);
        return true;
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
