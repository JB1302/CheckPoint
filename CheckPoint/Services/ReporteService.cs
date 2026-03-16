using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class ReporteService
    {
        private readonly IMongoCollection<Reporte> _reports;

        public ReporteService(ContextoMongoDb context) => _reports = context.Reports;

        public async Task<List<Reporte>> GetAllAsync() =>
            await _reports.Find(_ => true).SortByDescending(r => r.CreatedAt).ToListAsync();

        public async Task<List<Reporte>> GetPendingAsync() =>
            await _reports.Find(r => r.Status == "Pending").SortByDescending(r => r.CreatedAt).ToListAsync();

        public async Task<Reporte?> GetByIdAsync(string id) =>
            await _reports.Find(r => r.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Reporte report) =>
            await _reports.InsertOneAsync(report);

        public async Task UpdateStatusAsync(string id, string status, string reviewedBy)
        {
            var update = Builders<Reporte>.Update
                .Set(r => r.Status, status)
                .Set(r => r.ReviewedBy, reviewedBy)
                .Set(r => r.UpdatedAt, DateTime.UtcNow);
            await _reports.UpdateOneAsync(r => r.Id == id, update);
        }
    }
}

