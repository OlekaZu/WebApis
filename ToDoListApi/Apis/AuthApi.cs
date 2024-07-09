using ToDoListApi.Data;
using ToDoListApi.Data.Auth;

namespace ToDoListApi.Apis
{
    public class AuthApi : IMyApi
    {
        private WebApplication _app = null!;

        public void RegisterEndPoints(WebApplication app)
        {
            _app = app;

            app.MapPost("/login", Login)
                .Accepts<UserDto>("application/json")
                .Produces<string>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status401Unauthorized)
                .WithSummary("Login")
                .WithDescription("Sign in with username and password.")
                .WithTags("Auth");

            app.MapPost("/logout", Logout)
                .ExcludeFromDescription();

            app.MapPost("/register", Register)
                .Accepts<UserDto>("application/json")
                .Produces<string>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest)
                .WithSummary("Register")
                .WithDescription("Sign up for a new account.")
                .WithTags("Auth");

            app.MapPatch("/users/{id}/change-password", UpdateUserPassword)
                .Accepts<User>("application/json")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Update User Password")
                .WithDescription("Not-admin user can change only their password. Admins can change all passwords.")
                .WithTags("Updaters")
                .RequireAuthorization("user-profile");

            app.MapPatch("/users/{id}/change-role", UpdateUserRole)
                .Accepts<User>("application/json")
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Update User Role")
                .WithDescription("Change user's role (only by admins).")
                .WithTags("Updaters")
                .RequireAuthorization("user-profile");
        }

        [AllowAnonymous]
        private async Task<IResult> Login(ITokenService tokenService, IAuth auth,
            [FromBody] UserDto input)
        {
            var user = await auth.GetUser(input);
            if (user is null)
                return Results.Unauthorized();
            var token = tokenService.BuildToken(_app.Configuration["Jwt:Key"]!,
                _app.Configuration["Jwt:Issuer"]!, user);
            return Results.Ok(token);
        }

        [Authorize]
        private IResult Logout(ITokenService tokenService) =>
                Results.NoContent();

        [AllowAnonymous]
        private async Task<IResult> Register(ITokenService tokenService,
            IRepository<User> repository, IAuth auth, [FromBody] UserDto input)
        {
            if (!await auth.RegisterNewUser(input))
                return Results.BadRequest("User already exists.");
            await repository.SaveAsync();
            var newUser = await auth.GetUser(input);
            var token = tokenService.BuildToken(_app.Configuration["Jwt:Key"]!,
                _app.Configuration["Jwt:Issuer"]!, newUser!);
            return Results.Ok(token);
        }

        [Authorize]
        private async Task<IResult> UpdateUserPassword(IRepository<User> repository,
            IAuth auth, int id, string newPassword)
        {
            if (!await auth.UpdateUserPassword(id, newPassword))
                return Results.NotFound("User not found.");
            await repository.SaveAsync();
            return Results.Ok($"Password was changed for user {id}.");
        }

        [Authorize(Roles = "admin")]
        private async Task<IResult> UpdateUserRole(IRepository<User> repository, IAuth auth,
            int id, string newRole)
        {
            if (!await auth.UpdateUserRole(id, newRole))
                return Results.NotFound("User not found.");
            await repository.SaveAsync();
            return Results.Ok($"Role was changed for user {id}.");
        }
    }
}
