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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
