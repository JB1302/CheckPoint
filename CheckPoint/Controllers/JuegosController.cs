using CheckPoint.Models;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    [Authorize(Roles = "Admin")]
    public class JuegosController : Controller
    {
        private readonly JuegoService _gameService;

        public JuegosController(JuegoService gameService) => _gameService = gameService;

        // Lista los juegos activos.
        public async Task<IActionResult> Index()
        {
            var games = await _gameService.GetAllActiveAsync();
            return View(games);
        }

        // Muestra el formulario para crear un juego.
        public IActionResult Create() => View(new Juego());

        // Crea un nuevo juego.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Juego game)
        {
            if (!ModelState.IsValid) return View(game);
            await _gameService.CreateAsync(game);
            return RedirectToAction(nameof(Index));
        }

        // Muestra el formulario para editar un juego.
        public async Task<IActionResult> Edit(string id)
        {
            var game = await _gameService.GetByIdAsync(id);
            if (game == null) return NotFound();
            return View(game);
        }

        // Actualiza la informacion de un juego.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Juego game)
        {
            if (!ModelState.IsValid) return View(game);
            await _gameService.UpdateAsync(id, game);
            return RedirectToAction(nameof(Index));
        }
    }
}

