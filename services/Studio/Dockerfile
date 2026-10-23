# Build from the services/ folder: docker build -f Studio/Dockerfile .
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props .editorconfig ./
COPY BuildingBlocks/ BuildingBlocks/
COPY Studio/src/ Studio/src/
RUN dotnet publish Studio/src/Lexia.Studio.Api/Lexia.Studio.Api.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Lexia.Studio.Api.dll"]
