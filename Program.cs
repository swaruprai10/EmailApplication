using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using EmailCampaignApp.Data;
using EmailCampaignApp.Models;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// Add MongoDB connection string from configuration
builder.Services.AddSingleton<AppDbContext>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config.GetConnectionString("MongoDb");
    var mongoUrl = new MongoUrl(connectionString);
    return new AppDbContext(mongoUrl.ToString(), mongoUrl.DatabaseName);
});

// Add MongoDB Identity
builder.Services.AddIdentityMongoDbProvider<ApplicationUser, MongoRole>(identityOptions =>
{
    // Configure password, lockout, etc. as before
    identityOptions.Password.RequireDigit = true;
    identityOptions.Password.RequiredLength = 8;
    identityOptions.Password.RequireNonAlphanumeric = true;
    identityOptions.Password.RequireUppercase = true;
    identityOptions.Password.RequireLowercase = true;
}, mongoIdentityOptions =>
{
    mongoIdentityOptions.ConnectionString = builder.Configuration.GetConnectionString("MongoDb");
});

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Ensure cookies are always sent over HTTPS
    options.Cookie.SameSite = SameSiteMode.Strict; // Prevent cross-site request forgery
});

// 
var migrationOptions = new MongoMigrationOptions
{
    MigrationStrategy = new MigrateMongoMigrationStrategy(),
    BackupStrategy = new CollectionMongoBackupStrategy()
};

var storageOptions = new MongoStorageOptions
{
    MigrationOptions = migrationOptions,
    CheckConnection = true,
    QueuePollInterval = TimeSpan.FromSeconds(10),
    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection // Use Tail Notifications for better performance
};


builder.Services.AddHangfire(config =>
    config.UseMongoStorage(
        builder.Configuration.GetConnectionString("MongoDb"),
        storageOptions
    )
);

// Add Hangfire Server
builder.Services.AddHangfireServer(options => options.ServerName = "MyHangfireServer");

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

// JWT Authentication configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies"; // Use cookies for MVC authentication
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // Use JWT for API authentication
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Account/Login"; // Redirect to login page if unauthorized
    options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to access denied page
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Build the app
var app = builder.Build();

// Seed Roles and Database //Asynchronous seeding of roles and database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<MongoRole>>();


    // Seed roles
    string[] roleNames = { "Admin", "User", "Editor", "Author" };
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new MongoRole(roleName));
        }
    }

    // Seed default admin user
    var adminEmail = builder.Configuration["AdminUser:Email"] ?? Environment.GetEnvironmentVariable("ADMIN_EMAIL");
    var adminPassword = builder.Configuration["AdminUser:Password"] ?? Environment.GetEnvironmentVariable("ADMIN_PASSWORD");
    if (!string.IsNullOrEmpty(adminEmail) && !string.IsNullOrEmpty(adminPassword))
    {
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true // Set to true if you want to skip email confirmation
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"Admin user {adminEmail} created and assigned to Admin role.");
            }
            else
            {
                Console.WriteLine($"Error creating admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();  // <-- session early
app.UseAuthentication(); // Enable authentication
app.UseAuthorization(); // Enable authorization


// Optional: Hangfire Dashboard
app.UseHangfireDashboard(); // Access at /hangfire

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
