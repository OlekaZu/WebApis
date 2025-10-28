#Base .Net Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development

#Publish .Net application 
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["ToDoListApi.csproj", "./"]
RUN dotnet restore "ToDoListApi.csproj"
COPY . .
RUN dotnet publish "ToDoListApi.csproj" -c Release -o /app/build

# Build final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/build .
ENTRYPOINT ["dotnet", "ToDoListApi.dll"]
