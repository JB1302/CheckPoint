using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class PerfilService
    {
        private readonly IMongoCollection<Perfil> _profiles;

        public PerfilService(ContextoMongoDb context) => _profiles = context.Profiles;

        public async Task<Perfil?> GetByUserIdAsync(string userId) =>
            await _profiles.Find(p => p.UserId == userId).FirstOrDefaultAsync();

        public async Task<Perfil?> GetByIdAsync(string id) =>
            await _profiles.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Perfil profile) =>
            await _profiles.InsertOneAsync(profile);

        public async Task UpdateAsync(string id, Perfil profile)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            await _profiles.ReplaceOneAsync(p => p.Id == id, profile);
        }
    }
}

