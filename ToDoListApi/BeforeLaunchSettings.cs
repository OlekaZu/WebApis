using ToDoListApi.Apis;
using ToDoListApi.Data;
using ToDoListApi.Data.Auth;
using ToDoListApi.Data.TaskGroups;
using ToDoListApi.Data.TaskItems;

namespace ToDoListApi
{
	public static class BeforeLaunchSettings
	{
		public static void RegisterServices(WebApplicationBuilder builder)
		{
			var services = builder.Services;
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen(options =>
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

			services.AddDbContext<TasksDb>(options =>
			{
				options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
			});

			services.AddScoped<IRepository<User>, UserRepository>();
			services.AddScoped<IAuth, UserRepository>();
			services.AddScoped<IRepository<TaskGroup>, TaskGroupRepository>();
			services.AddScoped<IRepository<TaskItem>, TaskItemRepository>();
			services.AddScoped<IUserTaskItemRepository, TaskItemRepository>();
			services.AddSingleton<ITokenService>(new TokenService());

			services.AddAuthorization(options =>
			{
				options.AddPolicy("user-profile", policy => policy
					.RequireAuthenticatedUser()
					.RequireAssertion(context =>
					{
						if (context.Resource is not HttpContext http)
							return false;
						var pathSplits = http.Request.Path.Value!.Split('/');
						return context.User.HasClaim(ClaimTypes.NameIdentifier, pathSplits[2])
							|| context.User.HasClaim(ClaimTypes.Role, "admin");
					})
				);
			});

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

			services.AddTransient<IMyApi, AuthApi>();
			services.AddTransient<IMyApi, UsersApi>();
			services.AddTransient<IMyApi, TaskGroupsApi>();
			services.AddTransient<IMyApi, TaskItemsApi>();
			// Регистрируем кастомную проверку с именем "random-failure"
			services.AddHealthChecks().AddCheck<RandomFailingHealthCheck>("random-failure");
		}

		public static void Configure(WebApplication app)
		{
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
			app.UseHttpsRedirection();
		}
	}
}
