using CheckPoint.Models.AuditLogs;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace CheckPoint.Services
{
    public class AuditLogService
    {
        private readonly IMongoCollection<AuditLog> _auditLogs;

        public AuditLogService(ContextoMongoDb context)
        {
            _auditLogs = context.AuditLogs;
        }

        public async Task LogAsync(
            string userId,
            string action,
            string entityType,
            string entityId,
            Dictionary<string, string>? oldValues = null,
            Dictionary<string, string>? newValues = null,
            string ipAddress = "",
            string userAgent = "")
                {
                    var metadata = new BsonDocument
            {
                { "oldValues", oldValues != null ? new BsonDocument(oldValues) : BsonNull.Value },
                { "newValues", newValues != null ? new BsonDocument(newValues) : BsonNull.Value },
                { "ipAddress", ipAddress ?? string.Empty },
                { "userAgent", userAgent ?? string.Empty }
            };

                    await _auditLogs.InsertOneAsync(new AuditLog
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = userId,
                        Action = action,
                        EntityType = entityType,
                        EntityId = entityId,
                        CreatedAt = DateTime.UtcNow,
                        Metadata = metadata
                    });
                }

        public async Task<List<AuditLog>> GetAllAsync()
        {
            return await _auditLogs.Find(_ => true)
                                   .SortByDescending(a => a.CreatedAt)
                                   .ToListAsync();
        }

        public async Task<AuditLog?> GetByIdAsync(string id)
        {
            return await _auditLogs.Find(a => a.Id == id)
                                   .FirstOrDefaultAsync();
        }

        public async Task<List<AuditLog>> GetByEntityAsync(string entityType, string entityId)
        {
            return await _auditLogs.Find(a => a.EntityType == entityType && a.EntityId == entityId)
                                   .SortByDescending(a => a.CreatedAt)
                                   .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByUserIdAsync(string userId)
        {
            return await _auditLogs.Find(a => a.UserId == userId)
                                   .SortByDescending(a => a.CreatedAt)
                                   .ToListAsync();
        }
    }
}