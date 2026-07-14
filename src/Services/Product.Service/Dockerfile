FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files using exact physical paths
COPY ["src/Services/Product.Service/Product.Service.csproj", "src/Services/Product.Service/"]
COPY ["src/Ecommerce.Shared/Ecommerce.Shared.csproj", "src/Ecommerce.Shared/"]

# Restore targeting the product service
RUN dotnet restore "src/Services/Product.Service/Product.Service.csproj"

COPY . .

# NOTE: Verify if your physical folder structure has nested "src" folders
# If the project is at root, this should be: WORKDIR "/src/src/Services/Product.Service"
# If not, adjust to match your repository's layout.
WORKDIR "/src/src/Services/Product.Service"
RUN dotnet build "Product.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Product.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.Service.dll"]