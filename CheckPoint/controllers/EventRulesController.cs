using CheckPoint.Models.EventRules;
using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.controllers
{
    [Authorize]
    public class EventRulesController : Controller
    {
        private readonly EventRulesService _eventRulesService;
        private readonly EventsService _eventsService;

        public EventRulesController(
            EventRulesService eventRulesService,
            EventsService eventsService)
        {
            _eventRulesService = eventRulesService;
            _eventsService = eventsService;
        }

        // /EventRules/ByEvent?eventId=...
        [HttpGet]
        public async Task<IActionResult> ByEvent(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest();

            var rules = await _eventRulesService.GetByEventIdAsync(eventId);
            if (rules == null)
                return NotFound();

            return View(rules);
        }

        // GET: /EventRules/Create?eventId=...
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

            var existingRules = await _eventRulesService.GetByEventIdAsync(eventId);
            if (existingRules != null)
                return RedirectToAction(nameof(Edit), new { id = existingRules.Id });

            return View(new EventRules
            {
                EventId = eventId
            });
        }

        // POST: /EventRules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventRules model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ev = await _eventsService.GetByIdAsync(model.EventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            var existingRules = await _eventRulesService.GetByEventIdAsync(model.EventId);
            if (existingRules != null)
            {
                ModelState.AddModelError("", "This event already has rules.");
                return View(model);
            }

            model.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();

            await _eventRulesService.CreateAsync(model);

            return RedirectToAction("Details", "Events", new { id = model.EventId });
        }

        // GET: /EventRules/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var rule = await _eventRulesService.GetByIdAsync(id);
            if (rule == null)
                return NotFound();

            var ev = await _eventsService.GetByIdAsync(rule.EventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            return View(rule);
        }

        // POST: /EventRules/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EventRules model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var existingRule = await _eventRulesService.GetByIdAsync(id);
            if (existingRule == null)
                return NotFound();

            var ev = await _eventsService.GetByIdAsync(existingRule.EventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _eventRulesService.UpdateAsync(id, model);

            return RedirectToAction("Details", "Events", new { id = model.EventId });
        }

        // POST: /EventRules/DeleteByEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteByEvent(string eventId)
        {
            if (string.IsNullOrWhiteSpace(eventId))
                return BadRequest();

            var ev = await _eventsService.GetByIdAsync(eventId);
            if (ev == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ev.OrganizerId != userId && !User.IsInRole("Admin"))
                return Forbid();

            await _eventRulesService.DeleteByEventIdAsync(eventId);

            return RedirectToAction("Details", "Events", new { id = eventId });
        }
    }
}