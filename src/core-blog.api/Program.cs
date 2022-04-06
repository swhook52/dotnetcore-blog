using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

const string onlyConfiguredOrigins = "_OnlyConfiguredOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.secrets.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

builder.Services.AddControllers();

var origins = builder.Configuration.GetValue<string>("CorsOrigins").Split(";");
builder.Services.AddCors(options =>
{
    options.AddPolicy(onlyConfiguredOrigins,
    builder =>
    {
        builder
            .WithOrigins(origins)
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
            .WithHeaders("Origin", "Authorization", "Content-Type");
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:BlogConnectionString");

builder.Services
    .AddEntityFrameworkSqlServer()
    .AddDbContext<BloggingContext>(p => p.UseSqlServer(connectionString));

Domain.Startup.ConfigureServices(builder.Services);
Business.Startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Migrate the database to the latest version automatically on application startup
var serviceScopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var serviceScope = serviceScopeFactory.CreateScope())
{
    serviceScope.ServiceProvider.GetService<BloggingContext>().Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors(onlyConfiguredOrigins);
app.UseAuthorization();
app.UseResponseCompression();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();