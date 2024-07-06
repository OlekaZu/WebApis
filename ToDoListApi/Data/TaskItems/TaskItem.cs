using ToDoListApi.Data.Auth;
using ToDoListApi.Data.TaskGroups;

namespace ToDoListApi.Data.TaskItems;

public class TaskItem
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [Required]
    public int DoerId { get; set; }
    public User? Doer { get; set; }

    [Required]
    public int TaskGroupId { get; set; }
    public TaskGroup? Group { get; set; }

    [Required]
    public DateTime Begin { get; set; } = DateTime.Now;
    [Required]
    public DateTime End { get; set; } = DateTime.Now.AddDays(1);
    [Required]
    public bool IsCompleted { get; set; } = false;

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskPriority Priority { get; set; }
}
