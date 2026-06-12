FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app

# 1. Copy csproj files using exact paths from the repo root
COPY ["src/Services/Product.Service/Product.Service.csproj", "src/Services/Product.Service/"]
COPY ["src/Shared/Ecommerce.Shared/ECommerce.Shared.csproj", "src/Shared/Ecommerce.Shared/"]

# 2. Restore dependencies
RUN dotnet restore "src/Services/Product.Service/Product.Service.csproj"

# 3. Copy the rest of the source code
COPY . .

# 4. Move directly into the project folder and build
WORKDIR "/app/src/Services/Product.Service"
RUN dotnet build "Product.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Product.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.Service.dll"]