using CheckPoint.Services;
using CheckPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.Controllers
{
    [Authorize]
    public class PerfilesController : Controller
    {
        private readonly PerfilService _profileService;
        private readonly UsuarioService _userService;

        public PerfilesController(PerfilService profileService, UsuarioService userService)
        {
            _profileService = profileService;
            _userService = userService;
        }

        // Muestra el perfil publico de un usuario.
        [AllowAnonymous]
        public async Task<IActionResult> Index(string userId)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound();

            var profile = await _profileService.GetByUserIdAsync(userId);
            ViewBag.Usuario = user;
            return View(profile);
        }

        // Muestra el formulario para editar el perfil propio.
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null) return NotFound();

            var vm = new EditarPerfilViewModel
            {
                Id = profile.Id,
                DisplayName = profile.DisplayName,
                GamerTag = profile.GamerTag,
                Bio = profile.Bio,
                Country = profile.Country,
                FavoriteGamesRaw = string.Join(", ", profile.FavoriteGames),
                Twitch = profile.SocialLinks.GetValueOrDefault("Twitch", ""),
                YouTube = profile.SocialLinks.GetValueOrDefault("YouTube", ""),
                Twitter = profile.SocialLinks.GetValueOrDefault("Twitter", "")
            };

            return View(vm);
        }

        // Actualiza los datos del perfil propio.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditarPerfilViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null) return NotFound();

            profile.DisplayName = model.DisplayName;
            profile.GamerTag = model.GamerTag;
            profile.Bio = model.Bio;
            profile.Country = model.Country;
            profile.FavoriteGames = string.IsNullOrWhiteSpace(model.FavoriteGamesRaw)
                ? new()
                : model.FavoriteGamesRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                        .ToList();

            var socialLinks = new Dictionary<string, string>();
            if (!string.IsNullOrWhiteSpace(model.Twitch)) socialLinks["Twitch"] = model.Twitch;
            if (!string.IsNullOrWhiteSpace(model.YouTube)) socialLinks["YouTube"] = model.YouTube;
            if (!string.IsNullOrWhiteSpace(model.Twitter)) socialLinks["Twitter"] = model.Twitter;
            profile.SocialLinks = socialLinks;

            await _profileService.UpdateAsync(profile.Id, profile);

            TempData["Success"] = "Perfil actualizado correctamente.";
            return RedirectToAction(nameof(Index), new { userId });
        }
    }
}


