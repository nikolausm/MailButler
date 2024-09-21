FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MailButler.Api/MailButler.Api.csproj", "MailButler.Api/"]
COPY ["MailButler.Configuration.AzureJson/MailButler.Configuration.AzureJson.csproj", "MailButler.Configuration.AzureJson/"]
COPY ["MailButler.Configuration/MailButler.Configuration.csproj", "MailButler.Configuration/"]
COPY ["MailButler.Dtos/MailButler.Dtos.csproj", "MailButler.Dtos/"]
COPY ["MailButler.Options/MailButler.Options.csproj", "MailButler.Options/"]
COPY ["MailButler.UseCases/MailButler.UseCases.csproj", "MailButler.UseCases/"]
COPY ["MailButler.Core/MailButler.Core.csproj", "MailButler.Core/"]
COPY ["MailButler.MailRules/MailButler.MailRules.csproj", "MailButler.MailRules/"]
RUN dotnet restore "MailButler.Api/MailButler.Api.csproj"
COPY . .
WORKDIR "/src/MailButler.Api"
RUN dotnet build "MailButler.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MailButler.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MailButler.Api.dll"]
