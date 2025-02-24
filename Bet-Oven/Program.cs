using SportRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportDomain.Identity;
using ProjectName.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<BetUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<UserAgreementMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using(var  scope = app.Services.CreateScope())
{
    var RoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[]
    {
         "Admin",
         "User",
         "Editor"
    };
    foreach (var role in roles)
    {
        if (!await RoleManager.RoleExistsAsync(role))
            await RoleManager.CreateAsync(new IdentityRole(role));
    }
}
using (var scope = app.Services.CreateScope())
{
    var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<BetUser>>();

    string email = "admin@admin.com";
    string sitename = "admin@admin.com";
    string password = "Kai.123";
    string email2 = "editor@edit";
    string sitename2 = "editor@edit";
    string passwrod2 = "Kai.123";
    if (await UserManager.FindByEmailAsync(email) == null)
    {
        var user = new BetUser();
        user.Email = email;
        user.UserName = sitename;
        await UserManager.CreateAsync(user, password);
        await UserManager.AddToRoleAsync(user, "Admin");

    }

    if (await UserManager.FindByEmailAsync(email2) == null)
    {
        var user2 = new BetUser();
        user2.Email = email2;
        user2.UserName = sitename2;
        await UserManager.CreateAsync(user2, passwrod2);
        await UserManager.AddToRoleAsync(user2, "Editor");

    }
}


app.Run();
