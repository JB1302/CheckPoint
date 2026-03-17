using CheckPoint.Models.Events;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class EventsService
    {
        private readonly IMongoCollection<Events> _events;

        public EventsService(ContextoMongoDb context)
        {
            _events = context.Events;
        }

        public async Task<List<Events>> GetAllPublicAsync()
        {
            return await _events.Find(e => e.Status == "Published")
                                .SortByDescending(e => e.CreatedAt)
                                .ToListAsync();
        }

        public async Task<List<Events>> GetByOrganizerIdAsync(string organizerId)
        {
            return await _events.Find(e => e.OrganizerId == organizerId)
                                .SortByDescending(e => e.CreatedAt)
                                .ToListAsync();
        }

        public async Task<List<Events>> GetByGameIdAsync(string gameId)
        {
            return await _events.Find(e => e.GameId == gameId && e.Status == "Published")
                                .ToListAsync();
        }

        public async Task<Events?> GetByIdAsync(string id)
        {
            return await _events.Find(e => e.Id == id)
                                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Events ev)
        {
            ev.CreatedAt = DateTime.UtcNow;
            await _events.InsertOneAsync(ev);
        }

        public async Task UpdateAsync(string id, Events ev)
        {
            await _events.ReplaceOneAsync(e => e.Id == id, ev);
        }

        public async Task<List<Events>> SearchAsync(string? game, string? type, string? query)
        {
            var filter = Builders<Events>.Filter.Eq(e => e.Status, "Published");

            if (!string.IsNullOrWhiteSpace(game))
                filter &= Builders<Events>.Filter.Eq(e => e.GameId, game);

            if (!string.IsNullOrWhiteSpace(type))
                filter &= Builders<Events>.Filter.Eq(e => e.EventType, type);

            if (!string.IsNullOrWhiteSpace(query))
                filter &= Builders<Events>.Filter.Regex(
                    e => e.Title,
                    new MongoDB.Bson.BsonRegularExpression(query, "i"));

            return await _events.Find(filter)
                                .SortByDescending(e => e.StartDate)
                                .ToListAsync();
        }

        public async Task<List<Events>> GetAllAsync()
        {
            return await _events.Find(_ => true)
                                .SortByDescending(e => e.CreatedAt)
                                .ToListAsync();
        }

        public async Task CancelAsync(string id)
        {
            var update = Builders<Events>.Update
                .Set(e => e.Status, "Cancelled");

            await _events.UpdateOneAsync(e => e.Id == id, update);
        }
    }
}