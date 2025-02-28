FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Dometrain.Cart.Api/Dometrain.Cart.Api.csproj", "src/Dometrain.Cart.Api/"]
COPY ["src/Dometrain.Api.Shared/Dometrain.Api.Shared.csproj", "src/Dometrain.Api.Shared/"]
COPY ["src/Dometrain.Aspire.ServiceDefaults/Dometrain.Aspire.ServiceDefaults.csproj", "src/Dometrain.Aspire.ServiceDefaults/"]
COPY ["src/Dometrain.Monolith.Api.Sdk/Dometrain.Monolith.Api.Sdk.csproj", "src/Dometrain.Monolith.Api.Sdk/"]
COPY ["src/Dometrain.Monolith.Api.Contracts/Dometrain.Monolith.Api.Contracts.csproj", "src/Dometrain.Monolith.Api.Contracts/"]
RUN dotnet restore "src/Dometrain.Cart.Api/Dometrain.Cart.Api.csproj"
COPY . .
WORKDIR "/src/src/Dometrain.Cart.Api"
RUN dotnet build "Dometrain.Cart.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Dometrain.Cart.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dometrain.Cart.Api.dll"]
