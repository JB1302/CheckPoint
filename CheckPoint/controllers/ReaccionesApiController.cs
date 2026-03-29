using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/reacciones")]
    public class ReaccionesApiController : ControllerBase
    {
        private readonly ReactionService _reactionService;

        public ReaccionesApiController(ReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reacciones = await _reactionService.GetAllAsync();
                return Ok(reacciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las reacciones.", detalle = ex.Message });
            }
        }

        [HttpGet("contar")]
        public async Task<IActionResult> Contar([FromQuery] string targetId, [FromQuery] string targetType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(targetId) || string.IsNullOrWhiteSpace(targetType))
                    return BadRequest(new { mensaje = "Los parámetros targetId y targetType son obligatorios." });

                var total = await _reactionService.CountByTargetAsync(targetId, targetType);
                return Ok(new { targetId, targetType, total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al contar las reacciones.", detalle = ex.Message });
            }
        }

        [HttpGet("usuario")]
        public async Task<IActionResult> GetUserReaction([FromQuery] string targetId, [FromQuery] string targetType, [FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(targetId) || string.IsNullOrWhiteSpace(targetType) || string.IsNullOrWhiteSpace(userId))
                    return BadRequest(new { mensaje = "Los parámetros targetId, targetType y userId son obligatorios." });

                var reaccion = await _reactionService.GetUserReactionAsync(targetId, targetType, userId);
                if (reaccion == null)
                    return NotFound(new { mensaje = "El usuario no tiene reacción en este elemento." });

                return Ok(reaccion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la reacción del usuario.", detalle = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("toggle")]
        public async Task<IActionResult> Toggle([FromBody] ToggleReactionRequest request)
        {
            try
            {
                await _reactionService.ToggleAsync(request.TargetId, request.TargetType, request.UserId, request.ReactionType);

                var total = await _reactionService.CountByTargetAsync(request.TargetId, request.TargetType);
                var reaccion = await _reactionService.GetUserReactionAsync(request.TargetId, request.TargetType, request.UserId);

                return Ok(new
                {
                    mensaje = reaccion != null ? "Reacción agregada." : "Reacción eliminada.",
                    activo = reaccion != null,
                    total
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al procesar la reacción.", detalle = ex.Message });
            }
        }
    }

    public class ToggleReactionRequest
    {
        public string TargetId { get; set; } = string.Empty;
        public string TargetType { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string ReactionType { get; set; } = string.Empty;
    }
}
