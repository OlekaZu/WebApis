using ToDoListApi.Data.Auth;
using ToDoListApi.Data.TaskGroups;
using ToDoListApi.Data.TaskItems;
namespace ToDoListApi.Data;

public class TasksDb : DbContext
{

    public TasksDb(DbContextOptions<TasksDb> options) : base(options) { }

    public DbSet<User> Users => Set<User>();

    public DbSet<TaskGroup> TaskGroups => Set<TaskGroup>();

    public DbSet<TaskItem> TaskItems => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>()
            .HasOne(i => i.Doer)
            .WithMany(u => u.UserItems)
            .HasForeignKey(i => i.DoerId);
        modelBuilder.Entity<TaskItem>()
            .HasOne(i => i.Group)
            .WithMany(g => g.GroupItems)
            .HasForeignKey(i => i.TaskGroupId);

        // init users
        User anna = new User { Id = 1, UserName = "Anna", Password = "1234", Role = "admin" };
        User victor = new User { Id = 2, UserName = "Victor", Password = "1111", Role = "user" };
        User andrew = new User { Id = 3, UserName = "Andrew", Password = "1112", Role = "user" };

        // init task groups
        TaskGroup study = new TaskGroup { Id = 1, Name = "Study", Description = "Tasks refer to studying" };
        TaskGroup work = new TaskGroup { Id = 2, Name = "Work", Description = "Tasks refer to work" };
        TaskGroup personal = new TaskGroup { Id = 3, Name = "Personal", Description = "Tasks refer to private life" };

        // init task items
        TaskItem oneTaskAnna = new TaskItem
        {
            Id = 1,
            Name = "English Studying",
            Description = "Learn 10 new words",
            DoerId = anna.Id,
            TaskGroupId = study.Id,
            Begin = new DateTime(2024, 6, 1, 10, 0, 0),
            End = new DateTime(2024, 6, 1, 13, 0, 0),
            IsCompleted = false,
            Priority = TaskPriority.Medium
        };

        TaskItem twoTaskAnna = new TaskItem
        {
            Id = 2,
            Name = "Complete issue #12",
            Description = "Create new interface, refactoring code",
            DoerId = anna.Id,
            TaskGroupId = work.Id,
            Begin = new DateTime(2024, 6, 25, 10, 0, 0),
            End = new DateTime(2024, 6, 30, 18, 0, 0),
            IsCompleted = false,
            Priority = TaskPriority.High
        };

        TaskItem threeTaskAnna = new TaskItem
        {
            Id = 3,
            Name = "Clean the house",
            Description = "Wash dishes and vacuum the house",
            DoerId = anna.Id,
            TaskGroupId = personal.Id,
            Begin = new DateTime(2024, 6, 26, 10, 0, 0),
            End = new DateTime(2024, 6, 26, 17, 45, 0),
            IsCompleted = false,
            Priority = TaskPriority.Low
        };

        TaskItem oneTaskVictor = new TaskItem
        {
            Id = 4,
            Name = "Solve Issue #1",
            Description = "Learn algorythm and implement it",
            DoerId = victor.Id,
            TaskGroupId = work.Id,
            Begin = new DateTime(2024, 6, 23, 9, 0, 0),
            End = new DateTime(2024, 6, 27, 19, 50, 0),
            IsCompleted = false,
            Priority = TaskPriority.High
        };

        TaskItem twoTaskVictor = new TaskItem
        {
            Id = 5,
            Name = "Homework",
            Description = "Water flowers",
            DoerId = victor.Id,
            TaskGroupId = personal.Id,
            Begin = new DateTime(2024, 6, 20, 7, 30, 0),
            End = new DateTime(2024, 6, 20, 8, 0, 0),
            IsCompleted = false,
            Priority = TaskPriority.Medium
        };

        TaskItem oneTaskAndrew = new TaskItem
        {
            Id = 6,
            Name = "Complete Report",
            Description = "Write report about new project and send it",
            DoerId = andrew.Id,
            TaskGroupId = work.Id,
            Begin = new DateTime(2024, 6, 1, 11, 0, 0),
            End = new DateTime(2024, 6, 1, 15, 30, 0),
            IsCompleted = false,
            Priority = TaskPriority.Medium
        };

        // add data to entities
        modelBuilder.Entity<User>().HasData(anna, victor, andrew);
        modelBuilder.Entity<TaskGroup>().HasData(study, work, personal);
        modelBuilder.Entity<TaskItem>().HasData(oneTaskAnna, twoTaskAnna, threeTaskAnna,
            oneTaskVictor, twoTaskVictor, oneTaskAndrew);
    }
}
