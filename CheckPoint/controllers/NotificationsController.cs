using CheckPoint.Models.Notifications;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<CheckPoint.Hubs.NotificationsHub> _hubContext;

        public NotificationsController(NotificationService notificationService,
            Microsoft.AspNetCore.SignalR.IHubContext<CheckPoint.Hubs.NotificationsHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        // Show current user's notifications
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var notifications = await _notificationService.GetByUserIdAsync(userId);
            ViewBag.UnreadCount = await _notificationService.CountUnreadAsync(userId);

            return View(notifications);
        }

        // Show unread count for current user
        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var count = await _notificationService.CountUnreadAsync(userId);
            return Json(new { unreadCount = count });
        }

        // Partial used by AJAX to refresh the badge
        [HttpGet]
        public async Task<IActionResult> Badge()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Content(string.Empty);

            var recent = await _notificationService.GetByUserIdAsync(userId);
            var vm = recent.Take(5).Select(n => new CheckPoint.ViewModels.NotificationViewModel
            {
                Id = n.Id,
                Title = n.Type,
                Message = n.Message,
                CreatedAt = n.CreatedAt,
                IsRead = n.IsRead
            }).ToList();

            ViewBag.UnreadNotificationsCount = await _notificationService.CountUnreadAsync(userId);
            ViewBag.RecentNotifications = vm;

            return PartialView("~/Views/Notifications/_NotificationBadge.cshtml");
        }

        // Mark one notification as read
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var notification = await _notificationService.GetByIdAsync(id);
            if (notification == null)
                return NotFound();

            if (notification.UserId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _notificationService.MarkAsReadAsync(id);

            return RedirectToAction(nameof(Index));
        }

        // Mark all current user's notifications as read
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            await _notificationService.MarkAllReadAsync(userId);

            return RedirectToAction(nameof(Index));
        }

        // Optional test/manual creation
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Notification());
        }

        // Save notification manually
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notification model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            await _notificationService.CreateAsync(model);

            return RedirectToAction(nameof(Index));
        }
    }
}