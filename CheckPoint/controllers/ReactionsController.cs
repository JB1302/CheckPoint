using CheckPoint.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CheckPoint.controllers
{
    [Authorize]
    public class ReactionsController : Controller
    {
        private readonly ReactionService _reactionService;

        public ReactionsController(ReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        // Show reaction count for a target
        [HttpGet]
        public async Task<IActionResult> Count(string targetId, string targetType)
        {
            if (string.IsNullOrWhiteSpace(targetId) || string.IsNullOrWhiteSpace(targetType))
                return BadRequest();

            var count = await _reactionService.CountByTargetAsync(targetId, targetType);

            return Json(new
            {
                targetId,
                targetType,
                count
            });
        }

        // Show current user's reaction for a target
        [HttpGet]
        public async Task<IActionResult> MyReaction(string targetId, string targetType)
        {
            if (string.IsNullOrWhiteSpace(targetId) || string.IsNullOrWhiteSpace(targetType))
                return BadRequest();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            var reaction = await _reactionService.GetUserReactionAsync(targetId, targetType, userId);

            return Json(new
            {
                targetId,
                targetType,
                hasReaction = reaction != null,
                reactionType = reaction?.ReactionType
            });
        }

        // Toggle reaction for current user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(string targetId, string targetType, string reactionType, string? returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(targetId) ||
                string.IsNullOrWhiteSpace(targetType) ||
                string.IsNullOrWhiteSpace(reactionType))
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid();

            await _reactionService.ToggleAsync(targetId, targetType, userId, reactionType);

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}