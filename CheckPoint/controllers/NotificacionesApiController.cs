using CheckPoint.Models.Notifications;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/notificaciones")]
    public class NotificacionesApiController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificacionesApiController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var notificaciones = await _notificationService.GetAllAsync();
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las notificaciones.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var notificacion = await _notificationService.GetByIdAsync(id);
                if (notificacion == null)
                    return NotFound(new { mensaje = "Notificación no encontrada." });

                return Ok(notificacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la notificación.", detalle = ex.Message });
            }
        }

        [HttpGet("usuario/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            try
            {
                var notificaciones = await _notificationService.GetByUserIdAsync(userId);
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las notificaciones del usuario.", detalle = ex.Message });
            }
        }

        [HttpGet("usuario/{userId}/no-leidas")]
        public async Task<IActionResult> CountUnread(string userId)
        {
            try
            {
                var total = await _notificationService.CountUnreadAsync(userId);
                return Ok(new { userId, noLeidas = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al contar las notificaciones no leídas.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
        {
            try
            {
                var notificacion = new Notification
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    UserId = request.UserId,
                    Type = request.Type,
                    ReferenceId = request.ReferenceId,
                    Message = request.Message
                };

                await _notificationService.CreateAsync(notificacion);

                return StatusCode(201, notificacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la notificación.", detalle = ex.Message });
            }
        }

        [Authorize]
        [HttpPatch("{id}/leer")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            try
            {
                var existente = await _notificationService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Notificación no encontrada." });

                await _notificationService.MarkAsReadAsync(id);

                return Ok(new { mensaje = "Notificación marcada como leída." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al marcar la notificación como leída.", detalle = ex.Message });
            }
        }

        [Authorize]
        [HttpPatch("usuario/{userId}/leer-todas")]
        public async Task<IActionResult> MarkAllRead(string userId)
        {
            try
            {
                await _notificationService.MarkAllReadAsync(userId);
                return Ok(new { mensaje = "Todas las notificaciones marcadas como leídas." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al marcar las notificaciones como leídas.", detalle = ex.Message });
            }
        }
    }

    public class CreateNotificationRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string ReferenceId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
