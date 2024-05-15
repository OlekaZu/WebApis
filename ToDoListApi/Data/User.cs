namespace ToDoListApi.Data
{
    public class User
    {
        public uint Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
