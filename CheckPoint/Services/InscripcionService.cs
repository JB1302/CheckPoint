using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class InscripcionService
    {
        private readonly IMongoCollection<Inscripcion> _registrations;

        public InscripcionService(ContextoMongoDb context) => _registrations = context.Registrations;

        public async Task<List<Inscripcion>> GetByEventIdAsync(string eventId) =>
            await _registrations.Find(r => r.EventId == eventId).ToListAsync();

        public async Task<List<Inscripcion>> GetByUserIdAsync(string userId) =>
            await _registrations.Find(r => r.UserId == userId).ToListAsync();

        public async Task<Inscripcion?> GetByEventAndUserAsync(string eventId, string userId) =>
            await _registrations.Find(r => r.EventId == eventId && r.UserId == userId)
                                 .FirstOrDefaultAsync();

        public async Task<Inscripcion?> GetByIdAsync(string id) =>
            await _registrations.Find(r => r.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Inscripcion registration) =>
            await _registrations.InsertOneAsync(registration);

        public async Task UpdateStatusAsync(string id, string status)
        {
            var update = Builders<Inscripcion>.Update
                .Set(r => r.Status, status)
                .Set(r => r.UpdatedAt, DateTime.UtcNow);
            await _registrations.UpdateOneAsync(r => r.Id == id, update);
        }

        public async Task<bool> IsRegisteredAsync(string eventId, string userId) =>
            await _registrations.Find(r => r.EventId == eventId
                                        && r.UserId == userId
                                        && r.Status != "Cancelled").AnyAsync();
    }
}

