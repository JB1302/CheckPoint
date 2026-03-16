using CheckPoint.Models;
using CheckPoint.Services;
using CheckPoint.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.Controllers
{
    [Authorize]
    public class PublicacionesController : Controller
    {
        private readonly PublicacionService _postService;
        private readonly ComentarioService _commentService;
        private readonly EventoService _eventService;
        private readonly NotificacionService _notificationService;

        public PublicacionesController(
            PublicacionService postService,
            ComentarioService commentService,
            EventoService eventService,
            NotificacionService notificationService)
        {
            _postService = postService;
            _commentService = commentService;
            _eventService = eventService;
            _notificationService = notificationService;
        }

        // Muestra el formulario para crear una publicacion.
        public async Task<IActionResult> Create(string eventId)
        {
            var ev = await _eventService.GetByIdAsync(eventId);
            if (ev == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(new CrearPublicacionViewModel { EventId = eventId });
        }

        // Crea una publicacion para un evento.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearPublicacionViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var ev = await _eventService.GetByIdAsync(model.EventId);
            if (ev == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var post = new Publicacion
            {
                EventId = model.EventId,
                AuthorId = userId,
                Title = model.Title,
                Content = model.Content,
                Type = model.Type
            };

            await _postService.CreateAsync(post);
            return RedirectToAction("Details", "Eventos", new { id = model.EventId });
        }

        // Agrega un comentario a una publicacion.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CrearComentarioViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", "Eventos", new { id = model.EventId });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            await _commentService.CreateAsync(new Comentario
            {
                PostId = model.PostId,
                AuthorId = userId,
                Content = model.Content
            });

            return RedirectToAction("Details", "Eventos", new { id = model.EventId });
        }

        // Elimina logicamente una publicacion.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, string eventId)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ev = await _eventService.GetByIdAsync(post.EventId);

            if (post.AuthorId != userId && ev?.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _postService.SoftDeleteAsync(id);
            return RedirectToAction("Details", "Eventos", new { id = eventId });
        }
    }
}


