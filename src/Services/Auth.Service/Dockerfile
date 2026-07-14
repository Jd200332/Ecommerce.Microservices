FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 80                           
ENV ASPNETCORE_URLS=http://+:80 

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Auth.Service/Auth.Service.csproj", "src/Services/Auth.Service/"]
COPY ["src/Shared/Ecommerce.Shared/ECommerce.Shared.csproj", "src/Shared/Ecommerce.Shared/"]
COPY ["src/Shared/Ecommerce.MessageBus/ECommerce.MessageBus.csproj", "src/Shared/Ecommerce.MessageBus/"]
RUN dotnet restore "./src/Services/Auth.Service/Auth.Service.csproj"
COPY . .
WORKDIR "/src/src/Services/Auth.Service"
RUN dotnet build "./Auth.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Auth.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.Service.dll"]