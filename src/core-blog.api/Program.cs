using Domain;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Asp.Versioning;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Services
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// CORS
var origins = builder.Configuration.GetValue<string>("CorsOrigins")?.Split(";") ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("OnlyConfiguredOrigins", policy =>
    {
        policy.WithOrigins(origins)
              .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
              .WithHeaders("Origin", "Authorization", "Content-Type")
              .AllowCredentials();
    });
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version"));
}).AddMvc().AddApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// OpenAPI/Swagger
builder.Services.AddOpenApi();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic));

// Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Database
var connectionString = builder.Configuration.GetConnectionString("BlogConnectionString") 
    ?? throw new InvalidOperationException("Connection string 'BlogConnectionString' not found.");

builder.Services.AddDbContext<BloggingContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null);
    })
    .ConfigureWarnings(warnings => 
        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));

// Domain and Business Services
Domain.Startup.ConfigureServices(builder.Services);
Business.Startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BloggingContext>();
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        
        // In development, we can continue without migration
        if (!app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

app.UseHttpsRedirection();
app.UseResponseCompression();

app.UseCors("OnlyConfiguredOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// JSON Serialization Context for AOT
[JsonSerializable(typeof(object))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}