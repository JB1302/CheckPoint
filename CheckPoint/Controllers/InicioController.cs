using CheckPoint.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    public class InicioController : Controller
    {
        private readonly EventoService _eventService;
        private readonly JuegoService _gameService;

        public InicioController(EventoService eventService, JuegoService gameService)
        {
            _eventService = eventService;
            _gameService = gameService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllPublicAsync();
            var games = await _gameService.GetAllActiveAsync();
            ViewBag.Games = games;
            return View(events);
        }

        public IActionResult Error() => View();
    }
}

