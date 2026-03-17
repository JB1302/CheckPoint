using CheckPoint.Models.AuditLogs;
using CheckPoint.Models.Comments;
using CheckPoint.Models.EventRules;
using CheckPoint.Models.Events;
using CheckPoint.Models.Games;
using CheckPoint.Models.Notifications;
using CheckPoint.Models.Posts;
using CheckPoint.Models.Profiles;
using CheckPoint.Models.Reactions;
using CheckPoint.Models.Registrations;
using CheckPoint.Models.Reports;
using CheckPoint.Models.Users;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    /// <summary>
    /// Registrado como singleton para compartir MongoClient entre solicitudes.
    /// </summary>
    public class ContextoMongoDb
    {
        private readonly IMongoDatabase _db;

        public ContextoMongoDb(IOptions<ConfiguracionMongoDb> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _db = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users =>
            _db.GetCollection<User>("Users");

        public IMongoCollection<Profile> Profiles =>
            _db.GetCollection<Profile>("Profiles");

        public IMongoCollection<Game> Games =>
            _db.GetCollection<Game>("Games");

        public IMongoCollection<Events> Events =>
            _db.GetCollection<Events>("Events");

        public IMongoCollection<EventRules> EventRules =>
            _db.GetCollection<EventRules>("EventRules");

        public IMongoCollection<Registration> Registrations =>
            _db.GetCollection<Registration>("Registrations");

        public IMongoCollection<Post> Posts =>
            _db.GetCollection<Post>("Posts");

        public IMongoCollection<Comment> Comments =>
            _db.GetCollection<Comment>("Comments");

        public IMongoCollection<Reaction> Reactions =>
            _db.GetCollection<Reaction>("Reactions");

        public IMongoCollection<Notification> Notifications =>
            _db.GetCollection<Notification>("Notifications");

        public IMongoCollection<Report> Reports =>
            _db.GetCollection<Report>("Reports");

        public IMongoCollection<AuditLog> AuditLogs =>
            _db.GetCollection<AuditLog>("AuditLogs");
    }
}