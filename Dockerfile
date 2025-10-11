# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and restore dependencies
COPY BEAPI.sln ./
COPY BEAPI/BEAPI.csproj BEAPI/
RUN dotnet restore BEAPI/BEAPI.csproj

# Copy all source code
COPY . .
WORKDIR /src/BEAPI

# Publish app
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "BEAPI.dll"]

RUN apt-get update && apt-get install -y tzdata
ENV TZ=Asia/Ho_Chi_Minh
