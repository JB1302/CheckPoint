using CheckPoint.Models.AuditLogs;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly AuditLogService _auditLogService;

        public AuditLogController(AuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        // /AuditLog
        // /AuditLog?userId=xxx
        // /AuditLog?entityType=Event&entityId=xxx
        public async Task<IActionResult> Index(string? userId, string? entityType, string? entityId)
        {
            List<AuditLog> logs;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                logs = await _auditLogService.GetByUserIdAsync(userId);
            }
            else if (!string.IsNullOrWhiteSpace(entityType) && !string.IsNullOrWhiteSpace(entityId))
            {
                logs = await _auditLogService.GetByEntityAsync(entityType, entityId);
            }
            else
            {
                logs = await _auditLogService.GetAllAsync();
            }

            ViewBag.UserId = userId;
            ViewBag.EntityType = entityType;
            ViewBag.EntityId = entityId;

            return View(logs);
        }

        // /AuditLog/Details/{id}
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var log = await _auditLogService.GetByIdAsync(id);

            if (log == null)
                return NotFound();

            return View(log);
        }

        // /AuditLog/ByUser/{userId}
        public async Task<IActionResult> ByUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var logs = await _auditLogService.GetByUserIdAsync(userId);

            ViewBag.UserId = userId;
            return View("Index", logs);
        }

        // /AuditLog/ByEntity?entityType=Event&entityId=123
        public async Task<IActionResult> ByEntity(string entityType, string entityId)
        {
            if (string.IsNullOrWhiteSpace(entityType) || string.IsNullOrWhiteSpace(entityId))
                return BadRequest();

            var logs = await _auditLogService.GetByEntityAsync(entityType, entityId);

            ViewBag.EntityType = entityType;
            ViewBag.EntityId = entityId;

            return View("Index", logs);
        }
    }
}