# dotnetcore-blog: A realistic example of using modern frameworks

I found it to be incredibly hard to get an example of how to use dotnetcore in a non hello world application. Outside of Getting Started tutorials people need to use real libraries and use them together to accomplish a goal. We want to pull from configuration files and we all have one off problems to deal with.

I got frustrated one day when I was updating dotnetcore `1.0.0` to `1.1.0` and found it hard to find answers on how to fix it. So I decided to create a "latest dotnetcore" realistic example that uses common frameworks and patterns to see how dotnetcore really handles these situations.

I chose the subject matter of a blog half because I'm mocking the getting started tutorials I am working to explain in more detail, and half because it's a concept that most of us clearly understand.

I chose SQL Server as a database provider simply out of convenience.

## Updates
Over the years this project has served as a great example of building an API in .NET and how it has changed across different versions.
I'm happy that this simple little project has provided many developers a reference on certain concepts around APIs. I'll continue to
keep it updated and perhaps add a few twists on APIs that have popped up over the years this has been around.

## Current Versions
API TargetFramework: `net9.0`

Libraries: `net9.0`

Entity Framework Core: `9.0.0`


## Real World Uses
This repository shows examples of:

 - Returning proper HTTP Status code responses from ASP.NET Core Web API
 - Entity Framework Core with migrations and connection resilience
 - Decoupling Entity Framework Core from clients
 - Using appsettings secrets to hide connection strings and sensitive data
 - AutoMapper for mapping domain entities to data transfer objects
 - Modern API versioning with multiple strategies (URL, query string, headers)
 - Native .NET 9 OpenAPI integration
 - Nullable reference types for improved type safety
 - Response compression and performance optimizations
 - Configurable CORS policies for enhanced security
 - Health checks and monitoring capabilities


## Projects

### API
A .NET 9 Web API project designed to serve blog data to any client. Features modern ASP.NET Core patterns, native OpenAPI support, and enhanced security configurations.

### Business
Business logic layer containing services and application logic. This layer provides an abstraction for clients to access data based on their specific needs while maintaining separation of concerns.

### Domain
Contains Entity Framework Core models, database context, and migrations. This layer handles all data persistence concerns and database operations.

### DTO
The Data Transfer Objects project creates simplified models for API responses that hide sensitive data and provide only relevant information to calling clients. Essential for maintaining clean API contracts and security.


## Running the Example

### Prerequisites
You'll need the .NET 9 SDK. Download it from: [https://dotnet.microsoft.com/download/dotnet/9.0](https://dotnet.microsoft.com/download/dotnet/9.0)

You can verify your installation by running:
```bash
dotnet --version
# Should show 9.0.x
```

### Database Setup
First, you'll need to create your secret appsettings file. Create an `appsettings.secrets.json` file in the API root directory (`src/core-blog.api/`) next to the existing `appsettings.json`. This file will contain your database connection string:

```json
{
    "ConnectionStrings": {
        "BlogConnectionString": "Server=(localdb)\\mssqllocaldb;Database=CoreBlogNet9;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true"
    }
}
```

### Database Migration
Update your database to the current version using Entity Framework Core tools:

**Using Command Line:**
```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Run migrations
dotnet ef database update --project src/core-blog.domain --startup-project src/core-blog.api
```

**Using Package Manager Console in Visual Studio:**
Set the default project to `core-blog.domain` and run:
```powershell
Update-Database
```

### Running the Application
```bash
# From the solution root
dotnet run --project src/core-blog.api

# Or navigate to the API project
cd src/core-blog.api
dotnet run
```

The application will start and listen on:
- **HTTPS**: `https://localhost:5001`
- **HTTP**: `http://localhost:5000`

### API Endpoints
Once running, you can access these endpoints:

**Get all posts:**
```
GET https://localhost:5001/api/v1.0/posts
```

**Get specific post:**
```
GET https://localhost:5001/api/v1.0/post/{slug}
```

**Create new post:**
```
POST https://localhost:5001/api/v1.0/post
Content-Type: application/json

{
    "title": "My First .NET 9 Post",
    "body": "This is a post created with the modernized .NET 9 blog API",
    "slug": "first-net9-post",
    "tags": ["dotnet9", "api", "modernization"]
}
```

### API Documentation
In development mode, you can access the native OpenAPI documentation:
- **OpenAPI JSON**: `https://localhost:5001/openapi/v1.json`
- **Swagger UI**: Available through the development interface

### API Versioning
The API supports multiple versioning strategies:
- **URL Segment**: `/api/v1.0/posts`
- **Query Parameter**: `/api/posts?version=1.0`
- **Header**: `X-Version: 1.0`


If you have any questions or comments, feel free to contact me on BlueSky at [@stevenhook.bsky.social](https://bsky.app/profile/stevenhook.bsky.social)