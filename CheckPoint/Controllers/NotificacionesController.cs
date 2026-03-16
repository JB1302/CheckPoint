using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.Controllers
{
    [Authorize]
    public class NotificacionesController : Controller
    {
        private readonly NotificacionService _notificationService;

        public NotificacionesController(NotificacionService notificationService)
            => _notificationService = notificationService;

        // Lista las notificaciones del usuario.
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var notifications = await _notificationService.GetByUserIdAsync(userId);
            return View(notifications);
        }

        // Marca una notificacion como leida.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(string id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Marca todas las notificaciones como leidas.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _notificationService.MarkAllReadAsync(userId);
            return RedirectToAction(nameof(Index));
        }
    }
}


