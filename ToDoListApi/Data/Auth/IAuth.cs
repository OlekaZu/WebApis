namespace ToDoListApi.Data.Auth;

public interface IAuth
{
    Task<User?> GetUser(UserDto input);
    Task<bool> RegisterNewUser(UserDto input);
    Task<bool> UpdateUserPassword(int userId, string newPassword);
    Task<bool> UpdateUserRole(int userId, string newRole);
}
