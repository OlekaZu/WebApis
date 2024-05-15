using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ToDoListApi.Data
{
    public enum TaskGroupPriority
    {
        [EnumMember(Value = "Low")]
        Low,
        [EnumMember(Value = "Medium")]
        Medium,
        [EnumMember(Value = "High")]
        High
    }

    public class TaskGroup
    {
        public uint Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskGroupPriority Priority { get; set; }
    }
}
