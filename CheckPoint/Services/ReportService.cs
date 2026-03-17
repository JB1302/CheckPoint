using CheckPoint.Models.Reports;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class ReportService
    {
        private readonly IMongoCollection<Report> _reports;

        public ReportService(ContextoMongoDb context)
        {
            _reports = context.Reports;
        }

        public async Task<List<Report>> GetAllAsync()
        {
            return await _reports.Find(_ => true)
                                 .SortByDescending(r => r.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<List<Report>> GetPendingAsync()
        {
            return await _reports.Find(r => r.Status == "Pending")
                                 .SortByDescending(r => r.CreatedAt)
                                 .ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(string id)
        {
            return await _reports.Find(r => r.Id == id)
                                 .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Report report)
        {
            report.CreatedAt = DateTime.UtcNow;
            await _reports.InsertOneAsync(report);
        }

        public async Task UpdateStatusAsync(string id, string status)
        {
            var update = Builders<Report>.Update
                .Set(r => r.Status, status);

            await _reports.UpdateOneAsync(r => r.Id == id, update);
        }
    }
}