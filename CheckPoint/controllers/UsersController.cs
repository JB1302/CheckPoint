using CheckPoint.Models.Users;
using CheckPoint.Services;
using CheckPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BCryptNet = BCrypt.Net.BCrypt;

namespace CheckPoint.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserService _userService;
        private readonly AuditLogService _auditLogService;

        public UsersController(
            UserService userService,
            AuditLogService auditLogService)
        {
            _userService = userService;
            _auditLogService = auditLogService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Debe ingresar correo y contraseña.";
                return View();
            }

            var user = await _userService.GetByEmailAsync(email);

            if (user == null)
            {
                ViewBag.Error = "Usuario no encontrado.";
                return View();
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "La cuenta está inactiva.";
                return View();
            }

            if (!BCryptNet.Verify(password, user.PasswordHash))
            {
                ViewBag.Error = "Contraseña incorrecta.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            await _auditLogService.LogAsync(
                user.Id,
                "Login",
                "User",
                user.Id);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _auditLogService.LogAsync(
                    userId,
                    "Logout",
                    "User",
                    userId);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> MyAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound();

            return View("Details", user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new User
            {
                IsActive = true
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User model, string password)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _userService.ExistsByEmailAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            if (await _userService.ExistsByUsernameAsync(model.Username))
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Password is required.");
                return View(model);
            }

            model.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            model.PasswordHash = BCryptNet.HashPassword(password);
            model.CreatedAt = DateTime.UtcNow;

            await _userService.CreateAsync(model);

            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(adminUserId))
            {
                await _auditLogService.LogAsync(
                    adminUserId,
                    "CreateUser",
                    "User",
                    model.Id);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, User model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var existing = await _userService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var emailOwner = await _userService.GetByEmailAsync(model.Email);
            if (emailOwner != null && emailOwner.Id != model.Id)
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(model);
            }

            var usernameOwner = await _userService.GetByUsernameAsync(model.Username);
            if (usernameOwner != null && usernameOwner.Id != model.Id)
            {
                ModelState.AddModelError("Username", "Username already exists.");
                return View(model);
            }

            model.CreatedAt = existing.CreatedAt;
            model.PasswordHash = existing.PasswordHash;

            await _userService.UpdateAsync(id, model);

            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(adminUserId))
            {
                await _auditLogService.LogAsync(
                    adminUserId,
                    "UpdateUser",
                    "User",
                    model.Id);
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            user.IsActive = !user.IsActive;

            await _userService.UpdateAsync(id, user);

            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(adminUserId))
            {
                await _auditLogService.LogAsync(
                    adminUserId,
                    "ToggleUserActive",
                    "User",
                    id);
            }

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegistroViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (await _userService.ExistsByEmailAsync(model.Email))
            {
                ModelState.AddModelError("Email", "El correo ya está registrado.");
                return View(model);
            }

            if (await _userService.ExistsByUsernameAsync(model.Username))
            {
                ModelState.AddModelError("Username", "El nombre de usuario ya existe.");
                return View(model);
            }

            var user = new User
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                Username = model.Username,
                Email = model.Email,
                PasswordHash = BCryptNet.HashPassword(model.Password),
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userService.CreateAsync(user);

            await _auditLogService.LogAsync(
                user.Id,
                "Register",
                "User",
                user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
        }
    }
}