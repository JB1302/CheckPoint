using CheckPoint.Models.Reactions;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class ReactionService
    {
        private readonly IMongoCollection<Reaction> _reactions;

        public ReactionService(ContextoMongoDb context)
        {
            _reactions = context.Reactions;
        }

        public async Task<List<Reaction>> GetAllAsync()
        {
            return await _reactions.Find(_ => true).ToListAsync();
        }

        public async Task<long> CountByTargetAsync(string targetId, string targetType)
        {
            return await _reactions.CountDocumentsAsync(r => r.TargetId == targetId && r.TargetType == targetType);
        }

        public async Task<Reaction?> GetUserReactionAsync(string targetId, string targetType, string userId)
        {
            return await _reactions.Find(r => r.TargetId == targetId
                                           && r.TargetType == targetType
                                           && r.UserId == userId)
                                   .FirstOrDefaultAsync();
        }

        public async Task ToggleAsync(string targetId, string targetType, string userId, string type)
        {
            var existing = await GetUserReactionAsync(targetId, targetType, userId);

            if (existing != null)
            {
                await _reactions.DeleteOneAsync(r => r.Id == existing.Id);
            }
            else
            {
                await _reactions.InsertOneAsync(new Reaction
                {
                    TargetId = targetId,
                    TargetType = targetType,
                    UserId = userId,
                    ReactionType = type,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }
}