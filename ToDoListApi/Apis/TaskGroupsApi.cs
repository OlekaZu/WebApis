using ToDoListApi.Data;
using ToDoListApi.Data.TaskGroups;

namespace ToDoListApi.Apis
{
    public class TaskGroupsApi : IMyApi
    {
        public void RegisterEndPoints(WebApplication app)
        {
            app.MapGet("/taskgroups", GetAllTaskGroups)
                .Produces<List<TaskGroup>>(StatusCodes.Status200OK)
                .RequireAuthorization()
                .WithSummary("Get All Task Groups")
                .WithDescription("Get all task groups from the database.")
                .WithTags("Getters");

            app.MapGet("/taskgroups/{id}", GetTaskGroupById)
                .Produces<TaskGroup>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Get Task Group")
                .WithDescription("Get task group by its id from the database.")
                .WithTags("Getters");

            app.MapPost("/taskgroups", CreateTaskGroup)
                .Accepts<TaskGroup>("application/json")
                .Produces<TaskGroup>(StatusCodes.Status201Created)
                .Produces(StatusCodes.Status400BadRequest)
                .WithSummary("Create New Task Group")
                .WithDescription("Create new task group in the database (only for admins).")
                .WithTags("Creators");

            app.MapPut("/taskgroups", UpdateTaskGroup)
                .Accepts<TaskGroup>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Update Task Group Fully")
                .WithDescription("Update task group fullu in the database (only for admins).")
                .WithTags("Updaters");

            app.MapDelete("/taskgroups/{id}", DeleteTaskGroupById)
                .Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .WithSummary("Delete Task Group")
                .WithDescription("Delete task group from the database (only for admins).")
                .WithTags("Deleters");
        }

        [Authorize]
        private async Task<IResult> GetAllTaskGroups(IRepository<TaskGroup> repository) =>
            Results.Ok(await repository.GetAllAsync());

        [Authorize]
        private async Task<IResult> GetTaskGroupById(IRepository<TaskGroup> repository,
            int id) => await repository.GetByIdAsync(id) is TaskGroup group
            ? Results.Ok(group)
            : Results.NotFound("Group not found.");

        [Authorize(Roles = "admin")]
        private async Task<IResult> CreateTaskGroup(IRepository<TaskGroup> repository,
            [FromBody] TaskGroup group)
        {
            if (!await repository.InsertAsync(group))
                return Results.BadRequest();
            await repository.SaveAsync();
            return Results.Created($"/taskgroups/{group.Id}", group);
        }

        [Authorize(Roles = "admin")]
        private async Task<IResult> UpdateTaskGroup(IRepository<TaskGroup> repository,
            [FromBody] TaskGroup group)
        {
            if (!await repository.UpdateAsync(group))
                return Results.NotFound("Task group not found.");
            await repository.SaveAsync();
            return Results.NoContent();
        }

        [Authorize(Roles = "admin")]
        private async Task<IResult> DeleteTaskGroupById(IRepository<TaskGroup> repository,
            int id)
        {
            if (!await repository.DeleteByIdAsync(id))
                return Results.NotFound("Task group not found.");
            await repository.SaveAsync();
            return Results.NoContent();
        }
    }
}
