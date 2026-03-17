using CheckPoint.Models.Reports;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ReportService _reportService;
        private readonly NotificationService _notificationService;
        private readonly AuditLogService _auditLogService;

        public ReportsController(
            ReportService reportService,
            NotificationService notificationService,
            AuditLogService auditLogService)
        {
            _reportService = reportService;
            _notificationService = notificationService;
            _auditLogService = auditLogService;
        }

        // Show all reports for admin
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reports = await _reportService.GetAllAsync();
            return View(reports);
        }

        // Show pending reports for admin
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var reports = await _reportService.GetPendingAsync();
            return View("Index", reports);
        }

        // Show report details
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var report = await _reportService.GetByIdAsync(id);
            if (report == null)
                return NotFound();

            return View(report);
        }

        // Show create report form
        [HttpGet]
        public IActionResult Create(string? targetType = null, string? targetId = null)
        {
            var model = new Report
            {
                TargetType = targetType ?? string.Empty,
                TargetId = targetId ?? string.Empty,
                Status = "Pending"
            };

            return View(model);
        }

        // Save new report
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Report model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            model.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            model.ReporterId = userId;
            model.Status = string.IsNullOrWhiteSpace(model.Status) ? "Pending" : model.Status;

            await _reportService.CreateAsync(model);

            await _auditLogService.LogAsync(
                userId,
                "CreateReport",
                "Report",
                model.Id);

            return RedirectToAction("Details", new { id = model.Id });
        }

        // Update report status
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string id, string status)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(status))
                return BadRequest();

            var report = await _reportService.GetByIdAsync(id);
            if (report == null)
                return NotFound();

            await _reportService.UpdateStatusAsync(id, status);

            if (!string.IsNullOrWhiteSpace(report.ReporterId))
            {
                await _notificationService.CreateAsync(new CheckPoint.Models.Notifications.Notification
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    UserId = report.ReporterId,
                    Type = "ReportUpdated",
                    ReferenceId = report.Id,
                    Message = $"Your report status was updated to {status}."
                });
            }

            var adminUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(adminUserId))
            {
                await _auditLogService.LogAsync(
                    adminUserId,
                    "UpdateReportStatus",
                    "Report",
                    id);
            }

            return RedirectToAction("Details", new { id });
        }
    }
}