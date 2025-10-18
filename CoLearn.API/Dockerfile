# ===============================
# BUILD STAGE
# ===============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file(s) and restore
COPY ["CoLearn.API/CoLearn.API.csproj", "CoLearn.API/"]
RUN dotnet restore "CoLearn.API/CoLearn.API.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/CoLearn.API"
RUN dotnet publish "CoLearn.API.csproj" -c Release -o /app/publish

# ===============================
# RUNTIME STAGE
# ===============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Tell Azure which port to use
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Optional: for logging clarity
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENTRYPOINT ["dotnet", "CoLearn.API.dll"]
