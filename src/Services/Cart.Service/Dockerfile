FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy using exact relative paths from the workspace root (without `./` prefixes)
COPY ["src/Services/Cart.Service/Cart.Service.csproj", "src/Services/Cart.Service/"]
COPY ["Ecommerce.MessageBus/Ecommerce.MessageBus.csproj", "Ecommerce.MessageBus/"]
COPY ["Ecommerce.Shared/Ecommerce.Shared.csproj", "Ecommerce.Shared/"]

RUN dotnet restore "src/Services/Cart.Service/Cart.Service.csproj"

COPY . .

WORKDIR "/src/src/Services/Cart.Service"
RUN dotnet build "Cart.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cart.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cart.Service.dll"]