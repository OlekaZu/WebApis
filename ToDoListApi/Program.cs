using ToDoListApi.Data;
using ToDoListApi.Data.Auth;
using ToDoListApi.Data.TaskGroups;
using ToDoListApi.Data.TaskItems;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<TasksDb>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
});

builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IAuth, UserRepository>();
builder.Services.AddScoped<IRepository<TaskGroup>, TaskGroupRepository>();
builder.Services.AddScoped<IRepository<TaskItem>, TaskItemRepository>();
builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("user-profile", policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                {
                    if (context.Resource is not HttpContext http)
                        return false;
                    string userIdFromPath = http.Request.Path.Value!.Split('/').Last();
                    return context.User.HasClaim(ClaimTypes.NameIdentifier, userIdFromPath)
                        || context.User.HasClaim(ClaimTypes.Role, "admin");
                })
        );
        options.AddPolicy("user-task", policy => policy
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
            {
                if (context.Resource is not HttpContext http)
                    return false;
                var pathSplits = http.Request.Path.Value!.Split('/');
                return context.User.HasClaim(ClaimTypes.NameIdentifier, pathSplits[1])
                    || context.User.HasClaim(ClaimTypes.Role, "admin");
            })
        );
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TasksDb>();
    db.Database.EnsureCreated();
}

// Users Api
app.MapGet("/users", [Authorize(Roles = "admin")] async (IRepository<User> repository)
    => Results.Ok(await repository.GetAllAsync()))
    .Produces<List<User>>(StatusCodes.Status200OK)
    .WithName("GetAllUsers")
    .WithTags("Getters");

app.MapGet("/users/{id}", [Authorize] async (IRepository<User> repository, int id)
    => await repository.GetByIdAsync(id) is User user
    ? Results.Ok(user)
    : Results.NotFound("User not found."))
    .Produces<User>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetUserById")
    .WithTags("Getters")
    .RequireAuthorization("user-profile");

app.MapPost("/login", [AllowAnonymous] async (ITokenService tokenService, IAuth auth,
    [FromBody] UserDto input) =>
    {
        var user = await auth.GetUser(input);
        if (user is null)
            return Results.Unauthorized();
        var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"]!,
            builder.Configuration["Jwt:Issuer"]!, user);
        return Results.Ok(token);
    })
    .Accepts<UserDto>("application/json")
    .Produces<string>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized)
    .WithName("Login")
    .WithTags("Auth");

app.MapPost("/logout", [Authorize] (ITokenService tokenService) => Results.NoContent())
    .ExcludeFromDescription();

app.MapPost("/register", [AllowAnonymous] async (ITokenService tokenService, IAuth auth,
    [FromBody] UserDto input) =>
    {
        if (!await auth.RegisterNewUser(input))
            return Results.BadRequest("User already exists.");
        return Results.RedirectToRoute("/login", input, true);
        // !! проверить работает ли с редириктом
        //var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"]!,
        //    builder.Configuration["Jwt:Issuer"]!, newUser);
        //return Results.Ok(token);
    })
    .Accepts<UserDto>("application/json")
    .Produces<string>(StatusCodes.Status308PermanentRedirect)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("Register")
    .WithTags("Auth");

app.MapPost("/users", [Authorize(Roles = "admin")] async (IRepository<User> repository, [FromBody] User user) =>
    {
        if (await repository.InsertAsync(user) == false)
            return Results.BadRequest();
        await repository.SaveAsync();
        return Results.Created($"/users/{user.Id}", user);
    })
    .Accepts<User>("application/json")
    .Produces<User>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("CreateNewUser")
    .WithTags("Creators");

