using CheckPoint.Models.Profiles;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class ProfileService
    {
        private readonly IMongoCollection<Profile> _profiles;

        public ProfileService(ContextoMongoDb context)
        {
            _profiles = context.Profiles;
        }

        public async Task<Profile?> GetByUserIdAsync(string userId)
        {
            return await _profiles.Find(p => p.UserId == userId)
                                  .FirstOrDefaultAsync();
        }

        public async Task<Profile?> GetByIdAsync(string id)
        {
            return await _profiles.Find(p => p.Id == id)
                                  .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Profile profile)
        {
            await _profiles.InsertOneAsync(profile);
        }

        public async Task<List<Profile>> GetAllAsync() =>
            await _profiles.Find(_ => true).ToListAsync();

        public async Task UpdateAsync(string id, Profile profile)
        {
            await _profiles.ReplaceOneAsync(p => p.Id == id, profile);
        }

        public async Task DeleteAsync(string id) =>
            await _profiles.DeleteOneAsync(p => p.Id == id);
    }
}