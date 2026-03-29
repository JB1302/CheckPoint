using CheckPoint.Models.Registrations;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class RegistrationService
    {
        private readonly IMongoCollection<Registration> _registrations;

        public RegistrationService(ContextoMongoDb context)
        {
            _registrations = context.Registrations;
        }

        public async Task<List<Registration>> GetAllAsync()
        {
            return await _registrations.Find(_ => true).ToListAsync();
        }

        public async Task<List<Registration>> GetByEventIdAsync(string eventId)
        {
            return await _registrations.Find(r => r.EventId == eventId).ToListAsync();
        }

        public async Task<List<Registration>> GetByUserIdAsync(string userId)
        {
            return await _registrations.Find(r => r.UserId == userId).ToListAsync();
        }

        public async Task<Registration?> GetByEventAndUserAsync(string eventId, string userId)
        {
            return await _registrations.Find(r => r.EventId == eventId && r.UserId == userId)
                                       .FirstOrDefaultAsync();
        }

        public async Task<Registration?> GetByIdAsync(string id)
        {
            return await _registrations.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Registration registration)
        {
            registration.RegisteredAt = DateTime.UtcNow;
            await _registrations.InsertOneAsync(registration);
        }

        public async Task UpdateStatusAsync(string id, string status)
        {
            var update = Builders<Registration>.Update
                .Set(r => r.Status, status);

            await _registrations.UpdateOneAsync(r => r.Id == id, update);
        }

        public async Task<bool> IsRegisteredAsync(string eventId, string userId)
        {
            return await _registrations.Find(r => r.EventId == eventId
                                               && r.UserId == userId
                                               && r.Status != "Cancelled")
                                       .AnyAsync();
        }

        public async Task<long> CountConfirmedByEventIdAsync(string eventId)
        {
            return await _registrations.CountDocumentsAsync(r => r.EventId == eventId && r.Status == "Confirmed");
        }
    }
}