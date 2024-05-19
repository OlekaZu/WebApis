namespace ToDoListApi.Data
{
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
            TaskItem oneTask = new TaskItem
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

            TaskItem twoTask = new TaskItem
            {
                Id = 2,
                Name = "Finish Code",
                Description = "Solve Issue 12",
                DoerId = anna.Id,
                TaskGroupId = work.Id,
                Begin = new DateTime(2024, 6, 1, 9, 0, 0),
                End = new DateTime(2024, 6, 1, 19, 0, 0),
                IsCompleted = false,
                Priority = TaskPriority.High
            };

            TaskItem threeTask = new TaskItem
            {
                Id = 3,
                Name = "Homework",
                Description = "Water flowers",
                DoerId = victor.Id,
                TaskGroupId = personal.Id,
                Begin = new DateTime(2024, 6, 1, 19, 0, 0),
                End = new DateTime(2024, 6, 1, 19, 30, 0),
                IsCompleted = false,
                Priority = TaskPriority.Low
            };

            TaskItem fourTask = new TaskItem
            {
                Id = 4,
                Name = "Complete Report",
                Description = "Solve Issue 5",
                DoerId = andrew.Id,
                TaskGroupId = work.Id,
                Begin = new DateTime(2024, 6, 1, 11, 0, 0),
                End = new DateTime(2024, 6, 1, 15, 30, 0),
                IsCompleted = false,
                Priority = TaskPriority.High
            };

            // add data to entities
            modelBuilder.Entity<User>().HasData(anna, victor, andrew);
            modelBuilder.Entity<TaskGroup>().HasData(study, work, personal);
            modelBuilder.Entity<TaskItem>().HasData(oneTask, twoTask, threeTask, fourTask);
        }
    }
}
