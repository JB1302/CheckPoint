using MongoDB.Driver;
using CheckPoint.Models.Comments;

namespace CheckPoint.Services
{
    public class CommentService
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentService(ContextoMongoDb context)
        {
            _comments = context.Comments;
        }

        public async Task<List<Comment>> GetByPostIdAsync(string postId)
        {
            return await _comments.Find(c => c.PostId == postId && !c.IsDeleted)
                                  .SortBy(c => c.CreatedAt)
                                  .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(string id)
        {
            return await _comments.Find(c => c.Id == id)
                                  .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Comment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            comment.IsDeleted = false;

            await _comments.InsertOneAsync(comment);
        }

        public async Task SoftDeleteAsync(string id)
        {
            var update = Builders<Comment>.Update
                .Set(c => c.IsDeleted, true);

            await _comments.UpdateOneAsync(c => c.Id == id, update);
        }
    }
}