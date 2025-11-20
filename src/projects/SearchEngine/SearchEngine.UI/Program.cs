var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure API Client
var apiSettings = builder.Configuration.GetSection("ApiSettings");
var baseUrl = apiSettings.GetValue<string>("BaseUrl");

// Named Client for Search API
builder.Services.AddHttpClient("SearchApi", client =>
{
    client.BaseAddress = new Uri(baseUrl ?? "https://localhost:5103");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Bypass SSL certificate validation for development (localhost)
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
