using ToDoListApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TasksDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IRepository<TaskGroup>, TaskGroupRepository>();
builder.Services.AddScoped<IRepository<TaskItem>, TaskItemRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TasksDb>();
    db.Database.EnsureCreated();
}

// Users Api
app.MapGet("/users", async (IRepository<User> repository)
    => Results.Ok(await repository.GetAllAsync()));

app.MapGet("/users/{id}", async (IRepository<User> repository, int id)
    => Results.Ok(await repository.GetByIdAsync(id)));

app.MapPost("/users", async (IRepository<User> repository, [FromBody] User user)
    =>
{
    if (await repository.InsertAsync(user) == false)
        return Results.BadRequest();
    await repository.SaveAsync();
    return Results.Created($"/users/{user.Id}", user);
});

app.MapPut("/users", async (IRepository<User> repository, [FromBody] User user) =>
{
    if (await repository.UpdateAsync(user) == false)
        return Results.NotFound();
    await repository.SaveAsync();
    return Results.NoContent();
});

app.MapDelete("/users/{id}", async (IRepository<User> repository, int id) =>
{
    if (await repository.DeleteByIdAsync(id) == false)
        return Results.NotFound();
    await repository.SaveAsync();
    return Results.NoContent();
});

// TaskGroups Api
app.MapGet("/taskgroups", async (IRepository<TaskGroup> repository)
    => Results.Ok(await repository.GetAllAsync()));

app.MapGet("/taskgroups/{id}", async (IRepository<TaskGroup> repository, int id)
    => Results.Ok(await repository.GetByIdAsync(id)));

app.MapPost("/taskgroups", async (IRepository<TaskGroup> repository, [FromBody] TaskGroup group) =>
{
    if (await repository.InsertAsync(group) == false)
        return Results.BadRequest();
    await repository.SaveAsync();
    return Results.Created($"/taskgroups/{group.Id}", group);
});

app.MapPut("/taskgroups", async (IRepository<TaskGroup> repository, [FromBody] TaskGroup group) =>
{
    if (await repository.UpdateAsync(group) == false)
        return Results.NotFound();
    await repository.SaveAsync();
    return Results.NoContent();
});

app.MapDelete("/taskgroups/{id}", async (IRepository<TaskGroup> repository, int id) =>
{
    if (await repository.DeleteByIdAsync(id) == false)
        return Results.NotFound();
    await repository.SaveAsync();
    return Results.NoContent();
});

// TaskItems Api
JsonSerializerOptions options = new()
{
    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    WriteIndented = true
};
app.MapGet("/taskitems", async (IRepository<TaskItem> repository) =>
    Results.Ok(JsonSerializer.Serialize(await repository.GetAllAsync(), options)));

app.MapGet("/taskitems/{idUser}", async (IRepository<TaskItem> repository, int idUser)
    => Results.Ok(JsonSerializer.Serialize(await repository.GetByIdAsync(idUser), options)));

app.MapGet("/taskitems/{idUser}/{num}", async (IRepository<TaskItem> repository, int idUser, int num) =>
 {
     var res = await repository.GetBySpecifiedIdAsync(idUser, num);
     return res is TaskItem item ? Results.Ok(item) : Results.NotFound();
 });

app.MapPost("/taskitems", async (IRepository<TaskItem> repository, [FromBody] TaskItem item) =>
{
    if (await repository.InsertAsync(item) == false)
        return Results.BadRequest();
    await repository.SaveAsync();
    return Results.Created($"/taskitems/{item.Doer?.Id}", item);
});

app.MapPut("/taskitems", async (IRepository<TaskItem> repository, [FromBody] TaskItem item) =>
{
    if (await repository.UpdateAsync(item) == false)
        return Results.NotFound();
    await repository.SaveAsync();
    return Results.NoContent();
});

app.MapDelete("/taskitems/{idUser}/{num}", async (IRepository<TaskItem> repository, int idUser, int num) =>
{
    if (await repository.DeleteBySpecifiedIdAsync(idUser, num) == false)
        return Results.NotFound();
    await repository.SaveAsync();
    return Results.NoContent();
});

app.MapDelete("/taskitems/{idUser}", async (IRepository<TaskItem> repository, int idUser) =>
{
    if (await repository.DeleteByIdAsync(idUser) == false)
        return Results.NotFound();
    await repository.SaveAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();
app.Run();

