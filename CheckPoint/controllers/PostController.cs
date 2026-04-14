using CheckPoint.Models.Posts;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly PostService _postService;
        private readonly EventsService _eventsService;
        private readonly AuditLogService _auditLogService;

        public PostsController(
            PostService postService,
            EventsService eventsService,
            AuditLogService auditLogService)
        {
            _postService = postService;
            _eventsService = eventsService;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetGenericAsync();
            ViewBag.EventTitle = "Todas las publicaciones";
            ViewBag.IsGeneric = true;
            return View(posts);
        }

        [HttpGet]
        public async Task<IActionResult> ByEvent(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest();

            var posts = await _postService.GetByEventIdAsync(eventId);
            var ev = await _eventsService.GetByIdAsync(eventId);

            ViewBag.EventId = eventId;
            ViewBag.EventTitle = ev?.Title ?? eventId;
            ViewBag.IsGeneric = false;

            return View("Index", posts);
        }

        [HttpGet]
        public async Task<IActionResult> Create(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest();

            var ev = await _eventsService.GetByIdAsync(eventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var model = new Post
            {
                EventId = eventId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string content, string? eventId)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                if (!string.IsNullOrWhiteSpace(eventId))
                    return RedirectToAction("Details", "Events", new { id = eventId });

                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (!string.IsNullOrWhiteSpace(eventId))
            {
                var ev = await _eventsService.GetByIdAsync(eventId);
                if (ev == null)
                    return NotFound();
            }

            var post = new Post
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                AuthorId = userId,
                Content = content.Trim(),
                EventId = string.IsNullOrWhiteSpace(eventId) ? string.Empty : eventId
            };

            await _postService.CreateAsync(post);

            await _auditLogService.LogAsync(
                userId,
                "CreatePost",
                "Post",
                post.Id);

            if (!string.IsNullOrWhiteSpace(eventId))
                return RedirectToAction("Details", "Events", new { id = eventId });

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var post = await _postService.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (string.IsNullOrWhiteSpace(post.EventId))
            {
                if (post.AuthorId != userId && !User.IsInRole("Admin"))
                    return Forbid();

                return View(post);
            }

            var ev = await _eventsService.GetByIdAsync(post.EventId);
            if (ev == null)
                return NotFound();

            if (post.AuthorId != userId && ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Post model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var existing = await _postService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (string.IsNullOrWhiteSpace(existing.EventId))
            {
                if (existing.AuthorId != userId && !User.IsInRole("Admin"))
                    return Forbid();
            }
            else
            {
                var ev = await _eventsService.GetByIdAsync(existing.EventId);
                if (ev == null)
                    return NotFound();

                if (existing.AuthorId != userId && ev.OrganizerId != userId && !User.IsInRole("Admin"))
                    return Forbid();
            }

            existing.Content = model.Content?.Trim() ?? existing.Content;

            await _postService.UpdateAsync(id, existing);

            await _auditLogService.LogAsync(
                userId,
                "UpdatePost",
                "Post",
                existing.Id);

            if (!string.IsNullOrWhiteSpace(existing.EventId))
                return RedirectToAction("Details", "Events", new { id = existing.EventId });

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var post = await _postService.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (string.IsNullOrWhiteSpace(post.EventId))
            {
                if (post.AuthorId != userId && !User.IsInRole("Admin"))
                    return Forbid();
            }
            else
            {
                var ev = await _eventsService.GetByIdAsync(post.EventId);
                if (ev == null)
                    return NotFound();

                if (post.AuthorId != userId && ev.OrganizerId != userId && !User.IsInRole("Admin"))
                    return Forbid();
            }

            await _postService.DeleteAsync(id);

            await _auditLogService.LogAsync(
                userId,
                "DeletePost",
                "Post",
                id);

            if (!string.IsNullOrWhiteSpace(post.EventId))
                return RedirectToAction("Details", "Events", new { id = post.EventId });

            return RedirectToAction(nameof(Index));
        }
    }
}