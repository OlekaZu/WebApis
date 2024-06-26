using ToDoListApi.Data.Auth;
using ToDoListApi.Data.TaskGroups;

namespace ToDoListApi.Data.TaskItems;

public class TaskItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int DoerId { get; set; }
    public User? Doer { get; set; }
    public int PositionNumber { get; set; }

    public int TaskGroupId { get; set; }
    public TaskGroup? Group { get; set; }

    public DateTime Begin { get; set; } = DateTime.Now;
    public DateTime End { get; set; } = DateTime.Now.AddDays(1);
    public bool IsCompleted { get; set; } = false;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskPriority Priority { get; set; }
}
