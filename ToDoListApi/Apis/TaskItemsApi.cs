using ToDoListApi.Data;
using ToDoListApi.Data.TaskItems;

namespace ToDoListApi.Apis
{
    public class TaskItemsApi : IMyApi
    {
        public void RegisterEndPoints(WebApplication app)
        {
            app.MapGet("/taskitems", GetAllTasks)
                .Produces<List<TaskItem>>(StatusCodes.Status200OK)
                .WithSummary("Get All Task Items")
                .WithDescription("Get all tasks from the database (only for admins).")
                .WithTags("Getters");

            app.MapGet("/taskitems/{id}", GetTaskById)
                .Produces<List<TaskItem>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Get Task Item By Id")
                .WithDescription("Get task by its id from the database (only for admins).")
                .WithTags("Getters");

            app.MapGet("/users/{idUser}/taskitems", GetAllUserTasks)
                 .Produces<List<TaskItem>>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status404NotFound)
                 .WithSummary("Get All Task Items For Id User")
                 .WithDescription("Get all tasks of the current user or the specified user (for admins only) from the database.")
                 .WithTags("Getters")
                 .RequireAuthorization("user-profile");

            app.MapGet("/users/{idUser}/taskitems/{idTask}", GetUserTaskById)
                 .Produces<TaskItem>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status404NotFound)
                 .WithSummary("Get Task Item For User By Id")
                 .WithDescription("Get task of current or specified user (only for admins) by its id from the database.")
                 .WithTags("Getters")
                 .RequireAuthorization("user-profile");

            app.MapPost("/taskitems", CreateTask)
                .Accepts<TaskItem>("application/json")
                .Produces<TaskItem>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithSummary("Create New Task Item")
                .WithDescription("Create new task in the database (only for admins).")
                .WithTags("Creators");

            app.MapPost("/users/{idUser}/taskitems", CreateUserTask)
                .Accepts<TaskItem>("application/json")
                .Produces<TaskItem>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithSummary("Create New Task Item For Id User")
                .WithDescription("Create new task by the current user or by the specified user (for admins only) in the database.")
                .WithTags("Creators")
                .RequireAuthorization("user-profile");

            app.MapPut("/taskitems", UpdateTask)
                .Accepts<TaskItem>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithSummary("Update Task Item Fully")
                .WithDescription("Update task fully in the database (only for admins).")
                .WithTags("Updaters");

            app.MapPut("/users/{idUser}/taskitems", UpdateUserTask)
                .Accepts<TaskItem>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status400BadRequest)
                .WithSummary("Update Task Item For Id User Fully")
                .WithDescription("Update task of the current user or the specified user (only for admins) fully in the database.")
                .WithTags("Updaters")
                .RequireAuthorization("user-profile");

            app.MapDelete("/taskitems/{id}", DeleteTaskById)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Delete Task Item By Id")
                .WithDescription("Delete task by its id from the database (only for admins).")
                .WithTags("Deleters");

            app.MapDelete("/users/{idUser}/taskitems", DeleteAllUserTasks)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Delete All Task Items For Id User")
                .WithDescription("Delete all tasks of the current user or the specified user (only for admins) from the database.")
                .WithTags("Deleters")
                .RequireAuthorization("user-profile");

            app.MapDelete("/users/{idUser}/taskitems/{idTask}", DeleteUserTaskById)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Delete Task Item For User By Id")
                .WithDescription("Delete task of the current user or the specified user (only for admins) by its id from the database.")
                .WithTags("Deleters")
                .RequireAuthorization("user-profile");
        }

        [Authorize(Roles = "admin")]
        private async Task<IResult> GetAllTasks(IRepository<TaskItem> repository) =>
            Results.Ok(await repository.GetAllAsync());

        [Authorize(Roles = "admin")]
        private async Task<IResult> GetTaskById(IRepository<TaskItem> repository, int id) =>
            await repository.GetByIdAsync(id) is TaskItem item
            ? Results.Ok(item)
            : Results.NotFound("Item not found.");

        [Authorize]
        private async Task<IResult> GetAllUserTasks(IUserTaskItemRepository repository, int idUser)
        {
            var res = await repository.GetAllUserTaskItemsAsync(idUser);
            return res.Count > 0 ? Results.Ok(res)
            : Results.NotFound(Array.Empty<TaskItem>());
        }

        [Authorize]
        private async Task<IResult> GetUserTaskById(IUserTaskItemRepository repository,
            int idUser, int idTask)
        {
            var res = await repository.GetUserTaskItemAsync(idUser, idTask);
            return res is TaskItem item ? Results.Ok(item)
            : Results.NotFound("Task items not found.");
        }

        [Authorize(Roles = "admin")]
        private async Task<IResult> CreateTask(IRepository<TaskItem> repository,
        [FromBody] TaskItem item)
        {
            if (!await repository.InsertAsync(item))
                return Results.BadRequest();
            await repository.SaveAsync();
            return Results.Created($"/users/{item.Doer?.Id}/taskitems/{item.Id}", item);
        }

        [Authorize]
        private async Task<IResult> CreateUserTask(IUserTaskItemRepository tasksRepository,
            IRepository<TaskItem> repository, int idUser, [FromBody] TaskItem item)
        {
            if (!await tasksRepository.InsertUserTaskItemAsync(idUser, item))
                return Results.BadRequest();
            await repository.SaveAsync();
            return Results.Created($"/users/{item.Doer?.Id}/taskitems/{item.Id}", item);
        }

        [Authorize(Roles = "admin")]
        private async Task<IResult> UpdateTask(IRepository<TaskItem> repository,
            [FromBody] TaskItem item)
        {
            if (!await repository.UpdateAsync(item))
                return Results.NotFound("Task item not found.");
            await repository.SaveAsync();
            return Results.NoContent();
        }

        [Authorize]
        private async Task<IResult> UpdateUserTask(IUserTaskItemRepository tasksRepository,
        IRepository<TaskItem> repository, int idUser, [FromBody] TaskItem item)
        {
            if (!await tasksRepository.UpdateUserTaskItemAsync(idUser, item))
                return Results.NotFound("Task item not found.");
            await repository.SaveAsync();
            return Results.NoContent();
        }

        [Authorize(Roles = "admin")]
        private async Task<IResult> DeleteTaskById(IRepository<TaskItem> repository,
            int id)
        {
            if (!await repository.DeleteByIdAsync(id))
                return Results.NotFound("Task item not found.");
            await repository.SaveAsync();
            return Results.NoContent();
        }

        [Authorize]
        private async Task<IResult> DeleteAllUserTasks(IUserTaskItemRepository tasksRepository,
            IRepository<TaskItem> repository, int idUser)
        {
            if (!await tasksRepository.DeleteAllUserTaskItemsAsync(idUser))
                return Results.NotFound("User haven't any task items.");
            await repository.SaveAsync();
            return Results.NoContent();
        }

        [Authorize]
        private async Task<IResult> DeleteUserTaskById(IUserTaskItemRepository tasksRepository,
            IRepository<TaskItem> repository, int idUser, int idTask)
        {
            if (!await tasksRepository.DeleteUserTaskItemAsync(idUser, idTask))
                return Results.NotFound("Task item not found.");
            await repository.SaveAsync();
            return Results.NoContent();
        }
    }
}
