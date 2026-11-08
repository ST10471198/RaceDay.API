using RaceDay.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Enable Session (to remember who is logged in)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// 2. Register ApiService (The bridge to your API)
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    // Ensure this port matches your API project's port
    client.BaseAddress = new Uri("https://localhost:7012/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. Session MUST be used before Authorization and Endpoints
app.UseSession();
app.UseAuthorization();

// 4. Set the default route to the Home page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();