using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using RMERP.DAL.Models;
using Rotativa.AspNetCore;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Access Configuration
var configuration = builder.Configuration;

// Configure services
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

// Authentication settings
builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.LoginPath = new PathString("/AdminUsers/Login");
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
});

// Authorization settings
builder.Services.AddAuthorization();

// MVC and Razor Pages settings
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
    options.Conventions.AllowAnonymousToPage("/AdminUsers/Login");
});

// Database context
builder.Services.AddDbContext<RMERPContext>(options => options.UseSqlServer(configuration.GetConnectionString("RMERP")));
//builder.Services.AddDbContext<RMERPContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("RMERP")));
// Output caching
builder.Services.AddOutputCaching();

// Session settings
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
});

// Data Protection
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(configuration["RMERP_DATAPROTECTION_KEYS_PATH"]))
    .SetApplicationName("RMERP");

// Build the app
var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseCookiePolicy(); // Ensures cookie behavior

// Authentication and Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Session middleware
app.UseSession();

// Endpoints routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AdminUsers}/{action=DashBoard}/{id?}");

// Uncomment the following line if Rotativa is required
//RotativaConfiguration.Setup(configuration["RMERP_ROTATIVA_PATH"]);

// Run the app
app.Run();