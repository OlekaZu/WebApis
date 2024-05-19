namespace ToDoListApi.Data
{

    public class TaskGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [JsonIgnore]
        public List<TaskItem> GroupItems { get; set; } = [];
    }
}
