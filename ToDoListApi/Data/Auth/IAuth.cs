namespace ToDoListApi.Data.Auth;

public interface IAuth
{
    Task<User?> GetUser(UserDto user);
}
