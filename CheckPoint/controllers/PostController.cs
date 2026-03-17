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

        // Show posts for an event
        [HttpGet]
        public async Task<IActionResult> ByEvent(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest();

            var ev = await _eventsService.GetByIdAsync(eventId);
            if (ev == null)
                return NotFound();

            var posts = await _postService.GetByEventIdAsync(eventId);

            ViewBag.EventId = eventId;
            ViewBag.EventTitle = ev.Title;

            return View(posts);
        }

        // Show create post form
        [HttpGet]
        public async Task<IActionResult> Create(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest();

            var ev = await _eventsService.GetByIdAsync(eventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var model = new Post
            {
                EventId = eventId
            };

            return View(model);
        }

        // Save new post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ev = await _eventsService.GetByIdAsync(model.EventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            model.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            model.AuthorId = userId;

            await _postService.CreateAsync(model);

            await _auditLogService.LogAsync(
                userId,
                "CreatePost",
                "Post",
                model.Id);

            return RedirectToAction("Details", "Events", new { id = model.EventId });
        }

        // Show edit post form
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var post = await _postService.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            var ev = await _eventsService.GetByIdAsync(post.EventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (post.AuthorId != userId && ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(post);
        }

        // Save post changes
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

            var ev = await _eventsService.GetByIdAsync(existing.EventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (existing.AuthorId != userId && ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            existing.Content = model.Content;

            await _postService.UpdateAsync(id, existing);

            await _auditLogService.LogAsync(
                userId,
                "UpdatePost",
                "Post",
                existing.Id);

            return RedirectToAction("Details", "Events", new { id = existing.EventId });
        }

        // Delete post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var post = await _postService.GetByIdAsync(id);
            if (post == null)
                return NotFound();

            var ev = await _eventsService.GetByIdAsync(post.EventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            if (post.AuthorId != userId && ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _postService.DeleteAsync(id);

            await _auditLogService.LogAsync(
                userId,
                "DeletePost",
                "Post",
                id);

            return RedirectToAction("Details", "Events", new { id = post.EventId });
        }
    }
}