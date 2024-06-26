namespace ToDoListApi.Data.Auth;

public interface IAuth
{
    Task<User?> GetUser(UserDto user);
    Task<bool> RegisterNewUser(UserDto user);
    Task<bool> UpdateUserPassword(int userId, string newPassword);
    Task<bool> ChangeUserRole(int userId, string newRole);
}
