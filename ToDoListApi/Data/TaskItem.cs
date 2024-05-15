namespace ToDoListApi.Data
{
    public class TaskItem
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskGroup? Group { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public bool IsCompleted { get; set; }
        public User? Doer { get; set; }
    }
}
