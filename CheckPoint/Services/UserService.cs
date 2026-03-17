using CheckPoint.Models;
using CheckPoint.Models.Users;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(ContextoMongoDb context)
        {
            _users = context.Users;
        }

        public async Task<List<User>> GetAllAsync() =>
            await _users.Find(_ => true).ToListAsync();

        public async Task<User?> GetByIdAsync(string id) =>
            await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<User?> GetByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task CreateAsync(User user) =>
            await _users.InsertOneAsync(user);

        public async Task UpdateAsync(string id, User user) =>
            await _users.ReplaceOneAsync(u => u.Id == id, user);

        public async Task<bool> ExistsByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).AnyAsync();

        public async Task<bool> ExistsByUsernameAsync(string username) =>
            await _users.Find(u => u.Username == username).AnyAsync();
    }
}