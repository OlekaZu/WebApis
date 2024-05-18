
namespace ToDoListApi.Data
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int DoerId { get; set; }
        public User? Doer { get; set; }

        public int TaskGroupId { get; set; }
        public TaskGroup? Group { get; set; }

        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public bool IsCompleted { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskPriority Priority { get; set; }
    }
}
