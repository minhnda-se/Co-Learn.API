# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CoLearn.API/CoLearn.API.csproj", "CoLearn.API/"]
RUN dotnet restore "CoLearn.API/CoLearn.API.csproj"
COPY . .
WORKDIR "/src/CoLearn.API"
RUN dotnet publish "CoLearn.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "CoLearn.API.dll"]
