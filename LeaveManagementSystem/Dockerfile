# Étape 1 : build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY *.sln .
COPY LeaveManagementSystem/*.csproj ./LeaveManagementSystem/
RUN dotnet restore

COPY . .
WORKDIR /app/LeaveManagementSystem
RUN dotnet publish -c Release -o out

# Étape 2 : runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/LeaveManagementSystem/out ./

EXPOSE 80
ENTRYPOINT ["dotnet", "LeaveManagementSystem.dll"]
