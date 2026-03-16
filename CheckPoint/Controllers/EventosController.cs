using CheckPoint.Models;
using CheckPoint.Services;
using CheckPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CheckPoint.Controllers
{
    public class EventosController : Controller
    {
        private readonly EventoService _eventService;
        private readonly ReglaEventoService _eventRuleService;
        private readonly JuegoService _gameService;
        private readonly InscripcionService _registrationService;
        private readonly PublicacionService _postService;
        private readonly BitacoraAuditoriaService _auditLogService;

        public EventosController(
            EventoService eventService,
            ReglaEventoService eventRuleService,
            JuegoService gameService,
            InscripcionService registrationService,
            PublicacionService postService,
            BitacoraAuditoriaService auditLogService)
        {
            _eventService = eventService;
            _eventRuleService = eventRuleService;
            _gameService = gameService;
            _registrationService = registrationService;
            _postService = postService;
            _auditLogService = auditLogService;
        }

        // Lista y filtra eventos.
        public async Task<IActionResult> Index(string? game, string? type, string? q)
        {
            var events = await _eventService.SearchAsync(game, type, q);
            var games = await _gameService.GetAllActiveAsync();
            ViewBag.Games = games;
            ViewBag.SelectedGame = game;
            ViewBag.SelectedType = type;
            ViewBag.Query = q;
            return View(events);
        }

        // Muestra el detalle de un evento.
        public async Task<IActionResult> Details(string id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vm = new DetalleEventoViewModel
            {
                Evento = ev,
                Juego = await _gameService.GetByIdAsync(ev.GameId),
                Rules = await _eventRuleService.GetByEventIdAsync(id),
                Posts = await _postService.GetByEventIdAsync(id),
                Registrations = await _registrationService.GetByEventIdAsync(id),
                IsOrganizer = userId == ev.OrganizerId,
                IsRegistered = userId != null && await _registrationService.IsRegisteredAsync(id, userId),
                CanRegister = userId != null
                              && ev.Status == "Open"
                              && ev.CurrentParticipants < ev.MaxParticipants
                              && DateTime.UtcNow < ev.RegistrationDeadline
            };

            return View(vm);
        }

        // Muestra el formulario para crear evento.
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Create()
        {
            var vm = new CrearEventoViewModel
            {
                GameOptions = await BuildGameSelectListAsync()
            };
            return View(vm);
        }

        // Crea un nuevo evento.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Create(CrearEventoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.GameOptions = await BuildGameSelectListAsync();
                return View(model);
            }

            var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var ev = MapViewModelToEvent(model, organizerId);
            await _eventService.CreateAsync(ev);

            await _auditLogService.LogAsync(organizerId, "CreateEvent", "Evento", ev.Id,
                newValues: new() { ["title"] = ev.Title },
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

            return RedirectToAction(nameof(Details), new { id = ev.Id });
        }

        // Muestra el formulario para editar evento.
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var vm = MapEventToViewModel(ev);
            vm.GameOptions = await BuildGameSelectListAsync();
            return View(vm);
        }

        // Actualiza los datos de un evento.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Edit(string id, EditarEventoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.GameOptions = await BuildGameSelectListAsync();
                return View(model);
            }

            var existing = await _eventService.GetByIdAsync(id);
            if (existing == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existing.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var updated = MapViewModelToEvent(model, existing.OrganizerId);
            updated.Id = id;
            updated.CurrentParticipants = existing.CurrentParticipants;
            updated.CreatedAt = existing.CreatedAt;

            await _eventService.UpdateAsync(id, updated);

            await _auditLogService.LogAsync(userId!, "EditEvent", "Evento", id,
                oldValues: new() { ["title"] = existing.Title },
                newValues: new() { ["title"] = updated.Title },
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

            return RedirectToAction(nameof(Details), new { id });
        }

        // Cancela un evento.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> Cancel(string id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            ev.Status = "Cancelled";
            await _eventService.UpdateAsync(id, ev);

            await _auditLogService.LogAsync(userId!, "CancelEvent", "Evento", id,
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

            return RedirectToAction(nameof(Details), new { id });
        }

        // Utilidades internas

        private async Task<List<SelectListItem>> BuildGameSelectListAsync()
        {
            var games = await _gameService.GetAllActiveAsync();
            return games.Select(g => new SelectListItem(g.Name, g.Id)).ToList();
        }

        private static Evento MapViewModelToEvent(CrearEventoViewModel m, string organizerId) => new()
        {
            OrganizerId = organizerId,
            Title = m.Title,
            Description = m.Description,
            GameId = m.GameId,
            Type = m.Type,
            Format = m.Format,
            MaxParticipants = m.MaxParticipants,
            StartDate = m.StartDate,
            EndDate = m.EndDate,
            RegistrationDeadline = m.RegistrationDeadline,
            StreamUrl = m.StreamUrl,
            PrizePool = m.PrizePool,
            Tags = string.IsNullOrWhiteSpace(m.TagsRaw)
                ? new()
                : m.TagsRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
            Location = m.Type != "Online" ? new UbicacionEvento
            {
                VenueName = m.VenueName,
                Address = m.Address,
                City = m.City,
                Country = m.Country
            } : null,
            Status = "Open"
        };

        private static EditarEventoViewModel MapEventToViewModel(Evento ev) => new()
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            GameId = ev.GameId,
            Type = ev.Type,
            Format = ev.Format,
            MaxParticipants = ev.MaxParticipants,
            StartDate = ev.StartDate,
            EndDate = ev.EndDate,
            RegistrationDeadline = ev.RegistrationDeadline,
            StreamUrl = ev.StreamUrl,
            PrizePool = ev.PrizePool,
            TagsRaw = string.Join(", ", ev.Tags),
            VenueName = ev.Location?.VenueName ?? string.Empty,
            Address = ev.Location?.Address ?? string.Empty,
            City = ev.Location?.City ?? string.Empty,
            Country = ev.Location?.Country ?? string.Empty
        };
    }
}



