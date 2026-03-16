using CheckPoint.Models;
using CheckPoint.Services;
using CheckPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.Controllers
{
    [Authorize]
    public class InscripcionesController : Controller
    {
        private readonly InscripcionService _registrationService;
        private readonly EventoService _eventService;
        private readonly NotificacionService _notificationService;
        private readonly BitacoraAuditoriaService _auditLogService;

        public InscripcionesController(
            InscripcionService registrationService,
            EventoService eventService,
            NotificacionService notificationService,
            BitacoraAuditoriaService auditLogService)
        {
            _registrationService = registrationService;
            _eventService = eventService;
            _notificationService = notificationService;
            _auditLogService = auditLogService;
        }

        // Inscribe al usuario en un evento.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(string eventId)
        {
            var ev = await _eventService.GetByIdAsync(eventId);
            if (ev == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (await _registrationService.IsRegisteredAsync(eventId, userId))
                return RedirectToAction("Details", "Eventos", new { id = eventId });

            if (ev.Status != "Open" || ev.CurrentParticipants >= ev.MaxParticipants
                || DateTime.UtcNow > ev.RegistrationDeadline)
            {
                TempData["Error"] = "No es posible inscribirse en este evento.";
                return RedirectToAction("Details", "Eventos", new { id = eventId });
            }

            var registration = new Inscripcion
            {
                EventId = eventId,
                UserId = userId,
                Status = "Confirmed"
            };

            await _registrationService.CreateAsync(registration);
            await _eventService.IncrementParticipantsAsync(eventId);

            await _notificationService.CreateAsync(new Notificacion
            {
                UserId = userId,
                Title = "Inscripción confirmada",
                Message = $"Te has inscrito al evento: {ev.Title}",
                Type = "RegistrationConfirmed",
                ReferenceId = eventId,
                ReferenceType = "Evento"
            });

            await _auditLogService.LogAsync(userId, "Register", "Inscripcion", registration.Id,
                newValues: new() { ["eventId"] = eventId },
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

            TempData["Success"] = "¡Inscripción confirmada!";
            return RedirectToAction("Details", "Eventos", new { id = eventId });
        }

        // Cancela la inscripcion del usuario en un evento.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var registration = await _registrationService.GetByEventAndUserAsync(eventId, userId);

            if (registration == null)
                return RedirectToAction("Details", "Eventos", new { id = eventId });

            await _registrationService.UpdateStatusAsync(registration.Id, "Cancelled");
            await _eventService.DecrementParticipantsAsync(eventId);

            await _auditLogService.LogAsync(userId, "CancelRegistration", "Inscripcion", registration.Id,
                oldValues: new() { ["status"] = "Confirmed" },
                newValues: new() { ["status"] = "Cancelled" },
                ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");

            TempData["Info"] = "Inscripción cancelada.";
            return RedirectToAction("Details", "Eventos", new { id = eventId });
        }

        // Lista los eventos en los que el usuario esta inscrito.
        public async Task<IActionResult> MyEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var registrations = await _registrationService.GetByUserIdAsync(userId);

            var eventIds = registrations.Select(r => r.EventId).Distinct().ToList();
            var events = new List<Models.Evento>();
            foreach (var eid in eventIds)
            {
                var ev = await _eventService.GetByIdAsync(eid);
                if (ev != null) events.Add(ev);
            }

            ViewBag.Registrations = registrations;
            return View(events);
        }
    }
}


