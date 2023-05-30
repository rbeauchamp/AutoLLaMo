# See https://learn.microsoft.com/en-us/dotnet/core/docker/build-container
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://code.visualstudio.com/docs/containers/troubleshooting
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/AutoLLaMo.ConsoleApp/AutoLLaMo.ConsoleApp.csproj", "src/AutoLLaMo.ConsoleApp/"]
RUN dotnet restore "src/AutoLLaMo.ConsoleApp/AutoLLaMo.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/src/AutoLLaMo.ConsoleApp"
RUN dotnet build "AutoLLaMo.ConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutoLLaMo.ConsoleApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AutoLLaMo.ConsoleApp.dll"]
