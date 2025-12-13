using System.Text.Json;
using System.Text.Json.Serialization;
using LibrarySystem.Application.Jobs;
using LibrarySystem.Application.Services;
using LibrarySystem.DataAccess.Context;
using LibrarySystem.DataAccess.Repositories;
using LibrarySystem.Domain.Interfaces.Repositories;
using LibrarySystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
});

builder.Services.AddAuthorization();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://dev-pktdx0v0anbhhjfx.us.auth0.com/";
        options.Audience = "https://library-api";
        options.RequireHttpsMetadata = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = Environment.GetEnvironmentVariable("DOCKER_CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<LibrarySystemContext>(options =>
    options.UseNpgsql(connectionString));

// Регистрация репозиториев
builder.Services.AddTransient<IBookRepository, BookRepository>();
builder.Services.AddTransient<ILibraryRepository, LibraryRepository>();
builder.Services.AddTransient<ILibraryBookRepository, LibraryBookRepository>();

// Регистрация сервисов
builder.Services.AddTransient<ILibraryService, LibraryService>();
builder.Services.AddTransient<IBookService, BookService>();
builder.Services.AddTransient<ILibraryBookService, LibraryBookService>();

builder.Services.AddScoped<InitializeDatabaseJob>();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


var app = builder.Build();

var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var context = services.GetRequiredService<LibrarySystemContext>();
var pendingMigrations = context.Database.GetPendingMigrations().ToList();
if (pendingMigrations.Any())
{
    Console.WriteLine($"Applying {pendingMigrations.Count} migrations...");
    context.Database.Migrate();
    Console.WriteLine("Migrations applied successfully");
}
else
{
    Console.WriteLine("Database is up-to-date");
}

var initDatabaseJob = services.GetRequiredService<InitializeDatabaseJob>();
await initDatabaseJob.InitializeDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();