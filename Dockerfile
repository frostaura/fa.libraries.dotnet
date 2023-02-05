# Specify base image.
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy everything and restore / publish the solution.
COPY . ./
RUN dotnet build ./src/FrostAura.<DIVISION>.<APPLICATION>/FrostAura.<DIVISION>.<APPLICATION>.csproj
RUN dotnet publish ./src/FrostAura.<DIVISION>.<APPLICATION>/FrostAura.<DIVISION>.<APPLICATION>.csproj -c Release -o /app/out

# Build runtime image off the correct base.
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "FrostAura.<DIVISION>.<APPLICATION>.dll"]
EXPOSE 80