using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class PublicacionService
    {
        private readonly IMongoCollection<Publicacion> _posts;

        public PublicacionService(ContextoMongoDb context) => _posts = context.Posts;

        public async Task<List<Publicacion>> GetByEventIdAsync(string eventId) =>
            await _posts.Find(p => p.EventId == eventId && p.IsActive)
                        .SortByDescending(p => p.CreatedAt)
                        .ToListAsync();

        public async Task<Publicacion?> GetByIdAsync(string id) =>
            await _posts.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Publicacion post) =>
            await _posts.InsertOneAsync(post);

        public async Task UpdateAsync(string id, Publicacion post)
        {
            post.UpdatedAt = DateTime.UtcNow;
            await _posts.ReplaceOneAsync(p => p.Id == id, post);
        }

        public async Task SoftDeleteAsync(string id)
        {
            var update = Builders<Publicacion>.Update
                .Set(p => p.IsActive, false)
                .Set(p => p.UpdatedAt, DateTime.UtcNow);
            await _posts.UpdateOneAsync(p => p.Id == id, update);
        }
    }
}

