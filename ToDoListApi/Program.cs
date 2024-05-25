using ToDoListApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TasksDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<TaskGroup>, TaskGroupRepository>();
builder.Services.AddScoped<IRepository<TaskItem>, TaskItemRepository>();
<<<<<<< HEAD
=======
JsonSerializerOptions options = new()
{
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    WriteIndented = true
};
>>>>>>> 409c708364e95a5fe2188cc6cbd5b73c71fafa2a

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TasksDb>();
    db.Database.EnsureCreated();
}

// Users Api
app.MapGet("/users", async (IRepository<User> repository)
    => Results.Ok(await repository.GetAllAsync()))
    .Produces<List<User>>(StatusCodes.Status200OK)
    .WithName("GetAllUsers")
    .WithTags("Getters");

app.MapGet("/users/{id}", async (IRepository<User> repository, int id)
    => await repository.GetByIdAsync(id) is IEnumerable<User> users
    ? Results.Ok(users.First())
    : Results.NotFound(Array.Empty<User>()))
    .Produces<User>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetUser")
    .WithTags("Getters");

app.MapPost("/users", async (IRepository<User> repository, [FromBody] User user) =>
    {
        if (await repository.InsertAsync(user) == false)
            return Results.BadRequest();
        await repository.SaveAsync();
        return Results.Created($"/users/{user.Id}", user);
    })
    .Accepts<User>("application/json")
    .Produces<User>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("CreateNewUser")
    .WithTags("Creators");

app.MapPut("/users", async (IRepository<User> repository, [FromBody] User user) =>
    {
        if (await repository.UpdateAsync(user) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<User>("application/json")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("UpdateUser")
    .WithTags("Updaters");

app.MapDelete("/users/{id}", async (IRepository<User> repository, int id) =>
    {
        if (await repository.DeleteByIdAsync(id) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteUser")
    .WithTags("Deleters");

// TaskGroups Api
app.MapGet("/taskgroups", async (IRepository<TaskGroup> repository)
    => Results.Ok(await repository.GetAllAsync()))
    .Produces<List<TaskGroup>>(StatusCodes.Status200OK)
    .WithName("GetAllTaskGroups")
    .WithTags("Getters");

app.MapGet("/taskgroups/{id}", async (IRepository<TaskGroup> repository, int id)
    => await repository.GetByIdAsync(id) is IEnumerable<TaskGroup> groups
    ? Results.Ok(groups.First())
    : Results.NotFound(Array.Empty<TaskGroup>()))
    .Produces<TaskGroup>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetTaskGrpoup")
    .WithTags("Getters");

app.MapPost("/taskgroups", async (IRepository<TaskGroup> repository, [FromBody] TaskGroup group) =>
    {
        if (await repository.InsertAsync(group) == false)
            return Results.BadRequest();
        await repository.SaveAsync();
        return Results.Created($"/taskgroups/{group.Id}", group);
    })
    .Accepts<TaskGroup>("application/json")
    .Produces<TaskGroup>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("CreateNewTaskGroups")
    .WithTags("Creators");

app.MapPut("/taskgroups", async (IRepository<TaskGroup> repository, [FromBody] TaskGroup group) =>
    {
        if (await repository.UpdateAsync(group) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<TaskGroup>("application/json")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("UpdateTaskGroup")
    .WithTags("Updaters");

app.MapDelete("/taskgroups/{id}", async (IRepository<TaskGroup> repository, int id) =>
    {
        if (await repository.DeleteByIdAsync(id) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteTaskGrpoup")
    .WithTags("Deleters");

// TaskItems Api
app.MapGet("/taskitems", async (IRepository<TaskItem> repository) =>
    Results.Ok(await repository.GetAllAsync()))
    .Produces<List<TaskItem>>(StatusCodes.Status200OK)
    .WithName("GetAllTaskItems")
    .WithTags("Getters");

app.MapGet("/taskitems/{idUser}", async (IRepository<TaskItem> repository, int idUser)
    => await repository.GetByIdAsync(idUser) is IEnumerable<TaskItem> items
    ? Results.Ok(items)
    : Results.NotFound(Array.Empty<TaskItem>()))
    .Produces<List<TaskItem>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetTaskItemsByUserId")
    .WithTags("Getters");

app.MapGet("/taskitems/{idUser}/{num}", async (IRepository<TaskItem> repository, int idUser, int num) =>
     {
         var res = await repository.GetBySpecifiedIdAsync(idUser, num);
         return res is TaskItem item ? Results.Ok(item) : Results.NotFound();
     })
     .Produces<TaskItem>(StatusCodes.Status200OK)
     .Produces(StatusCodes.Status404NotFound)
     .WithName("GetTaskItemByUserIdAndNumberCount")
     .WithTags("Getters");

app.MapPost("/taskitems", async (IRepository<TaskItem> repository, [FromBody] TaskItem item) =>
    {
        if (await repository.InsertAsync(item) == false)
            return Results.BadRequest();
        await repository.SaveAsync();
        return Results.Created($"/taskitems/{item.Doer?.Id}", item);
    })
    .Accepts<TaskItem>("application/json")
    .Produces<TaskItem>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("CreateNewTaskItem")
    .WithTags("Creators");

app.MapPut("/taskitems", async (IRepository<TaskItem> repository, [FromBody] TaskItem item) =>
    {
        if (await repository.UpdateAsync(item) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<TaskItem>("application/json")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("UpdateTaskItem")
    .WithTags("Updaters");

app.MapDelete("/taskitems/{idUser}/{num}", async (IRepository<TaskItem> repository, int idUser, int num) =>
    {
        if (await repository.DeleteBySpecifiedIdAsync(idUser, num) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteTaskItemByUserIdAndNumberCount")
    .WithTags("Deleters");

app.MapDelete("/taskitems/{idUser}", async (IRepository<TaskItem> repository, int idUser) =>
    {
        if (await repository.DeleteByIdAsync(idUser) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteAllTaskItemsByUserId")
    .WithTags("Deleters");

app.UseHttpsRedirection();
app.Run();

