using CheckPoint.Models.Comments;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckPoint.Controllers
{
    [ApiController]
    [Route("api/comentarios")]
    public class ComentariosApiController : ControllerBase
    {
        private readonly CommentService _commentService;

        public ComentariosApiController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var comentarios = await _commentService.GetAllAsync();
                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los comentarios.", detalle = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var comentario = await _commentService.GetByIdAsync(id);
                if (comentario == null)
                    return NotFound(new { mensaje = "Comentario no encontrado." });

                return Ok(comentario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el comentario.", detalle = ex.Message });
            }
        }

        [HttpGet("publicacion/{postId}")]
        public async Task<IActionResult> GetByPostId(string postId)
        {
            try
            {
                var comentarios = await _commentService.GetByPostIdAsync(postId);
                return Ok(comentarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los comentarios de la publicación.", detalle = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
        {
            try
            {
                var comentario = new Comment
                {
                    Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    PostId = request.PostId,
                    AuthorId = request.AuthorId,
                    Content = request.Content
                };

                await _commentService.CreateAsync(comentario);

                return StatusCode(201, comentario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al crear el comentario.", detalle = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var existente = await _commentService.GetByIdAsync(id);
                if (existente == null)
                    return NotFound(new { mensaje = "Comentario no encontrado." });

                await _commentService.SoftDeleteAsync(id);

                return Ok(new { mensaje = "Comentario eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el comentario.", detalle = ex.Message });
            }
        }
    }

    public class CreateCommentRequest
    {
        public string PostId { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
