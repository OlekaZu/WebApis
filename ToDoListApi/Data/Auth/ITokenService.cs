namespace ToDoListApi.Data.Auth;

public interface ITokenService
{
    string BuildToken(string key, string issuer, User user);
}
