var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers and views
builder.Services.AddControllersWithViews();

// Register Memory Cache & Response Caching
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();

// Register Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register HttpClient for external API calls
builder.Services.AddHttpClient();

// Register HttpContextAccessor for session extensions
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware order matters: Response Caching -> Session -> Authorization
app.UseResponseCaching();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Course}/{action=Index}/{id?}");

app.Run();