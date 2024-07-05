namespace ToDoListApi.Data.TaskItems
{
    public interface IUserTaskItemRepository
    {
        Task<List<TaskItem>> GetAllUserTaskItemsAsync(int idUser);
        Task<TaskItem?> GetUserTaskItemAsync(int idUser, int posNum);
        Task<bool> InsertUserTaskItemAsync(int idUser, TaskItem taskItem);
        Task<bool> UpdateUserTaskItemAsync(int idUser, TaskItem taskItem);
        Task<bool> DeleteAllUserTaskItemsAsync(int idUser);
        Task<bool> DeleteUserTaskItemAsync(int idUser, int posNum);
    }
}
