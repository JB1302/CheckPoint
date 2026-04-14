using CheckPoint.Models.Posts;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class PostService
    {
        private readonly IMongoCollection<Post> _posts;
        private readonly CommentService _commentService;

        public PostService(ContextoMongoDb context, CommentService commentService)
        {
            _posts = context.Posts;
            _commentService = commentService;
        }

        public async Task<List<Post>> GetByEventIdAsync(string eventId)
        {
            var posts = await _posts.Find(p => p.EventId == eventId)
                                   .SortByDescending(p => p.CreatedAt)
                                   .ToListAsync();

            foreach (var post in posts)
            {
                post.Comments = await _commentService.GetByPostIdAsync(post.Id) ?? new List<CheckPoint.Models.Comments.Comment>();
            }

            return posts;
        }

        public async Task<Post?> GetByIdAsync(string id)
        {
            var post = await _posts.Find(p => p.Id == id)
                                   .FirstOrDefaultAsync();

            if (post != null)
            {
                post.Comments = await _commentService.GetByPostIdAsync(post.Id) ?? new List<CheckPoint.Models.Comments.Comment>();
            }

            return post;
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

        public async Task<List<Post>> GetAllAsync()
        {
            var posts = await _posts.Find(_ => true)
                                   .SortByDescending(p => p.CreatedAt)
                                   .ToListAsync();

            foreach (var post in posts)
            {
                post.Comments = await _commentService.GetByPostIdAsync(post.Id) ?? new List<CheckPoint.Models.Comments.Comment>();
            }

            return posts;
        }
    }
}