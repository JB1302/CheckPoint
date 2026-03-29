using CheckPoint.Models.Notifications;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class NotificationService
    {
        private readonly IMongoCollection<Notification> _notifications;

        public NotificationService(ContextoMongoDb context)
        {
            _notifications = context.Notifications;
        }

        public async Task<List<Notification>> GetAllAsync()
        {
            return await _notifications.Find(_ => true).ToListAsync();
        }

        public async Task<List<Notification>> GetByUserIdAsync(string userId)
        {
            return await _notifications.Find(n => n.UserId == userId)
                                       .SortByDescending(n => n.CreatedAt)
                                       .ToListAsync();
        }

        public async Task<long> CountUnreadAsync(string userId)
        {
            return await _notifications.CountDocumentsAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task CreateAsync(Notification notification)
        {
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsRead = false;

            await _notifications.InsertOneAsync(notification);
        }

        public async Task MarkAsReadAsync(string id)
        {
            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
            await _notifications.UpdateOneAsync(n => n.Id == id, update);
        }

        public async Task MarkAllReadAsync(string userId)
        {
            var update = Builders<Notification>.Update.Set(n => n.IsRead, true);
            await _notifications.UpdateManyAsync(n => n.UserId == userId && !n.IsRead, update);
        }

        public async Task<Notification?> GetByIdAsync(string id)
        {
            return await _notifications.Find(n => n.Id == id)
                                       .FirstOrDefaultAsync();
        }
    }
}