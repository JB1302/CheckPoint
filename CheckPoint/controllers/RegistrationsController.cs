using CheckPoint.Models.Registrations;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.controllers
{
    [Authorize]
    public class RegistrationsController : Controller
    {
        private readonly RegistrationService _registrationService;
        private readonly EventsService _eventsService;
        private readonly NotificationService _notificationService;
        private readonly AuditLogService _auditLogService;

        public RegistrationsController(
            RegistrationService registrationService,
            EventsService eventsService,
            NotificationService notificationService,
            AuditLogService auditLogService)
        {
            _registrationService = registrationService;
            _eventsService = eventsService;
            _notificationService = notificationService;
            _auditLogService = auditLogService;
        }

        // Show registrations of current user
        [HttpGet]
        public async Task<IActionResult> MyRegistrations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var registrations = await _registrationService.GetByUserIdAsync(userId);
            return View(registrations);
        }

        // Show registrations of a specific event
        [HttpGet]
        public async Task<IActionResult> ByEvent(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest();

            var ev = await _eventsService.GetByIdAsync(eventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var registrations = await _registrationService.GetByEventIdAsync(eventId);

            ViewBag.EventId = eventId;
            ViewBag.EventTitle = ev.Title;

            return View(registrations);
        }

        // Join an event
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest();

            var ev = await _eventsService.GetByIdAsync(eventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (ev.Status != "Open")
                return RedirectToAction("Details", "Events", new { id = eventId });

            var alreadyRegistered = await _registrationService.IsRegisteredAsync(eventId, userId);
            if (alreadyRegistered)
                return RedirectToAction("Details", "Events", new { id = eventId });

            var confirmedCount = await _registrationService.CountConfirmedByEventIdAsync(eventId);
            if (confirmedCount >= ev.MaxParticipants)
                return RedirectToAction("Details", "Events", new { id = eventId });

            var registration = new Registration
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                EventId = eventId,
                UserId = userId,
                Status = "Confirmed"
            };

            await _registrationService.CreateAsync(registration);

            if (!string.IsNullOrWhiteSpace(ev.OrganizerId))
            {
                await _notificationService.CreateAsync(new CheckPoint.Models.Notifications.Notification
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    UserId = ev.OrganizerId,
                    Type = "RegistrationConfirmed",
                    ReferenceId = eventId,
                    Message = "A new participant has joined your event."
                });
            }

            await _auditLogService.LogAsync(
                userId,
                "JoinEvent",
                "Registration",
                registration.Id);

            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        // Cancel current user's registration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest();

            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var registration = await _registrationService.GetByEventAndUserAsync(eventId, userId);
            if (registration == null)
                return NotFound();

            await _registrationService.UpdateStatusAsync(registration.Id, "Cancelled");

            await _auditLogService.LogAsync(
                userId,
                "CancelRegistration",
                "Registration",
                registration.Id);

            return RedirectToAction(nameof(MyRegistrations));
        }

        // Change registration status (organizer/admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string id, string status)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(status))
                return BadRequest();

            var registration = await _registrationService.GetByIdAsync(id);
            if (registration == null)
                return NotFound();

            var ev = await _eventsService.GetByIdAsync(registration.EventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _registrationService.UpdateStatusAsync(id, status);

            await _notificationService.CreateAsync(new CheckPoint.Models.Notifications.Notification
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                UserId = registration.UserId,
                Type = "RegistrationUpdated",
                ReferenceId = registration.EventId,
                Message = $"Your registration status was changed to {status}."
            });

            await _auditLogService.LogAsync(
                userId,
                "UpdateRegistrationStatus",
                "Registration",
                id);

            return RedirectToAction("ByEvent", new { eventId = registration.EventId });
        }
    }
}