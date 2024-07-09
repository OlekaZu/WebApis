using ToDoListApi.Data;
using ToDoListApi.Data.Auth;

namespace ToDoListApi.Apis
{
    public class UsersApi : IMyApi
    {
        public void RegisterEndPoints(WebApplication app)
        {
            app.MapGet("/users", GetAllUsers)
                .Produces<List<User>>(StatusCodes.Status200OK)
                .WithSummary("Get All Users")
                .WithDescription("Get all users from the database (only by admins).")
                .WithTags("Getters");

            app.MapGet("/users/{id}", GetUserById)
                .Produces<User>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Get User By Id")
                .WithDescription("Get user by their id from the database.")
                .WithTags("Getters")
                .RequireAuthorization("user-profile");

            app.MapPost("/users", CreateUser)
                .Accepts<User>("application/json")
                .Produces<User>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithSummary("Create New User")
                .WithDescription("Create new user in the database (only by admins).")
                .WithTags("Creators");

            app.MapPut("/users", UpdateUser)
                .Accepts<User>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Update User Fully")
                .WithDescription("Update new user in the database fully (only by admins).")
                .WithTags("Updaters");

            app.MapDelete("/users/{id}", DeleteUserById)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Delete User")
                .WithDescription("Non-admin user can delete only own account. Admins can delete any user's account.")
                .WithTags("Deleters")
                .RequireAuthorization("user-profile");
        }

        [Authorize(Roles = "admin")]
        private async Task<IResult> GetAllUsers(IRepository<User> repository) =>
            Results.Ok(await repository.GetAllAsync());

        [Authorize]
        private async Task<IResult> GetUserById(IRepository<User> repository, int id)
            => await repository.GetByIdAsync(id) is User user
            ? Results.Ok(user)
            : Results.NotFound("User not found.");

        [Authorize(Roles = "admin")]
        private async Task<IResult> CreateUser(IRepository<User> repository, [FromBody] User user)
        {
            if (!await repository.InsertAsync(user))
                return Results.BadRequest("User already exists.");
            await repository.SaveAsync();
            return Results.Created($"/users/{user.Id}", user);
        }

        [Authorize(Roles = "admin")]
        private async Task<IResult> UpdateUser(IRepository<User> repository, [FromBody] User user)
        {
            if (!await repository.UpdateAsync(user))
                return Results.NotFound();
            await repository.SaveAsync();
            return Results.NoContent();
        }

        [Authorize]
        private async Task<IResult> DeleteUserById(IRepository<User> repository, int id)
        {
            if (await repository.DeleteByIdAsync(id) == false)
                return Results.NotFound("User not found.");
            await repository.SaveAsync();
            return Results.NoContent();
        }
    }
}
