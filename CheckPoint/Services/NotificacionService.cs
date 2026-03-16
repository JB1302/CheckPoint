using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class NotificacionService
    {
        private readonly IMongoCollection<Notificacion> _notifications;

        public NotificacionService(ContextoMongoDb context) => _notifications = context.Notifications;

        public async Task<List<Notificacion>> GetByUserIdAsync(string userId) =>
            await _notifications.Find(n => n.UserId == userId)
                                .SortByDescending(n => n.CreatedAt)
                                .ToListAsync();

        public async Task<long> CountUnreadAsync(string userId) =>
            await _notifications.CountDocumentsAsync(n => n.UserId == userId && !n.IsRead);

        public async Task CreateAsync(Notificacion notification) =>
            await _notifications.InsertOneAsync(notification);

        public async Task MarkAsReadAsync(string id)
        {
            var update = Builders<Notificacion>.Update.Set(n => n.IsRead, true);
            await _notifications.UpdateOneAsync(n => n.Id == id, update);
        }

        public async Task MarkAllReadAsync(string userId)
        {
            var update = Builders<Notificacion>.Update.Set(n => n.IsRead, true);
            await _notifications.UpdateManyAsync(n => n.UserId == userId && !n.IsRead, update);
        }
    }
}

