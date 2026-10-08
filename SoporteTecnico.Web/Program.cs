using Microsoft.AspNetCore.Authentication.Cookies;
using SoporteTecnico.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Cadena de conexión: las clases DAL estáticas la leen de DbContexto.ConnectionString
DbContexto.ConnectionString = builder.Configuration.GetConnectionString("SoporteTecnicoDB")
    ?? throw new InvalidOperationException(
        "Falta la cadena de conexión 'SoporteTecnicoDB' en appsettings.json.");
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.Name = "SoporteTecnico.Auth";
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();