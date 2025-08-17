using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WorkShopManager.Data;
using WorkShopManager.Models;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using WorkShopManager.Helpers;



var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSingleton<CarQueryService>();

builder.Services.AddDbContext<WorkshopContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<WorkshopContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();


var app = builder.Build();

var serviceProvider = builder.Services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!await roleManager.RoleExistsAsync("Workshop"))
    {
        await roleManager.CreateAsync(new IdentityRole("Workshop"));
    }

    if (!await roleManager.RoleExistsAsync("Client"))
    {
        await roleManager.CreateAsync(new IdentityRole("Client"));
    }

    var context = scope.ServiceProvider.GetRequiredService<WorkshopContext>();
    var carQuery = scope.ServiceProvider.GetRequiredService<CarQueryService>();

    if (!context.CarMakes.Any())
    {
        var makesWithModels = await carQuery.GetAllFromLocalJsonAsync();
        context.CarMakes.AddRange(makesWithModels);
        await context.SaveChangesAsync();
        Console.WriteLine("Dane zaimportowane z lokalnego pliku JSON.");
    }
}

app.UseStaticFiles();
app.UseRouting();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
        }
        return Task.CompletedTask;
    });

    await next();
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
