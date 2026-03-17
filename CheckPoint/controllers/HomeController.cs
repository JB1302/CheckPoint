using Microsoft.AspNetCore.Mvc;
using CheckPoint.Services;

namespace CheckPoint.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventsService _eventsService;
        private readonly GameService _gameService;

        public HomeController(EventsService eventsService, GameService gameService)
        {
            _eventsService = eventsService;
            _gameService = gameService;
        }

        public async Task<IActionResult> Index()
        {
            var eventsList = await _eventsService.GetAllPublicAsync();
            var games = await _gameService.GetAllAsync();

            ViewBag.Games = games;
            return View(eventsList);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }
    }
}