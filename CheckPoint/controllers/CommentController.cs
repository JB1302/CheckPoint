using CheckPoint.Models.Comments;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly CommentService _commentService;
        private readonly PostService _postService;
        private readonly EventsService _eventService;

        public CommentsController(
            CommentService commentService,
            PostService postService,
            EventsService eventService)
        {
            _commentService = commentService;
            _postService = postService;
            _eventService = eventService;
        }

        // /Comments/ByPost?postId=...
        [HttpGet]
        public async Task<IActionResult> ByPost(string postId)
        {
            if (string.IsNullOrWhiteSpace(postId))
                return BadRequest();

            var comments = await _commentService.GetByPostIdAsync(postId);
            ViewBag.PostId = postId;

            return View(comments);
        }

        // Crea comentario sobre un post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string postId, string eventId, string content)
        {
            if (string.IsNullOrWhiteSpace(postId) ||
                string.IsNullOrWhiteSpace(eventId) ||
                string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var post = await _postService.GetByIdAsync(postId);
            if (post == null)
                return NotFound();

            await _commentService.CreateAsync(new Comment
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                PostId = postId,
                AuthorId = userId,
                Content = content
            });

            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        // Elimina lógicamente un comentario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, string eventId)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var comment = await _commentService.GetByIdAsync(id);
            if (comment == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var post = await _postService.GetByIdAsync(comment.PostId);
            var ev = post != null ? await _eventService.GetByIdAsync(post.EventId) : null;

            if (comment.AuthorId != userId && ev?.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _commentService.SoftDeleteAsync(id);

            return RedirectToAction("Details", "Events", new { id = eventId });
        }
    }
}