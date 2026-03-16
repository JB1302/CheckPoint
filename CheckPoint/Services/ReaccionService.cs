using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class ReaccionService
    {
        private readonly IMongoCollection<Reaccion> _reactions;

        public ReaccionService(ContextoMongoDb context) => _reactions = context.Reactions;

        public async Task<long> CountByTargetAsync(string targetId, string targetType) =>
            await _reactions.CountDocumentsAsync(r => r.TargetId == targetId && r.TargetType == targetType);

        public async Task<Reaccion?> GetUserReactionAsync(string targetId, string targetType, string userId) =>
            await _reactions.Find(r => r.TargetId == targetId
                                    && r.TargetType == targetType
                                    && r.UserId == userId)
                            .FirstOrDefaultAsync();

        public async Task ToggleAsync(string targetId, string targetType, string userId, string type)
        {
            var existing = await GetUserReactionAsync(targetId, targetType, userId);
            if (existing != null)
                await _reactions.DeleteOneAsync(r => r.Id == existing.Id);
            else
                await _reactions.InsertOneAsync(new Reaccion
                {
                    TargetId = targetId,
                    TargetType = targetType,
                    UserId = userId,
                    Type = type
                });
        }
    }
}

