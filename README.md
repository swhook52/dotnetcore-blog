# dotnetcore-blog: A realistic example of using modern frameworks

I found it to be incredibly hard to get an example of how to use dotnetcore in a non hello world application. Outside of Getting Started tutorials people need to use real libraries and use them together to accomplish a goal. We want to pull from configuration files and we all have one off problems to deal with.

I got frustrated one day when I was updating dotnetcore 1.0.0 to 1.1.0 and found it incredibly hard to find answers on how to fix it. So I decided to create a "latest dotnetcore" realistic example that uses common frameworks and patterns to see how dotnetcore really handles these situations.

I chose the subject matter of a blog half because I'm mocking the getting started tutorials I am working to explain in more detail, and half because it's a concept that most of us clearly understand.

I chose SQL Server as a database provider simply out of convenience. I'd like to perhaps create a branch that uses DocumentDB in Azure as well.
 
## Current Versions
`Microsoft.NETCore.App: "1.1.1"`

`Microsoft.AspNetCore: "1.1.2"`

`Microsoft.AspNetCore.Mvc: "1.1.3"`

`NETStandard.Library": "1.6.1"`

`Microsoft.EntityFrameworkCore: "1.1.2"`


## Real World Uses
This repository shows examples of:

 - Returning proper HTTP Status code responses from aspnetcore Web API
 - Entity Framework Core with first migrations
 - Decoupling Entity Framework core from clients
 - Using appsettings secrets to hide connection strings and sensitive data
 - AutoMapper to make domain entities to safer data transfer objects
 - Unit testing projects _[coming soon]_


## Projects

### API
An aspnetcore Web API project meant to feed blog data to any client.

### Business
Business is meant for services. I use this layer as an easy layer for a client to access data based on the client's needs.

### Domain
Maybe Domain isn't a great name here. It's more a Data project. Meant to house my Entity Framework models and configuration. Domain logic is more stored in the Business project.

### DTO
The Data transfer objects project is meant to create a simple model for the calling code that does not show sensitive data and does not show irrelevant data to the calling client. DTOs are common when mapping from an Entity Framework model.

## Running the Example
First, you'll need the correct version of the dotnet sdk. You can get that here: [https://www.microsoft.com/net/download/core#/current](https://www.microsoft.com/net/download/core#/current). Be careful to be on the right tab. There is an 'LTS' (Long Term Support) tab that typically has an older version of the SDK. You want to look at the 'Latest' tab. I'm using the `.NET Core 1.0.3 SDK - Installer x64` and the `Visual Studio 2015 Tools (Preview 2) x64`.

Next, you'll need to create your secret appsettings file. As shown in `core-blob.api\Startup.cs`, you will need an `appsettings.secrets.json` file in your API root (right next to the existing `appsettings.json` for the API). This will hold your connection string for your database. You can look at `appsettings.json` to see the format that you need to use, but it should look something like this, depending on your connection, mine is using a local database.

    {
        "ConnectionStrings": {
            "BlogConnectionString": "Server=(localdb)\\mssqllocaldb;Database=BlogExample;Trusted_Connection=True;MultipleActiveResultSets=true"
        }
    }

Once you have this in place you should be good to go. You can use the command prompt to update your entity framework database using `dotnet ef` or you can use the Package Manager Console (with the default project pointing to `core-blog.domain`) and run `Update-Database`. This runs all of the migrations and creates the database in its current form.

If you have any questions or comments, feel free to contact me on twitter at [@StevenHook](https://twitter.com/stevenhook)