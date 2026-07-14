FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Services/Cart.Service/Cart.Service.csproj", "src/Services/Cart.Service/"]
COPY ["src/Shared/Ecommerce.MessageBus/ECommerce.MessageBus.csproj", "src/Shared/Ecommerce.MessageBus/"]
COPY ["src/Shared/Ecommerce.Shared/ECommerce.Shared.csproj", "src/Shared/Ecommerce.Shared/"]

RUN mkdir -p src/Shared/ECommerce.Shared && cp src/Shared/Ecommerce.Shared/ECommerce.Shared.csproj src/Shared/ECommerce.Shared/

RUN dotnet restore "./src/Services/Cart.Service/Cart.Service.csproj"

COPY . .

RUN ln -s /app/src/Shared/Ecommerce.Shared /app/src/Shared/ECommerce.Shared || true

WORKDIR "/src/src/Services/Cart.Service"
RUN dotnet build "./Cart.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Cart.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cart.Service.dll"]