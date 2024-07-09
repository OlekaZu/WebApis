## Web Apis Examples
### ToDoListApi
Minimal WebApi for scheduling and tracking your tasks.

#### Stack:
- Asp.Net Core 8
- Entity Framework 8
- Sqlite
- JwtBearer (for authentication and authorization)
- Swagger UI

#### Consists of such APIs as:
- AuthApi - responsable for login, registration, change of user's password and role.
- UsersApi - responsable for CRUD operations of **Users DbSet**
- TaskGroupsApi - responsable for CRUD operations of **TaskGroups DbSet**
- TaskItems - responsable for CRUD operations of **TaskItems DbSet**

Each User can have a lot of task items. Each task group can combine with several task items.


