using Microsoft.EntityFrameworkCore;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Models.DTO;
using RoyalCottages_API.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Set up default connection to Server SQL
builder.Services.AddDbContext<ApplicationDbContext>(
options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add AutoMapper for DTO 
// AutoMapper is a library that helps to map objects from one type to another.
// In this case, it is used to map between the Cottage model and the CottageCreateDTO and CottageUpdateDTO
// classes and User model and UserDTO class. This is useful for separating the data that is sent to the client
// from the data that is stored in the database.
builder.Services.AddAutoMapper(o =>
{
    o.CreateMap<Cottage, CottageCreateDTO>().ReverseMap(); // ReverseMap() allows mapping in both directions
    o.CreateMap<Cottage, CottageUpdateDTO>().ReverseMap(); 
    o.CreateMap<Cottage, CottageDTO>().ReverseMap();
    o.CreateMap<CottageUpdateDTO, CottageDTO>().ReverseMap();
    o.CreateMap<User, UserDTO>().ReverseMap();
});

// Add AuthService for authentication and authorization
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Seed data to Database
await SeedDataAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

}

// Use HTTPS redirection to ensure that all HTTP requests are redirected to HTTPS for security.
app.UseHttpsRedirection();

// Use authorization middleware to ensure that the API endpoints are protected and require proper authentication and authorization.
app.UseAuthorization();

// Map controller routes to the endpoints defined in the controllers.
app.MapControllers();

app.Run();

// Seed data to Database
static async Task SeedDataAsync(WebApplication app)
{
    // Create a scope to get the required services
    using var scope = app.Services.CreateScope();
    // Get the ApplicationDbContext from the service provider
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Automatically apply any migrations that have not been applied to the DB
    await context.Database.MigrateAsync();
}
