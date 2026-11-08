using RaceDay.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add MVC Controllers with Views
builder.Services.AddControllersWithViews();

// 2. Register HttpContextAccessor to read session in Razor Views (_Layout.cshtml)
builder.Services.AddHttpContextAccessor();

// 3. Register HttpClient for API communication
builder.Services.AddHttpClient("RaceDayAPI", client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5289/api/";
    client.BaseAddress = new Uri(apiBaseUrl);
});

// 4. Register custom API Services
builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<OrganiserApiService>();

// 5. Configure Session State for authentication and role management
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 6. Enable Session Middleware BEFORE Authorization
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();