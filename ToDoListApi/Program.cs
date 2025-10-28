using ToDoListApi;
using ToDoListApi.Apis;

var builder = WebApplication.CreateBuilder(args);
BeforeLaunchSettings.RegisterServices(builder);
var app = builder.Build();
BeforeLaunchSettings.Configure(app);

var allApis = app.Services.GetServices<IMyApi>();
foreach (var api in allApis)
{
    if (api is null)
        throw new InvalidProgramException("API not found.");
    api.RegisterEndPoints(app);
}

// Настраивает эндпоинт для проб доступности пода
app.MapHealthChecks("/healthz");

app.Run();

