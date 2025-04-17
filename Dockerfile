# Use the .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app/hw03  # Set the working directory to /app/hw03

# Copy the .csproj file and restore dependencies
COPY hw03/*.csproj ./
RUN dotnet restore  # Restore dependencies

# Copy the rest of the files from the hw03 directory
COPY hw03/. ./  # Copy all files from hw03 directory
RUN dotnet publish -c Release -o out  # Publish the app to the 'out' folder

# Use the runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app/hw03  # Set the working directory to /app/hw03
COPY --from=build /app/hw03/out .

EXPOSE 80

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "hw03.dll"]
