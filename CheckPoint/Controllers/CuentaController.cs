using BCryptNet = BCrypt.Net.BCrypt;
using CheckPoint.Models;
using CheckPoint.Services;
using CheckPoint.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.Controllers
{
    public class CuentaController : Controller
    {
        private readonly UsuarioService _userService;
        private readonly PerfilService _profileService;

        public CuentaController(UsuarioService userService, PerfilService profileService)
        {
            _userService = userService;
            _profileService = profileService;
        }

        // Muestra el formulario de inicio de sesion.
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Inicio");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // Procesa el inicio de sesion.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(IniciarSesionViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userService.GetByEmailAsync(model.Email);
            if (user == null || !user.IsActive || !BCryptNet.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(model);
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _userService.UpdateAsync(user.Id, user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Inicio");
        }

        // Muestra el formulario de registro.
        [HttpGet]
        public IActionResult Register() => View();

        // Procesa el registro de usuario.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _userService.ExistsByEmailAsync(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Ya existe una cuenta con ese correo.");
                return View(model);
            }

            if (await _userService.ExistsByUsernameAsync(model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "El nombre de usuario ya está en uso.");
                return View(model);
            }

            // Solo se permite autoasignar los roles Usuario y Organizer.
            var role = model.Role == "Organizer" ? "Organizer" : "Usuario";

            var user = new Usuario
            {
                Username = model.Username,
                Email = model.Email.ToLowerInvariant(),
                PasswordHash = BCryptNet.HashPassword(model.Password),
                Role = role
            };

            await _userService.CreateAsync(user);

            // Crear un perfil por defecto
            await _profileService.CreateAsync(new Perfil
            {
                UserId = user.Id,
                DisplayName = user.Username
            });

            return RedirectToAction(nameof(Login));
        }

        // Cierra la sesion del usuario autenticado.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Inicio");
        }

        public IActionResult AccessDenied() => View();
    }
}



