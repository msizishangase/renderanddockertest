# 1️⃣ Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore and publish
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# 2️⃣ Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Render will tell the container which port to use
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Launch the Web API
ENTRYPOINT ["dotnet", "AccountAPI.dll"]
