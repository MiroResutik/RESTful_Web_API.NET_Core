using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using RoyalCottage_DTO;
using RoyalCottages_API.Data;
using RoyalCottages_API.Models;
using RoyalCottages_API.Services;
using RoyalCottages_API.Services.IServices;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Get the secret key from the configuration for JWT token generation and validation
var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtSettings")["Secret"]);

// Add services to the container.

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

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

// Add the ImageService to the dependency injection container, which implements the IImageService interface
builder.Services.AddScoped<IImageService, ImageService>();

// Add the TokenService to the dependency injection container, which implements the ITokenService interface
builder.Services.AddScoped<ITokenService, TokenService>();

// Add API versioning services to the container and configure the default API version and reporting of API versions
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true; // Assume the default API version when no version is specified in the request
    options.DefaultApiVersion = new ApiVersion(1.0); // Set the default API version to 1.0
    options.ReportApiVersions = true; // Report the supported API versions in the response headers
}).AddApiExplorer(option => // Add API explorer services to the container and configure the group name format and substitution of API version in the URL
{
    option.GroupNameFormat = "'v'VVV"; // Set the group name format for API versioning to "v{version}" (e.g., v1, v2, etc.)
    option.SubstituteApiVersionInUrl = true; // Substitute the API version in the URL when generating links to the API endpoints
});

// Add CORS services to the container to allow cross-origin requests from any origin, with any header and method, and expose all headers in the response
builder.Services.AddCors();

// Set up default connection to Server SQL
builder.Services.AddDbContext<ApplicationDbContext>(
options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container for controllers, which handle incoming HTTP requests and return responses
builder.Services.AddControllers();

// Add API version description provider to the container, which provides information about the available API versions and their descriptions
var builderProvider = builder.Services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

// Add OpenAPI services to the container for API documentation and testing
foreach (var description in builderProvider.ApiVersionDescriptions)
{
    var versionName = description.GroupName; // Get the group name for the API version (e.g., "v1", "v2", etc.)
    var versionNumber = description.ApiVersion.ToString(); // Get the API version number as a string (e.g., "1.0", "2.0", etc.)
    var displayName = $"Demo API -- {versionNumber}";

    // Add OpenAPI services to the container for API documentation and testing
    builder.Services.AddOpenApi(versionName,options =>
    {
        // Add a document transformer to customize the OpenAPI document for JWT authentication
        options.AddDocumentTransformer((document, context, cancellationToken) =>
        {
            document.Info = new OpenApiInfo
            {
                Title = "Royal Cottage API",
                Version = versionName,
                Description = displayName,
                Contact = new OpenApiContact
                {
                    Name = "Miro Resutik",
                    Email = "miro@ApiTest.co.uk"
                }
            };
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

}


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
    //o.CreateMap<User, UserDTO>().ReverseMap(); 
    o.CreateMap<ApplicationUser, UserDTO>().ReverseMap();
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
    // Use Swagger for API documentation and testing in development environment
    app.MapOpenApi("/openapi/{documentName}.json");

    // Get the API version description provider from the service provider
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    // Map the OpenAPI documents for each API version to the corresponding endpoint
    app.MapScalarApiReference(option=>
    {
        option.Title = "Demo - Royal Cottage API";

        // Sort the API version descriptions by version number in ascending order
        var sortedVersion = provider.ApiVersionDescriptions.OrderBy(v => v.ApiVersion).ToList();

        // Loop through each API version description and add a document for it to the OpenAPI reference
        foreach (var description in sortedVersion)
        {
            var versionName = description.GroupName;
            var versionNumber = description.ApiVersion.ToString();
            var displayName = $"Demo API -- {versionNumber}";

            var isDefault = description.ApiVersion.Equals(new ApiVersion(2.0));

            option.AddDocument(versionName, displayName, $"/openapi/{versionName}.json", isDefault);

        }
        
    });

}

// Use static files middleware to serve static files (e.g., images, CSS, JavaScript) from the wwwroot folder
app.UseStaticFiles();

// No restriction on API Endpoint
app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("*"));

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
