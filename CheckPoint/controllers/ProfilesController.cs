using CheckPoint.Models.Profiles;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.controllers
{
    public class ProfilesController : Controller
    {
        private readonly ProfileService _profileService;
        private readonly GameService _gameService;
        private readonly AuditLogService _auditLogService;

        public ProfilesController(
            ProfileService profileService,
            GameService gameService,
            AuditLogService auditLogService)
        {
            _profileService = profileService;
            _gameService = gameService;
            _auditLogService = auditLogService;
        }

        // Show profile by profile id
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var profile = await _profileService.GetByIdAsync(id);
            if (profile == null)
                return NotFound();

            ViewBag.Games = await _gameService.GetAllActiveAsync();

            return View(profile);
        }

        // Show current user's profile
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var profile = await _profileService.GetByUserIdAsync(userId);
            if (profile == null)
                return RedirectToAction(nameof(Create));

            return RedirectToAction(nameof(Details), new { id = profile.Id });
        }

        // Show create profile form
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var existingProfile = await _profileService.GetByUserIdAsync(userId);
            if (existingProfile != null)
                return RedirectToAction(nameof(Edit), new { id = existingProfile.Id });

            ViewBag.Games = await _gameService.GetAllActiveAsync();

            return View(new Profile
            {
                UserId = userId
            });
        }

        // Save new profile
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Profile model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.Games = await _gameService.GetAllActiveAsync();
                return View(model);
            }

            var existingProfile = await _profileService.GetByUserIdAsync(userId);
            if (existingProfile != null)
                return RedirectToAction(nameof(Edit), new { id = existingProfile.Id });

            model.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            model.UserId = userId;

            await _profileService.CreateAsync(model);

            await _auditLogService.LogAsync(
                userId,
                "CreateProfile",
                "Profile",
                model.Id);

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // Show edit profile form
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var profile = await _profileService.GetByIdAsync(id);
            if (profile == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (profile.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            ViewBag.Games = await _gameService.GetAllActiveAsync();

            return View(profile);
        }

        // Save profile changes
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Profile model)
        {
            if (id != model.Id)
                return BadRequest();

            var existing = await _profileService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (existing.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.Games = await _gameService.GetAllActiveAsync();
                return View(model);
            }

            model.UserId = existing.UserId;

            await _profileService.UpdateAsync(id, model);

            await _auditLogService.LogAsync(
                userId,
                "UpdateProfile",
                "Profile",
                model.Id);

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
    }
}