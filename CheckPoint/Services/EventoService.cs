using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class EventoService
    {
        private readonly IMongoCollection<Evento> _events;

        public EventoService(ContextoMongoDb context) => _events = context.Events;

        public async Task<List<Evento>> GetAllPublicAsync() =>
            await _events.Find(e => e.Status == "Open" || e.Status == "InProgress")
                         .SortByDescending(e => e.CreatedAt)
                         .ToListAsync();

        public async Task<List<Evento>> GetByOrganizerIdAsync(string organizerId) =>
            await _events.Find(e => e.OrganizerId == organizerId)
                         .SortByDescending(e => e.CreatedAt)
                         .ToListAsync();

        public async Task<List<Evento>> GetByGameIdAsync(string gameId) =>
            await _events.Find(e => e.GameId == gameId && e.Status == "Open")
                         .ToListAsync();

        public async Task<Evento?> GetByIdAsync(string id) =>
            await _events.Find(e => e.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Evento ev) =>
            await _events.InsertOneAsync(ev);

        public async Task UpdateAsync(string id, Evento ev)
        {
            ev.UpdatedAt = DateTime.UtcNow;
            await _events.ReplaceOneAsync(e => e.Id == id, ev);
        }

        public async Task IncrementParticipantsAsync(string id)
        {
            var update = Builders<Evento>.Update
                .Inc(e => e.CurrentParticipants, 1)
                .Set(e => e.UpdatedAt, DateTime.UtcNow);
            await _events.UpdateOneAsync(e => e.Id == id, update);
        }

        public async Task DecrementParticipantsAsync(string id)
        {
            var update = Builders<Evento>.Update
                .Inc(e => e.CurrentParticipants, -1)
                .Set(e => e.UpdatedAt, DateTime.UtcNow);
            await _events.UpdateOneAsync(e => e.Id == id, update);
        }

        public async Task<List<Evento>> SearchAsync(string? game, string? type, string? query)
        {
            var filter = Builders<Evento>.Filter.In(e => e.Status, new[] { "Open", "InProgress" });

            if (!string.IsNullOrWhiteSpace(game))
                filter &= Builders<Evento>.Filter.Eq(e => e.GameId, game);

            if (!string.IsNullOrWhiteSpace(type))
                filter &= Builders<Evento>.Filter.Eq(e => e.Type, type);

            if (!string.IsNullOrWhiteSpace(query))
                filter &= Builders<Evento>.Filter.Regex(e => e.Title,
                    new MongoDB.Bson.BsonRegularExpression(query, "i"));

            return await _events.Find(filter).SortByDescending(e => e.StartDate).ToListAsync();
        }
    }
}

