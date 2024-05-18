using ToDoListApi.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TasksDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TasksDb>();
    db.Database.EnsureCreated();
}

app.MapGet("/users", async (TasksDb db) => await db.Users.ToListAsync());
app.MapPost("/users", async ([FromBody] User user, TasksDb db)
    =>
{
    if (db.Users.Any(u => u.Id == user.Id || u.UserName == user.UserName))
        throw new Exception("User already exists");
    db.Users.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/users/{user.Id}", user);
});
app.MapPut("/users", async ([FromBody] User user, TasksDb db) =>
{
    var userFromDb = await db.Users.FindAsync(new object[] { user.Id });
    if (userFromDb == null)
        return Results.NotFound();
    userFromDb.UserName = user.UserName;
    userFromDb.Password = user.Password;
    userFromDb.Role = user.Role;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/users/{id}", async (int id, TasksDb db) =>
{
    var userFromDb = await db.Users.FindAsync(new object[] { id });
    if (userFromDb == null)
        return Results.NotFound();
    db.Users.Remove(userFromDb);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/taskgroups", async (TasksDb db) => await db.TaskGroups.ToListAsync());
app.MapGet("/taskgroups/{id}", async (int id, TasksDb db)
    => await db.TaskGroups.FirstOrDefaultAsync(g => g.Id == id) is TaskGroup group
    ? Results.Ok(group)
    : Results.NotFound());
app.MapPost("/taskgroups", async ([FromBody] TaskGroup group, TasksDb db) =>
{
    if (db.TaskGroups.Any(g => g.Id == group.Id || g.Name == group.Name))
        throw new Exception("TaskGroup already exists");
    db.TaskGroups.Add(group);
    await db.SaveChangesAsync();
    return Results.Created($"/taskgroups/{group.Id}", group);
});
app.MapPut("/taskgroups", async ([FromBody] TaskGroup group, TasksDb db) =>
{
    var groupFromDb = await db.TaskGroups.FindAsync(new object[] { group.Id });
    if (groupFromDb == null)
        return Results.NotFound();
    groupFromDb.Name = group.Name;
    groupFromDb.Description = group.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/taskgroups/{id}", async (int id, TasksDb db) =>
{
    var groupFromDb = await db.TaskGroups.FindAsync(new object[] { id });
    if (groupFromDb == null)
        return Results.NotFound();
    db.TaskGroups.Remove(groupFromDb);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/taskitems", async (TasksDb db) =>
{
    JsonSerializerOptions options = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = true
    };
    return JsonSerializer.Serialize(await db.TaskItems.Include(i => i.Doer)
        .Include(i => i.Group).ToListAsync(), options);
});
app.MapGet("/taskitems/{idUser}", async (int idUser, TasksDb db) => await db.TaskItems
    .Where(i => i.Doer != null && i.Doer.Id == idUser).ToListAsync());
app.MapGet("/taskitems/{idUser}/{num}", async (int idUser, int num, TasksDb db) =>
{
    var list = await db.TaskItems.Where(i => i.Doer != null && i.Doer.Id == idUser).ToListAsync();
    if (list.Count >= num)
        return Results.NotFound();
    return Results.Ok(list[num]);
});
app.MapPost("/taskitems", async ([FromBody] TaskItem item, TasksDb db) =>
{
    if (db.TaskItems.Any(i => i.Id == item.Id))
        throw new Exception("TaskItem already exists");
    db.TaskItems.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/taskitems/{item.Doer?.Id}", item);
});
app.MapPut("/taskitems", async ([FromBody] TaskItem item, TasksDb db) =>
{
    var taskItemFromDb = await db.TaskItems.FindAsync(new object[] { item.Id });
    if (taskItemFromDb == null)
        return Results.NotFound();
    taskItemFromDb.Name = item.Name;
    taskItemFromDb.Description = item.Description;
    taskItemFromDb.DoerId = item.DoerId;
    taskItemFromDb.TaskGroupId = item.TaskGroupId;
    taskItemFromDb.Begin = item.Begin;
    taskItemFromDb.End = item.End;
    taskItemFromDb.IsCompleted = item.IsCompleted;
    taskItemFromDb.Priority = item.Priority;
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/taskitems/{idUser}/{num}", async (int idUser, int num, TasksDb db) =>
{
    var taskItemsFromDb = await db.TaskItems.Where(i => i.Doer != null && i.Doer.Id == idUser).ToListAsync();
    if (taskItemsFromDb.Count >= num)
        return Results.NotFound();
    db.TaskItems.Remove(taskItemsFromDb[num]);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/taskitems/{idUser}", async (int idUser, TasksDb db) =>
{
    var taskItmesFromDb = await db.TaskItems.Where(x => x.Id == idUser).ToListAsync();
    db.TaskItems.RemoveRange(taskItmesFromDb);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.UseHttpsRedirection();
app.Run();

