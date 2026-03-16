using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class ReglaEventoService
    {
        private readonly IMongoCollection<ReglaEvento> _eventRules;

        public ReglaEventoService(ContextoMongoDb context) => _eventRules = context.EventRules;

        public async Task<ReglaEvento?> GetByEventIdAsync(string eventId) =>
            await _eventRules.Find(r => r.EventId == eventId).FirstOrDefaultAsync();

        public async Task CreateAsync(ReglaEvento rule) =>
            await _eventRules.InsertOneAsync(rule);

        public async Task UpdateAsync(string id, ReglaEvento rule)
        {
            rule.UpdatedAt = DateTime.UtcNow;
            await _eventRules.ReplaceOneAsync(r => r.Id == id, rule);
        }

        public async Task DeleteByEventIdAsync(string eventId) =>
            await _eventRules.DeleteOneAsync(r => r.EventId == eventId);
    }
}

