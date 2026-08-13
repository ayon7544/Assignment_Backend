FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/AssignmentSystem.API/AssignmentSystem.API.csproj src/AssignmentSystem.API/
RUN dotnet restore src/AssignmentSystem.API/AssignmentSystem.API.csproj
COPY src/AssignmentSystem.API/. src/AssignmentSystem.API/
RUN dotnet publish src/AssignmentSystem.API/AssignmentSystem.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AssignmentSystem.API.dll"]