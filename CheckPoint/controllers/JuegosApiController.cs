using CheckPoint.Models.Games;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/juegos")]
    public class JuegosApiController : ControllerBase
    {
        private readonly GameService _gameService;

        public JuegosApiController(GameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var juegos = await _gameService.GetAllAsync();
                return Ok(juegos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener juegos.", detalle = ex.Message });
            }
        }

        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos()
        {
            try
            {
                var juegos = await _gameService.GetAllActiveAsync();
                return Ok(juegos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener juegos activos.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var juego = await _gameService.GetByIdAsync(id);
                if (juego == null)
                    return NotFound(new { mensaje = "Juego no encontrado." });

                return Ok(juego);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el juego.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameRequest request)
        {
            try
            {
                var juego = new Game
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Name = request.Name,
                    Genre = request.Genre,
                    Platforms = request.Platforms ?? new List<string>(),
                    IsActive = request.IsActive ?? true
                };

                await _gameService.CreateAsync(juego);

                return StatusCode(201, juego);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el juego.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateGameRequest request)
        {
            try
            {
                var existente = await _gameService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Juego no encontrado." });

                if (!string.IsNullOrWhiteSpace(request.Name))
                    existente.Name = request.Name;

                if (!string.IsNullOrWhiteSpace(request.Genre))
                    existente.Genre = request.Genre;

                if (request.Platforms != null)
                    existente.Platforms = request.Platforms;

                if (request.IsActive.HasValue)
                    existente.IsActive = request.IsActive.Value;

                await _gameService.UpdateAsync(id, existente);

                return Ok(existente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el juego.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existente = await _gameService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Juego no encontrado." });

                await _gameService.DeleteAsync(id);

                return Ok(new { mensaje = "Juego eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el juego.", detalle = ex.Message });
            }
        }
    }

    public class CreateGameRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public List<string>? Platforms { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UpdateGameRequest
    {
        public string? Name { get; set; }
        public string? Genre { get; set; }
        public List<string>? Platforms { get; set; }
        public bool? IsActive { get; set; }
    }
}
