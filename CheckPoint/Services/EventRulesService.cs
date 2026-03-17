using CheckPoint.Models.EventRules;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class EventRulesService
    {
        private readonly IMongoCollection<EventRules> _eventRules;

        public EventRulesService(ContextoMongoDb context)
        {
            _eventRules = context.EventRules;
        }

        public async Task<EventRules?> GetByEventIdAsync(string eventId)
        {
            return await _eventRules.Find(r => r.EventId == eventId)
                                    .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(EventRules rule)
        {
            await _eventRules.InsertOneAsync(rule);
        }

        public async Task UpdateAsync(string id, EventRules rule)
        {
            await _eventRules.ReplaceOneAsync(r => r.Id == id, rule);
        }

        public async Task DeleteByEventIdAsync(string eventId)
        {
            await _eventRules.DeleteOneAsync(r => r.EventId == eventId);
        }

        public async Task<EventRules?> GetByIdAsync(string id)
        {
            return await _eventRules.Find(r => r.Id == id)
                                    .FirstOrDefaultAsync();
        }
    }
}