app.MapPut("/users", [Authorize(Roles = "admin")] async (IRepository<User> repository, [FromBody] User user) =>
    {
        if (await repository.UpdateAsync(user) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<User>("application/json")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("UpdateFullUser")
    .WithTags("Updaters");

app.MapPatch("/users/{id}", [Authorize] async (IAuth auth, int id, string newPassword) =>
{
    if (!await auth.UpdateUserPassword(id, newPassword))
        return Results.NotFound("User not found.");
    return Results.Ok($"Password was changed for user {id}.");
})
    .Accepts<User>("application/json")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("UpdateUserPassword")
    .WithTags("Updaters")
    .RequireAuthorization("user-profile");

app.MapPatch("/users/{id}", [Authorize(Roles = "admin")] async (IAuth auth, int id, string newRole) =>
{
    if (!await auth.ChangeUserRole(id, newRole))
        return Results.NotFound("User not found.");
    return Results.Ok($"Role was changed for user {id}.");
})
    .Accepts<User>("application/json")
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("UpdateUserRole")
    .WithTags("Updaters")
    .RequireAuthorization("user-profile");

app.MapDelete("/users/{id}", [Authorize] async (IRepository<User> repository, int id) =>
    {
        if (await repository.DeleteByIdAsync(id) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteUser")
    .WithTags("Deleters")
    .RequireAuthorization("user-profile");

// TaskGroups Api
app.MapGet("/taskgroups", [Authorize] async (IRepository<TaskGroup> repository)
    => Results.Ok(await repository.GetAllAsync()))
    .Produces<List<TaskGroup>>(StatusCodes.Status200OK)
    .RequireAuthorization()
    .WithName("GetAllTaskGroups")
    .WithTags("Getters");

app.MapGet("/taskgroups/{id}", [Authorize] async (IRepository<TaskGroup> repository, int id)
    => await repository.GetByIdAsync(id) is TaskGroup group
    ? Results.Ok(group)
    : Results.NotFound("Group not found."))
    .Produces<TaskGroup>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetTaskGrpoup")
    .WithTags("Getters");

app.MapPost("/taskgroups", [Authorize(Roles = "admin")] async (IRepository<TaskGroup> repository, [FromBody] TaskGroup group) =>
    {
        if (await repository.InsertAsync(group) == false)
            return Results.BadRequest();
        await repository.SaveAsync();
        return Results.Created($"/taskgroups/{group.Id}", group);
    })
    .Accepts<TaskGroup>("application/json")
    .Produces<TaskGroup>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("CreateNewTaskGroups")
    .WithTags("Creators");

app.MapPut("/taskgroups", [Authorize(Roles = "admin")] async (IRepository<TaskGroup> repository, [FromBody] TaskGroup group) =>
    {
        if (await repository.UpdateAsync(group) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<TaskGroup>("application/json")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("UpdateTaskGroup")
    .WithTags("Updaters");

app.MapDelete("/taskgroups/{id}", [Authorize(Roles = "admin")] async (IRepository<TaskGroup> repository, int id) =>
    {
        if (await repository.DeleteByIdAsync(id) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteTaskGrpoup")
    .WithTags("Deleters");

// TaskItems Api
app.MapGet("/taskitems", [Authorize(Roles = "admin")] async (IRepository<TaskItem> repository) =>
    Results.Ok(await repository.GetAllAsync()))
    .Produces<List<TaskItem>>(StatusCodes.Status200OK)
    .WithName("GetAllTaskItems")
    .WithTags("Getters");

app.MapGet("/taskitems/{id}", [Authorize(Roles = "admin")] async (IRepository<TaskItem> repository, int id)
    => await repository.GetByIdAsync(id) is TaskItem item
    ? Results.Ok(item)
    : Results.NotFound("Item not found."))
    .Produces<List<TaskItem>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("GetTaskItemsById")
    .WithTags("Getters");

app.MapGet("/users/{idUser}/taskitems", [Authorize] async (IRepository<TaskItem> repository, int idUser, int num) =>
     {
         var res = await repository.GetBySpecifiedIdAsync(idUser, num);
         return res is TaskItem item ? Results.Ok(item) : Results.NotFound();
     })
     .Produces<TaskItem>(StatusCodes.Status200OK)
     .Produces(StatusCodes.Status404NotFound)
     .WithName("GetAllTaskItemsForIdUser")
     .WithTags("Getters")
     .RequireAuthorization("user-task");

app.MapPost("/taskitems", [Authorize(Roles = "admin")] async (IRepository<TaskItem> repository, [FromBody] TaskItem item) =>
    {
        if (await repository.InsertAsync(item) == false)
            return Results.BadRequest();
        await repository.SaveAsync();
        return Results.Created($"/taskitems/{item.Doer?.Id}", item);
    })
    .Accepts<TaskItem>("application/json")
    .Produces<TaskItem>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("CreateNewTaskItem")
    .WithTags("Creators")
.RequireAuthorization("user-task-update");

app.MapPut("/taskitems", [Authorize] async (IRepository<TaskItem> repository, [FromBody] TaskItem item) =>
    {
        if (await repository.UpdateAsync(item) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<TaskItem>("application/json")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .WithName("UpdateTaskItem")
    .WithTags("Updaters");

app.MapDelete("/taskitems/{idUser}/{num}", [Authorize] async (IRepository<TaskItem> repository, int idUser, int num) =>
    {
        if (await repository.DeleteBySpecifiedIdAsync(idUser, num) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteTaskItemByUserIdAndNumberCount")
    .WithTags("Deleters")
    .RequireAuthorization("user-task");

app.MapDelete("/taskitems/{idUser}", [Authorize] async (IRepository<TaskItem> repository, int idUser) =>
    {
        if (await repository.DeleteByIdAsync(idUser) == false)
            return Results.NotFound();
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("DeleteAllTaskItemsByUserId")
    .WithTags("Deleters")
    .RequireAuthorization("user-profile");

app.UseHttpsRedirection();
app.Run();

