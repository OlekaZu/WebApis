using ToDoListApi.Data;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var users = new List<User>();
var taskGroups = new List<TaskGroup>();
var taskItems = new List<TaskItem>();

app.MapGet("/users", () => users);
app.MapGet("/users/{id}", (int id) => users.FirstOrDefault(u => u.Id == id));
app.MapPost("/users", (User user) => users.Add(user));
app.MapPut("/users", (User user) =>
{
    var index = users.FindIndex(u => u.Id == user.Id);
    if (index < 0)
        throw new Exception("User not found");
    users[index] = user;
});
app.MapDelete("/users/{id}", (int id) =>
{
    var index = users.FindIndex(u => u.Id == id);
    if (index < 0)
        throw new Exception("User not found");
    users.RemoveAt(index);
});

app.MapGet("/taskgroups", () => taskGroups);
app.MapGet("/taskgroups/{id}", (int id) => taskGroups.FirstOrDefault(g => g.Id == id));
app.MapPost("/taskgroups", (TaskGroup group) => taskGroups.Add(group));
app.MapPut("/taskgroups", (TaskGroup group) =>
{
    var index = taskGroups.FindIndex(g => g.Id == group.Id);
    if (index < 0)
        throw new Exception("TaskGroup not found");
    taskGroups[index] = group;
});
app.MapDelete("/taskgroups/{id}", (int id) =>
{
    var index = taskGroups.FindIndex(g => g.Id == id);
    if (index < 0)
        throw new Exception("TaskGroup not found");
    taskGroups.RemoveAt(index);
});

app.MapGet("/taskitems", () => taskItems);
app.MapGet("/taskitems/{idUser}/{id}", (int idUser, int id)
    => taskItems.FirstOrDefault(i => i.Id == id && i.Doer?.Id == idUser));
app.MapGet("/taskitems/{idUser}", (int idUser) => taskItems.Where(i => i.Doer?.Id == idUser));
app.MapPost("/taskitems", (TaskItem item) => taskItems.Add(item));
app.MapPut("/taskitems", (TaskItem item) =>
{
    var index = taskItems.FindIndex(i => i.Id == item.Id && i.Doer?.Id == item.Doer?.Id);
    if (index < 0)
        throw new Exception("TaskItem not found");
    taskItems[index] = item;
});
app.MapDelete("/taskitems/{idUser}/{id}", (int idUser, int id) =>
{
    var index = taskItems.FindIndex(i => i.Id == id && i.Doer?.Id == idUser);
    if (index < 0)
        throw new Exception("TaskItem not found");
    taskItems.RemoveAt(index);
});
app.MapDelete("/taskitems/{idUser}", (int idUser) => taskItems.RemoveAll(i => i.Doer?.Id == idUser));

app.Run();

