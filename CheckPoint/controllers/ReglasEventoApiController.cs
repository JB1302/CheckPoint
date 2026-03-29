using CheckPoint.Models.EventRules;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/reglas")]
    public class ReglasEventoApiController : ControllerBase
    {
        private readonly EventRulesService _eventRulesService;

        public ReglasEventoApiController(EventRulesService eventRulesService)
        {
            _eventRulesService = eventRulesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reglas = await _eventRulesService.GetAllAsync();
                return Ok(reglas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las reglas.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var regla = await _eventRulesService.GetByIdAsync(id);
                if (regla == null)
                    return NotFound(new { mensaje = "Regla no encontrada." });

                return Ok(regla);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la regla.", detalle = ex.Message });
            }
        }

        [HttpGet("evento/{eventId}")]
        public async Task<IActionResult> GetByEventId(string eventId)
        {
            try
            {
                var regla = await _eventRulesService.GetByEventIdAsync(eventId);
                if (regla == null)
                    return NotFound(new { mensaje = "No se encontraron reglas para este evento." });

                return Ok(regla);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las reglas del evento.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventRulesRequest request)
        {
            try
            {
                var regla = new EventRules
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    EventId = request.EventId,
                    RulesText = request.RulesText,
                    CheckInRequired = request.CheckInRequired,
                    AllowedPlatforms = request.AllowedPlatforms ?? new List<string>(),
                    MinRank = request.MinRank
                };

                await _eventRulesService.CreateAsync(regla);

                return StatusCode(201, regla);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la regla.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateEventRulesRequest request)
        {
            try
            {
                var existente = await _eventRulesService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Regla no encontrada." });

                if (!string.IsNullOrWhiteSpace(request.RulesText))
                    existente.RulesText = request.RulesText;

                if (request.CheckInRequired.HasValue)
                    existente.CheckInRequired = request.CheckInRequired.Value;

                if (request.AllowedPlatforms != null)
                    existente.AllowedPlatforms = request.AllowedPlatforms;

                if (request.MinRank != null)
                    existente.MinRank = request.MinRank;

                await _eventRulesService.UpdateAsync(id, existente);

                return Ok(existente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la regla.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("evento/{eventId}")]
        public async Task<IActionResult> DeleteByEventId(string eventId)
        {
            try
            {
                var existente = await _eventRulesService.GetByEventIdAsync(eventId);
                if (existente == null)
                    return NotFound(new { mensaje = "No se encontraron reglas para este evento." });

                await _eventRulesService.DeleteByEventIdAsync(eventId);

                return Ok(new { mensaje = "Reglas del evento eliminadas correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar las reglas.", detalle = ex.Message });
            }
        }
    }

    public class CreateEventRulesRequest
    {
        public string EventId { get; set; } = string.Empty;
        public string RulesText { get; set; } = string.Empty;
        public bool CheckInRequired { get; set; }
        public List<string>? AllowedPlatforms { get; set; }
        public string? MinRank { get; set; }
    }

    public class UpdateEventRulesRequest
    {
        public string? RulesText { get; set; }
        public bool? CheckInRequired { get; set; }
        public List<string>? AllowedPlatforms { get; set; }
        public string? MinRank { get; set; }
    }
}
