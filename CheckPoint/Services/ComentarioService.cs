using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class ComentarioService
    {
        private readonly IMongoCollection<Comentario> _comments;

        public ComentarioService(ContextoMongoDb context) => _comments = context.Comments;

        public async Task<List<Comentario>> GetByPostIdAsync(string postId) =>
            await _comments.Find(c => c.PostId == postId && c.IsActive)
                           .SortBy(c => c.CreatedAt)
                           .ToListAsync();

        public async Task<Comentario?> GetByIdAsync(string id) =>
            await _comments.Find(c => c.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Comentario comment) =>
            await _comments.InsertOneAsync(comment);

        public async Task SoftDeleteAsync(string id)
        {
            var update = Builders<Comentario>.Update
                .Set(c => c.IsActive, false)
                .Set(c => c.UpdatedAt, DateTime.UtcNow);
            await _comments.UpdateOneAsync(c => c.Id == id, update);
        }
    }
}

