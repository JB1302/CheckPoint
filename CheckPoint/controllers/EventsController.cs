using CheckPoint.Models.Events;
using CheckPoint.Services;
using CheckPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CheckPoint.controllers
{
    public class EventsController : Controller
    {
        private readonly EventsService _eventsService;
        private readonly GameService _gameService;
        private readonly EventRulesService _eventRulesService;
        private readonly PostService _postService;
        private readonly RegistrationService _registrationService;
        private readonly AuditLogService _auditLogService;

        public EventsController(
            EventsService eventsService,
            GameService gameService,
            EventRulesService eventRulesService,
            PostService postService,
            RegistrationService registrationService,
            AuditLogService auditLogService)
        {
            _eventsService = eventsService;
            _gameService = gameService;
            _eventRulesService = eventRulesService;
            _postService = postService;
            _registrationService = registrationService;
            _auditLogService = auditLogService;
        }

        // Show public events with filters
        [HttpGet]
        public async Task<IActionResult> Index(string? game, string? type, string? q)
        {
            var eventsList = await _eventsService.SearchAsync(game, type, q);
            var games = await _gameService.GetAllActiveAsync();

            ViewBag.Games = games;
            ViewBag.SelectedGame = game;
            ViewBag.SelectedType = type;
            ViewBag.Query = q;

            return View(eventsList);
        }

        // Show event details with related data
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var ev = await _eventsService.GetByIdAsync(id);
            if (ev == null)
                return NotFound();

            var game = await _gameService.GetByIdAsync(ev.GameId);
            var rules = await _eventRulesService.GetByEventIdAsync(ev.Id);
            var posts = await _postService.GetByEventIdAsync(ev.Id);
            var registrations = await _registrationService.GetByEventIdAsync(ev.Id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var isRegistered = false;
            var isOrganizer = false;
            var canRegister = false;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                isRegistered = await _registrationService.IsRegisteredAsync(ev.Id, userId);
                isOrganizer = ev.OrganizerId == userId;
            }

            canRegister =
                User.Identity?.IsAuthenticated == true &&
                !isRegistered &&
                ev.Status == "Open";

            var vm = new EventDetailViewModel
            {
                Event = ev,
                Game = game,
                EventRules = rules,
                Posts = posts,
                Registrations = registrations,
                IsRegistered = isRegistered,
                CanRegister = canRegister,
                IsOrganizer = isOrganizer
            };

            return View(vm);
        }

        // Show events created by current user
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var myEvents = await _eventsService.GetByOrganizerIdAsync(userId);
            return View(myEvents);
        }

        // Show create event form
        [Authorize(Roles = "Organizer,Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var games = await _gameService.GetAllActiveAsync();

            var vm = new CreateEventViewModel
            {
                GameOptions = games.Select(g => new SelectListItem
                {
                    Value = g.Id,
                    Text = g.Name
                }).ToList()
            };

            return View(vm);
        }

        // Save new event
        [Authorize(Roles = "Organizer,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var gamesReload = await _gameService.GetAllActiveAsync();
                model.GameOptions = gamesReload.Select(g => new SelectListItem
                {
                    Value = g.Id,
                    Text = g.Name
                }).ToList();

                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var location = string.IsNullOrWhiteSpace(model.VenueName)
                ? null
                : $"{model.VenueName} - {model.Address} - {model.City} - {model.Country}";

            var ev = new Events
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                Title = model.Title,
                Description = model.Description,
                GameId = model.GameId,
                EventType = model.EventType,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Location = location,
                OnlineLink = model.StreamUrl,
                MaxParticipants = model.MaxParticipants,
                OrganizerId = userId,
                Status = "Open"
            };

            await _eventsService.CreateAsync(ev);

            await _auditLogService.LogAsync(
                userId,
                "CreateEvent",
                "Event",
                ev.Id);

            return RedirectToAction(nameof(Details), new { id = ev.Id });
        }

        // Show edit event form
        [Authorize(Roles = "Organizer,Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var ev = await _eventsService.GetByIdAsync(id);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var games = await _gameService.GetAllActiveAsync();

            var vm = new EditEventViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                GameId = ev.GameId,
                EventType = ev.EventType,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                MaxParticipants = ev.MaxParticipants,
                StreamUrl = ev.OnlineLink ?? string.Empty,
                GameOptions = games.Select(g => new SelectListItem
                {
                    Value = g.Id,
                    Text = g.Name
                }).ToList()
            };

            return View(vm);
        }

        // Save event changes
        [Authorize(Roles = "Organizer,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditEventViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            var existing = await _eventsService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existing.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            if (!ModelState.IsValid)
            {
                var gamesReload = await _gameService.GetAllActiveAsync();
                model.GameOptions = gamesReload.Select(g => new SelectListItem
                {
                    Value = g.Id,
                    Text = g.Name
                }).ToList();

                return View(model);
            }

            var location = string.IsNullOrWhiteSpace(model.VenueName)
                ? existing.Location
                : $"{model.VenueName} - {model.Address} - {model.City} - {model.Country}";

            existing.Title = model.Title;
            existing.Description = model.Description;
            existing.GameId = model.GameId;
            existing.EventType = model.EventType;
            existing.StartDate = model.StartDate;
            existing.EndDate = model.EndDate;
            existing.MaxParticipants = model.MaxParticipants;
            existing.OnlineLink = model.StreamUrl;
            existing.Location = location;

            await _eventsService.UpdateAsync(id, existing);

            await _auditLogService.LogAsync(
                userId!,
                "UpdateEvent",
                "Event",
                existing.Id);

            return RedirectToAction(nameof(Details), new { id = existing.Id });
        }

        // Cancel an event
        [Authorize(Roles = "Organizer,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var ev = await _eventsService.GetByIdAsync(id);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _eventsService.CancelAsync(id);

            await _auditLogService.LogAsync(
                userId!,
                "CancelEvent",
                "Event",
                id);

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}