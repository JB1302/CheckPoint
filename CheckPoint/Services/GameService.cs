using CheckPoint.Models.Games;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class GameService
    {
        private readonly IMongoCollection<Game> _games;

        public GameService(ContextoMongoDb context)
        {
            _games = context.Games;
        }

        public async Task<List<Game>> GetAllActiveAsync()
        {
            return await _games.Find(g => g.IsActive)
                               .SortBy(g => g.Name)
                               .ToListAsync();
        }

        public async Task<Game?> GetByIdAsync(string id)
        {
            return await _games.Find(g => g.Id == id)
                               .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Game game)
        {
            await _games.InsertOneAsync(game);
        }

        public async Task UpdateAsync(string id, Game game)
        {
            await _games.ReplaceOneAsync(g => g.Id == id, game);
        }

        public async Task DeleteAsync(string id)
        {
            await _games.DeleteOneAsync(g => g.Id == id);
        }

        public async Task<List<Game>> GetAllAsync()
        {
            return await _games.Find(_ => true)
                               .SortBy(g => g.Name)
                               .ToListAsync();
        }
    }
}