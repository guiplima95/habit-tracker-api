FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["HabitTracker.Worker.csproj", "./"]
RUN dotnet restore "./HabitTracker.Worker.csproj"

COPY . .
RUN dotnet publish "./HabitTracker.Worker.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "HabitTracker.Worker.dll"]