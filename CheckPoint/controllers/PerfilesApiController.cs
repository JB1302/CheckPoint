using CheckPoint.Models.Profiles;
using CheckPoint.Services;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/perfiles")]
    public class PerfilesApiController : ControllerBase
    {
        private readonly ProfileService _profileService;

        public PerfilesApiController(ProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var perfiles = await _profileService.GetAllAsync();
                return Ok(perfiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener perfiles.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var perfil = await _profileService.GetByIdAsync(id);
                if (perfil == null)
                    return NotFound(new { mensaje = "Perfil no encontrado." });

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el perfil.", detalle = ex.Message });
            }
        }

        [HttpGet("usuario/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            try
            {
                var perfil = await _profileService.GetByUserIdAsync(userId);
                if (perfil == null)
                    return NotFound(new { mensaje = "Perfil no encontrado para ese usuario." });

                return Ok(perfil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el perfil.", detalle = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProfileRequest request)
        {
            try
            {
                var existente = await _profileService.GetByUserIdAsync(request.UserId);
                if (existente != null)
                    return BadRequest(new { mensaje = "Ya existe un perfil para ese usuario." });

                var perfil = new Profile
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    UserId = request.UserId,
                    DisplayName = request.DisplayName,
                    Bio = request.Bio,
                    AvatarUrl = request.AvatarUrl,
                    Country = request.Country,
                    FavoriteGameIds = request.FavoriteGameIds ?? new List<string>()
                };

                await _profileService.CreateAsync(perfil);

                return StatusCode(201, perfil);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el perfil.", detalle = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                var existente = await _profileService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Perfil no encontrado." });

                if (!string.IsNullOrWhiteSpace(request.DisplayName))
                    existente.DisplayName = request.DisplayName;

                if (request.Bio != null)
                    existente.Bio = request.Bio;

                if (request.AvatarUrl != null)
                    existente.AvatarUrl = request.AvatarUrl;

                if (request.Country != null)
                    existente.Country = request.Country;

                if (request.FavoriteGameIds != null)
                    existente.FavoriteGameIds = request.FavoriteGameIds;

                await _profileService.UpdateAsync(id, existente);

                return Ok(existente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el perfil.", detalle = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existente = await _profileService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Perfil no encontrado." });

                await _profileService.DeleteAsync(id);

                return Ok(new { mensaje = "Perfil eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el perfil.", detalle = ex.Message });
            }
        }
    }

    public class CreateProfileRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Country { get; set; }
        public List<string>? FavoriteGameIds { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Country { get; set; }
        public List<string>? FavoriteGameIds { get; set; }
    }
}
