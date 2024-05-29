using ToDoListApi.Data.TaskItems;

namespace ToDoListApi.Data.Auth;

public class User
{
    public int Id { get; set; }
    [Required]
    public string UserName { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    [JsonIgnore]
    public List<TaskItem> UserItems { get; set; } = [];
}
