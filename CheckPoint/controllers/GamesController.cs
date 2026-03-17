using CheckPoint.Models.Games;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.controllers
{
    public class GamesController : Controller
    {
        private readonly GameService _gameService;
        private readonly AuditLogService _auditLogService;

        public GamesController(
            GameService gameService,
            AuditLogService auditLogService)
        {
            _gameService = gameService;
            _auditLogService = auditLogService;
        }

        // Show active games
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var games = await _gameService.GetAllActiveAsync();
            return View(games);
        }

        // Show game details
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var game = await _gameService.GetByIdAsync(id);
            if (game == null)
                return NotFound();

            return View(game);
        }

        // Show all games for admin
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var games = await _gameService.GetAllAsync();
            return View(games);
        }

        // Show create game form
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Game());
        }

        // Save new game
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Game model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            await _gameService.CreateAsync(model);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _auditLogService.LogAsync(
                    userId,
                    "CreateGame",
                    "Game",
                    model.Id);
            }

            return RedirectToAction(nameof(Manage));
        }

        // Show edit game form
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var game = await _gameService.GetByIdAsync(id);
            if (game == null)
                return NotFound();

            return View(game);
        }

        // Save game changes
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Game model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var existing = await _gameService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            await _gameService.UpdateAsync(id, model);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _auditLogService.LogAsync(
                    userId,
                    "UpdateGame",
                    "Game",
                    model.Id);
            }

            return RedirectToAction(nameof(Manage));
        }

        // Delete a game
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var game = await _gameService.GetByIdAsync(id);
            if (game == null)
                return NotFound();

            await _gameService.DeleteAsync(id);

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await _auditLogService.LogAsync(
                    userId,
                    "DeleteGame",
                    "Game",
                    id);
            }

            return RedirectToAction(nameof(Manage));
        }
    }
}