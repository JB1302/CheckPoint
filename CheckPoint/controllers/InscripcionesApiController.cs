using CheckPoint.Models.Registrations;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/inscripciones")]
    public class InscripcionesApiController : ControllerBase
    {
        private readonly RegistrationService _registrationService;

        public InscripcionesApiController(RegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var inscripciones = await _registrationService.GetAllAsync();
                return Ok(inscripciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las inscripciones.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var inscripcion = await _registrationService.GetByIdAsync(id);
                if (inscripcion == null)
                    return NotFound(new { mensaje = "Inscripción no encontrada." });

                return Ok(inscripcion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la inscripción.", detalle = ex.Message });
            }
        }

        [HttpGet("evento/{eventId}")]
        public async Task<IActionResult> GetByEventId(string eventId)
        {
            try
            {
                var inscripciones = await _registrationService.GetByEventIdAsync(eventId);
                return Ok(inscripciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las inscripciones del evento.", detalle = ex.Message });
            }
        }

        [HttpGet("usuario/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            try
            {
                var inscripciones = await _registrationService.GetByUserIdAsync(userId);
                return Ok(inscripciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las inscripciones del usuario.", detalle = ex.Message });
            }
        }

        [HttpGet("verificar")]
        public async Task<IActionResult> IsRegistered([FromQuery] string eventId, [FromQuery] string userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eventId) || string.IsNullOrWhiteSpace(userId))
                    return BadRequest(new { mensaje = "Los parámetros eventId y userId son obligatorios." });

                var inscrito = await _registrationService.IsRegisteredAsync(eventId, userId);
                return Ok(new { eventId, userId, inscrito });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al verificar la inscripción.", detalle = ex.Message });
            }
        }

        [HttpGet("evento/{eventId}/confirmados")]
        public async Task<IActionResult> CountConfirmed(string eventId)
        {
            try
            {
                var total = await _registrationService.CountConfirmedByEventIdAsync(eventId);
                return Ok(new { eventId, confirmados = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al contar las inscripciones confirmadas.", detalle = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRegistrationRequest request)
        {
            try
            {
                var yaInscrito = await _registrationService.IsRegisteredAsync(request.EventId, request.UserId);
                if (yaInscrito)
                    return BadRequest(new { mensaje = "El usuario ya está inscrito en este evento." });

                var inscripcion = new Registration
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    EventId = request.EventId,
                    UserId = request.UserId,
                    Status = request.Status ?? "Pending"
                };

                await _registrationService.CreateAsync(inscripcion);

                return StatusCode(201, inscripcion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la inscripción.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpPatch("{id}/estado")]
        public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateRegistrationStatusRequest request)
        {
            try
            {
                var existente = await _registrationService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Inscripción no encontrada." });

                await _registrationService.UpdateStatusAsync(id, request.Status);

                return Ok(new { mensaje = "Estado de la inscripción actualizado correctamente.", nuevoEstado = request.Status });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el estado de la inscripción.", detalle = ex.Message });
            }
        }
    }

    public class CreateRegistrationRequest
    {
        public string EventId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? Status { get; set; }
    }

    public class UpdateRegistrationStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}
