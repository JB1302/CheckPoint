using CheckPoint.Services;
using CheckPoint.Models;
using BCryptNet = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configuracion de MongoDB
builder.Services.Configure<ConfiguracionMongoDb>(
    builder.Configuration.GetSection("MongoDB"));

// Contexto MongoDB \(singleton: un MongoClient por ciclo de vida de la app\)
builder.Services.AddSingleton<ContextoMongoDb>();

// Servicios de dominio
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<PerfilService>();
builder.Services.AddScoped<JuegoService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<ReglaEventoService>();
builder.Services.AddScoped<InscripcionService>();
builder.Services.AddScoped<PublicacionService>();
builder.Services.AddScoped<ComentarioService>();
builder.Services.AddScoped<ReaccionService>();
builder.Services.AddScoped<NotificacionService>();
builder.Services.AddScoped<ReporteService>();
builder.Services.AddScoped<BitacoraAuditoriaService>();

// Autenticacion por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.LogoutPath = "/Cuenta/Logout";
        options.AccessDeniedPath = "/Cuenta/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var usuarioService = scope.ServiceProvider.GetRequiredService<UsuarioService>();
    var perfilService = scope.ServiceProvider.GetRequiredService<PerfilService>();

    await SembrarUsuarioDemoAsync(
        usuarioService,
        perfilService,
        username: "organizador_demo",
        email: "organizador@checkpoint.local",
        role: "Organizer",
        passwordPlano: "CheckPoint123!");

    await SembrarUsuarioDemoAsync(
        usuarioService,
        perfilService,
        username: "usuario_demo",
        email: "usuario@checkpoint.local",
        role: "Usuario",
        passwordPlano: "CheckPoint123!");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Inicio/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Inicio}/{action=Index}/{id?}");

app.Run();

static async Task SembrarUsuarioDemoAsync(
    UsuarioService usuarioService,
    PerfilService perfilService,
    string username,
    string email,
    string role,
    string passwordPlano)
{
    var existente = await usuarioService.GetByEmailAsync(email);
    if (existente == null)
    {
        existente = new Usuario
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            Username = username,
            Email = email,
            PasswordHash = BCryptNet.HashPassword(passwordPlano),
            Role = role,
            IsActive = true
        };

        await usuarioService.CreateAsync(existente);
    }

    var perfil = await perfilService.GetByUserIdAsync(existente.Id);
    if (perfil == null)
    {
        await perfilService.CreateAsync(new Perfil
        {
            Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
            UserId = existente.Id,
            DisplayName = existente.Username
        });
    }
}


