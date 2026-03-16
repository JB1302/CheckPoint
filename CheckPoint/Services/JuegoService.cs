using CheckPoint.Models;
using MongoDB.Driver;

namespace CheckPoint.Services
{
    public class JuegoService
    {
        private readonly IMongoCollection<Juego> _games;

        public JuegoService(ContextoMongoDb context) => _games = context.Games;

        public async Task<List<Juego>> GetAllActiveAsync() =>
            await _games.Find(g => g.IsActive).SortBy(g => g.Name).ToListAsync();

        public async Task<Juego?> GetByIdAsync(string id) =>
            await _games.Find(g => g.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Juego game) =>
            await _games.InsertOneAsync(game);

        public async Task UpdateAsync(string id, Juego game) =>
            await _games.ReplaceOneAsync(g => g.Id == id, game);

        public async Task DeleteAsync(string id) =>
            await _games.DeleteOneAsync(g => g.Id == id);
    }
}

