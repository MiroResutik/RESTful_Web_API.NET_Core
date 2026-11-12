using Microsoft.AspNetCore.Authentication.Cookies;
using RoyalCottage_DTO;
using RoyalCottage_Web.Services;
using RoyalCottage_Web.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register AutoMapper with the specified mappings
builder.Services.AddAutoMapper(o =>
{
    o.CreateMap<CottageDTO, CottageCreateDTO>().ReverseMap(); // ReverseMap() allows mapping in both directions
    o.CreateMap<CottageDTO, CottageUpdateDTO>().ReverseMap();

});

// Register the IHttpContextAccessor service to access the current HTTP context
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true; // Set the HttpOnly flag for the authentication cookie to enhance security
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Set the expiration time for the authentication cookie to 60 minutes
        options.SlidingExpiration = true; // Enable sliding expiration, which extends the expiration time if the user is active
        options.LoginPath = "/auth/login"; // Set the login path for unauthenticated users to redirect to the login page
        options.AccessDeniedPath = "/auth/accessdenied"; // Set the access denied path for users without sufficient permissions to redirect to the access denied page
    });

// Register the CottageService with the dependency injection container
builder.Services.AddHttpClient("RoyalCottageAPI", client =>
{
    // Retrieve the CottageAPI URL from the configuration settings
    var cottageAPIUrl = builder.Configuration.GetValue<string>("ServiceUrl:CottageAPI");

    // Set the base address for the HttpClient to the CottageAPI URL
    client.BaseAddress = new Uri(cottageAPIUrl);

    // Add the "Accept" header to indicate that the client expects JSON responses
    client.DefaultRequestHeaders.Add("Accept", "application/json");

});

// Register the ICottageService interface with its implementation CottageService
builder.Services.AddScoped<ICottageService, CottageService>();

// Register the IAuthService interface with its implementation AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Enable authentication middleware to handle user authentication and manage authentication cookies
app.UseAuthentication();

// Enable authorization middleware to enforce access control based on user roles and permissions
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
