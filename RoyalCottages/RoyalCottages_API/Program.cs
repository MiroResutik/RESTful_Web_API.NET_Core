using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Models.DTO;
using RoyalCottages_API.Services;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Get the secret key from the configuration for JWT token generation and validation
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtSettings")["Secret"]);

// Add services to the container.

// Add authentication services to the container and configure JWT bearer authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default authentication scheme to JWT bearer
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default challenge scheme to JWT bearer
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // Set the default scheme to JWT bearer
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to false for testing
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters // Set the token validation parameters for JWT token validation
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key), // validate the signing key using the secret key from the configuration
        ValidateIssuer = false, // set to false for testing
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero // set the clock skew to zero to prevent token expiration issues
    };
});

// Set up default connection to Server SQL
builder.Services.AddDbContext<ApplicationDbContext>(
options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container for controllers, which handle incoming HTTP requests and return responses
builder.Services.AddControllers();

// Add OpenAPI services to the container for API documentation and testing
builder.Services.AddOpenApi(options =>
{
    // Add a document transformer to customize the OpenAPI document for JWT authentication
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            // Add a security scheme for JWT bearer authentication to the OpenAPI document
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter JWT Bearer token"
            }
        };

        // Document security array - Add a security requirement to the OpenAPI document to require JWT authentication for all endpoint
        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                {new OpenApiSecuritySchemeReference("Bearer"), new List<string>() }
            }
        ];

        // Return a completed task to indicate that the document transformation is complete
        return Task.CompletedTask;
    });
});

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
    o.CreateMap<CottageAmenities, CottageAmenitiesCreateDTO>().ReverseMap();
    o.CreateMap<CottageAmenities, CottageAmenitiesUpdateDTO>().ReverseMap();
    o.CreateMap<CottageAmenities, CottageAmenitiesDTO>()
    .ForMember(destination =>destination.CottageName, opt=>opt.MapFrom(src=>src.Cottage!=null? src.Cottage.Name : null));
    o.CreateMap<CottageAmenitiesDTO, CottageAmenities>();

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

// Use authentication middleware to validate the JWT token in incoming requests.
app.UseAuthentication();
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
