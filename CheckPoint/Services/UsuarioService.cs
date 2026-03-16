using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class UsuarioService
    {
        private readonly IMongoCollection<Usuario> _users;

        public UsuarioService(ContextoMongoDb context) => _users = context.Users;

        public async Task<List<Usuario>> GetAllAsync() =>
            await _users.Find(_ => true).ToListAsync();

        public async Task<Usuario?> GetByIdAsync(string id) =>
            await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

        public async Task<Usuario?> GetByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

        public async Task<Usuario?> GetByUsernameAsync(string username) =>
            await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task CreateAsync(Usuario user) =>
            await _users.InsertOneAsync(user);

        public async Task UpdateAsync(string id, Usuario user) =>
            await _users.ReplaceOneAsync(u => u.Id == id, user);

        public async Task<bool> ExistsByEmailAsync(string email) =>
            await _users.Find(u => u.Email == email).AnyAsync();

        public async Task<bool> ExistsByUsernameAsync(string username) =>
            await _users.Find(u => u.Username == username).AnyAsync();
    }
}

