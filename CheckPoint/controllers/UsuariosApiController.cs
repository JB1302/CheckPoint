using CheckPoint.Models.Users;
using CheckPoint.Services;
using Microsoft.AspNetCore.Mvc;
using BCryptNet = BCrypt.Net.BCrypt;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosApiController : ControllerBase
    {
        private readonly UserService _userService;

        public UsuariosApiController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var usuarios = await _userService.GetAllAsync();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener usuarios.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var usuario = await _userService.GetByIdAsync(id);
                if (usuario == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el usuario.", detalle = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            try
            {
                if (await _userService.ExistsByEmailAsync(request.Email))
                    return BadRequest(new { mensaje = "El correo ya está registrado." });

                if (await _userService.ExistsByUsernameAsync(request.Username))
                    return BadRequest(new { mensaje = "El nombre de usuario ya existe." });

                var usuario = new User
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = BCryptNet.HashPassword(request.Password),
                    Role = request.Role ?? "User",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _userService.CreateAsync(usuario);

                return StatusCode(201, usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el usuario.", detalle = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var existente = await _userService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != existente.Email)
                {
                    if (await _userService.ExistsByEmailAsync(request.Email))
                        return BadRequest(new { mensaje = "El correo ya está registrado." });
                    existente.Email = request.Email;
                }

                if (!string.IsNullOrWhiteSpace(request.Username) && request.Username != existente.Username)
                {
                    if (await _userService.ExistsByUsernameAsync(request.Username))
                        return BadRequest(new { mensaje = "El nombre de usuario ya existe." });
                    existente.Username = request.Username;
                }

                if (!string.IsNullOrWhiteSpace(request.Role))
                    existente.Role = request.Role;

                if (request.IsActive.HasValue)
                    existente.IsActive = request.IsActive.Value;

                if (!string.IsNullOrWhiteSpace(request.Password))
                    existente.PasswordHash = BCryptNet.HashPassword(request.Password);

                await _userService.UpdateAsync(id, existente);

                return Ok(existente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar el usuario.", detalle = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existente = await _userService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Usuario no encontrado." });

                await _userService.DeleteAsync(id);

                return Ok(new { mensaje = "Usuario eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el usuario.", detalle = ex.Message });
            }
        }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Role { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
