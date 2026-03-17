using CheckPoint.Models.Profiles;
using CheckPoint.Models.Users;
using CheckPoint.Services;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configuración de MongoDB
builder.Services.Configure<ConfiguracionMongoDb>(
    builder.Configuration.GetSection("MongoDB"));

// Contexto MongoDB (singleton: un MongoClient por ciclo de vida de la app)
builder.Services.AddSingleton<ContextoMongoDb>();

// Servicios de dominio
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<EventsService>();
builder.Services.AddScoped<EventRulesService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<ReactionService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<AuditLogService>();

// Autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";
        options.LogoutPath = "/Users/Logout";
        options.AccessDeniedPath = "/Users/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userService = scope.ServiceProvider.GetRequiredService<UserService>();
    var profileService = scope.ServiceProvider.GetRequiredService<ProfileService>();

    await SembrarUsuarioDemoAsync(
        userService,
        profileService,
        username: "organizador_demo",
        email: "organizador@checkpoint.local",
        role: "Organizer",
        passwordPlano: "CheckPoint123!");

    await SembrarUsuarioDemoAsync(
        userService,
        profileService,
        username: "usuario_demo",
        email: "usuario@checkpoint.local",
        role: "User",
        passwordPlano: "CheckPoint123!");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static async Task SembrarUsuarioDemoAsync(
    UserService userService,
    ProfileService profileService,
    string username,
    string email,
    string role,
    string passwordPlano)
{
    var existingUser = await userService.GetByEmailAsync(email);

    if (existingUser == null)
    {
        existingUser = new User
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Username = username,
            Email = email,
            PasswordHash = BCryptNet.HashPassword(passwordPlano),
            Role = role,
            IsActive = true
        };

        await userService.CreateAsync(existingUser);
    }

    var profile = await profileService.GetByUserIdAsync(existingUser.Id);

    if (profile == null)
    {
        await profileService.CreateAsync(new Profile
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            UserId = existingUser.Id,
            DisplayName = existingUser.Username,
            FavoriteGameIds = new List<string>()
        });
    }
}