using CheckPoint.Models.Posts;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/publicaciones")]
    public class PublicacionesApiController : ControllerBase
    {
        private readonly PostService _postService;

        public PublicacionesApiController(PostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var publicaciones = await _postService.GetAllAsync();
                return Ok(publicaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las publicaciones.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var publicacion = await _postService.GetByIdAsync(id);
                if (publicacion == null)
                    return NotFound(new { mensaje = "Publicación no encontrada." });

                return Ok(publicacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener la publicación.", detalle = ex.Message });
            }
        }

        [HttpGet("evento/{eventId}")]
        public async Task<IActionResult> GetByEventId(string eventId)
        {
            try
            {
                var publicaciones = await _postService.GetByEventIdAsync(eventId);
                return Ok(publicaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener las publicaciones del evento.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostRequest request)
        {
            try
            {
                var publicacion = new Post
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    EventId = request.EventId,
                    AuthorId = request.AuthorId,
                    Content = request.Content
                };

                await _postService.CreateAsync(publicacion);

                return StatusCode(201, publicacion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear la publicación.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePostRequest request)
        {
            try
            {
                var existente = await _postService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Publicación no encontrada." });

                if (!string.IsNullOrWhiteSpace(request.Content))
                    existente.Content = request.Content;

                await _postService.UpdateAsync(id, existente);

                return Ok(existente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al actualizar la publicación.", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existente = await _postService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Publicación no encontrada." });

                await _postService.DeleteAsync(id);

                return Ok(new { mensaje = "Publicación eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar la publicación.", detalle = ex.Message });
            }
        }
    }

    public class CreatePostRequest
    {
        public string EventId { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class UpdatePostRequest
    {
        public string? Content { get; set; }
    }
}
