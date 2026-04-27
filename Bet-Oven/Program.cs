using SportRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportDomain.Identity;
using ProjectName.Middleware;
using SportRepository.Implementation;
using SportRepository.Interface;
using SportService.Interface;
using SportService.Implementation;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<BetUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<FootballApiService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<BetUser>>();

    var roles = new[] { "Admin", "User", "Editor" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string adminEmail = "admin@admin.com";
    string adminPassword = "Kai.123";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new BetUser
        {
            Email = adminEmail,
            UserName = adminEmail
        };

        await userManager.CreateAsync(admin, adminPassword);
        await userManager.AddToRoleAsync(admin, "Admin");
    }

    string editorEmail = "editor@edit";
    string editorPassword = "Kai.123";

    if (await userManager.FindByEmailAsync(editorEmail) == null)
    {
        var editor = new BetUser
        {
            Email = editorEmail,
            UserName = editorEmail
        };

        await userManager.CreateAsync(editor, editorPassword);
        await userManager.AddToRoleAsync(editor, "Editor");
    }
}

app.Run();