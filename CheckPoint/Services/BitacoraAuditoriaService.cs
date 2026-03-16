using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class BitacoraAuditoriaService
    {
        private readonly IMongoCollection<BitacoraAuditoria> _auditLogs;

        public BitacoraAuditoriaService(ContextoMongoDb context) => _auditLogs = context.AuditLogs;

        public async Task LogAsync(string userId, string action, string entityType, string entityId,
            Dictionary<string, string>? oldValues = null,
            Dictionary<string, string>? newValues = null,
            string ipAddress = "",
            string userAgent = "")
        {
            await _auditLogs.InsertOneAsync(new BitacoraAuditoria
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues ?? new(),
                NewValues = newValues ?? new(),
                IpAddress = ipAddress,
                UserAgent = userAgent
            });
        }

        public async Task<List<BitacoraAuditoria>> GetByEntityAsync(string entityType, string entityId) =>
            await _auditLogs.Find(a => a.EntityType == entityType && a.EntityId == entityId)
                            .SortByDescending(a => a.CreatedAt)
                            .ToListAsync();

        public async Task<List<BitacoraAuditoria>> GetByUserIdAsync(string userId) =>
            await _auditLogs.Find(a => a.UserId == userId)
                            .SortByDescending(a => a.CreatedAt)
                            .ToListAsync();
    }
}

