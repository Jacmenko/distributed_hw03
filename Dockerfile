# Use the .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app/hw03

# Copy the .csproj file and restore dependencies
COPY hw03/*.csproj ./
RUN dotnet restore

# Copy the rest of the files from the hw03 directory
COPY hw03/. ./
RUN dotnet publish -c Release -o /app/hw03/out

# Use the runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app/hw03

# Copy the published output from the build image
COPY --from=build /app/hw03/out .

EXPOSE 80

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "hw03.dll"]
