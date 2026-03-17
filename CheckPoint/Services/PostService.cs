using CheckPoint.Models.Posts;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class PostService
    {
        private readonly IMongoCollection<Post> _posts;

        public PostService(ContextoMongoDb context)
        {
            _posts = context.Posts;
        }

        public async Task<List<Post>> GetByEventIdAsync(string eventId)
        {
            return await _posts.Find(p => p.EventId == eventId)
                               .SortByDescending(p => p.CreatedAt)
                               .ToListAsync();
        }

        public async Task<Post?> GetByIdAsync(string id)
        {
            return await _posts.Find(p => p.Id == id)
                               .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Post post)
        {
            post.CreatedAt = DateTime.UtcNow;
            await _posts.InsertOneAsync(post);
        }

        public async Task UpdateAsync(string id, Post post)
        {
            post.UpdatedAt = DateTime.UtcNow;
            await _posts.ReplaceOneAsync(p => p.Id == id, post);
        }

        public async Task DeleteAsync(string id)
        {
            await _posts.DeleteOneAsync(p => p.Id == id);
        }
    }
}