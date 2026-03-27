using CheckPoint.Models.Events;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/eventos")]
    public class EventosApiController : ControllerBase
    {
        private readonly EventsService _eventsService;

        public EventosApiController(EventsService eventsService)
        {
            _eventsService = eventsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var eventos = await _eventsService.GetAllAsync();
                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener eventos.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var evento = await _eventsService.GetByIdAsync(id);
                if (evento == null)
                    return NotFound(new { mensaje = "Evento no encontrado." });

                return Ok(evento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el evento.", detalle = ex.Message });
            }
        }

        [HttpGet("organizador/{organizerId}")]
        public async Task<IActionResult> GetByOrganizer(string organizerId)
        {
            try
            {
                var eventos = await _eventsService.GetByOrganizerIdAsync(organizerId);
                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener eventos del organizador.", detalle = ex.Message });
            }
        }

        [HttpGet("juego/{gameId}")]
        public async Task<IActionResult> GetByGame(string gameId)
        {
            try
            {
                var eventos = await _eventsService.GetByGameIdAsync(gameId);
                return Ok(eventos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener eventos del juego.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            try
            {
                var organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrWhiteSpace(organizerId))
                    return Forbid();

                var evento = new Events
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Title = request.Title,
                    Description = request.Description,
                    GameId = request.GameId,
                    EventType = request.EventType,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Location = request.Location,
                    OnlineLink = request.OnlineLink,
                    MaxParticipants = request.MaxParticipants,
                    OrganizerId = organizerId,
                    Status = request.Status ?? "Published"
                };

                await _eventsService.CreateAsync(evento);

                return StatusCode(201, evento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el evento.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateEventRequest request)
        {
            try
            {
                var existente = await _eventsService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Evento no encontrado." });

                if (!string.IsNullOrWhiteSpace(request.Title))
                    existente.Title = request.Title;

                if (!string.IsNullOrWhiteSpace(request.Description))
                    existente.Description = request.Description;

                if (!string.IsNullOrWhiteSpace(request.GameId))
                    existente.GameId = request.GameId;

                if (!string.IsNullOrWhiteSpace(request.EventType))
                    existente.EventType = request.EventType;

                if (request.StartDate.HasValue)
                    existente.StartDate = request.StartDate.Value;

                if (request.EndDate.HasValue)
                    existente.EndDate = request.EndDate.Value;

                if (request.Location != null)
                    existente.Location = request.Location;

                if (request.OnlineLink != null)
                    existente.OnlineLink = request.OnlineLink;

                if (request.MaxParticipants.HasValue)
                    existente.MaxParticipants = request.MaxParticipants.Value;

                if (!string.IsNullOrWhiteSpace(request.Status))
                    existente.Status = request.Status;

                await _eventsService.UpdateAsync(id, existente);

                return Ok(existente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el evento.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existente = await _eventsService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Evento no encontrado." });

                await _eventsService.DeleteAsync(id);

                return Ok(new { mensaje = "Evento eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el evento.", detalle = ex.Message });
            }
        }
    }

    public class CreateEventRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Location { get; set; }
        public string? OnlineLink { get; set; }
        public int MaxParticipants { get; set; }
        public string OrganizerId { get; set; } = string.Empty;
        public string? Status { get; set; }
    }

    public class UpdateEventRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? GameId { get; set; }
        public string? EventType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }
        public string? OnlineLink { get; set; }
        public int? MaxParticipants { get; set; }
        public string? Status { get; set; }
    }
}
