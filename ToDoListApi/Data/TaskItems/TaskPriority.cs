using System.Runtime.Serialization;

namespace ToDoListApi.Data.TaskItems;
public enum TaskPriority
{
    [EnumMember(Value = "Low")]
    Low,
    [EnumMember(Value = "Medium")]
    Medium,
    [EnumMember(Value = "High")]
    High
}
