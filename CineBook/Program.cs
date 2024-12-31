using CineBook.ApplicationDbContext;
using CineBook.Interface;
using CineBook.Models;
using CineBook.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<AppDbContextion>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CineBookConnectionString")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Set desired options
    options.Password.RequiredLength = 1;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<AppDbContextion>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IEmailService, EmailService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapGet("/", (HttpContext context) =>
{
    // Check if the user is authenticated
    if (!context.User.Identity.IsAuthenticated)
    {
        // Redirect unauthenticated users to the login page
        context.Response.Redirect("/Auth/signin");
        return Task.CompletedTask;
    }

    else if (context.User.Identity.IsAuthenticated)
    {
        if (context.Request.Path.StartsWithSegments("/Auth/signin") || context.Request.Path.StartsWithSegments("/Auth/signup"))
        {
            context.Response.Redirect("/Home/Index");
            return Task.CompletedTask;
        }

        context.Response.Redirect("/Home/Index");
        return Task.CompletedTask;
    }

    context.Response.Redirect("/Home/Index");
    return Task.CompletedTask;
});



app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
