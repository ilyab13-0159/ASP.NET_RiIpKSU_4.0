using ASPApp.DTO;
using ASPApp.Models.Entity;
using ASPApp.Providers;
using ASPApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPApp.Services
{
    public class GameService : IGameService
    {
        private readonly DBProvider _context;
        private readonly ILogger<GameService> _logger;
        public GameService(DBProvider context, ILogger<GameService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<List<GameDTO>> GetGamesAsync()
        {
            var games = await _context.Games.Include(g => g.Author).ToListAsync();
            return games.Select(g => new GameDTO { Id= g.Id, Name = g.Name, Author = g.Author?.Name}).ToList();
        }

        public async Task<GameDTO> CreateGameAsync(GameDTO gameDTO)
        {
            var search = _context.Games.Any(a => a.Id == gameDTO.Id | a.Name == gameDTO.Name);
            if (search) throw new Exception("Игра с таким идентификатором или именем существует");
            var author = _context.Authors.FirstOrDefault(a => a.Id == int.Parse(gameDTO.Author));
            var game = new Game { Id = gameDTO.Id, Name = gameDTO.Name, Author = author};
            _logger.LogInformation($"Create new game: Name - {gameDTO.Name}, Author - {gameDTO.Author}");
            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();
            return gameDTO;
        }

        public async Task<GameDTO> DeleteGameAsync(int id)
        {
            var game = await _context.Games.FirstOrDefaultAsync(a => a.Id == id);
            if (game == null) throw new Exception("Такой игры не существует");
            _logger.LogInformation($"Delete game: Name - {game.Name}, Author - {game.Author}");
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
            return new GameDTO { Id = game.Id, Name = game.Name};
        }

        public async Task<GameDTO> GetGameAsync(int id)
        {
            var game = await _context.Games.Include(g => g.Author).FirstOrDefaultAsync(a => a.Id == id);
            if (game == null) return null;
            return new GameDTO { Id = game.Id, Name = game.Name, Author = game.Author?.Name };
        }

        public async Task<GameDTO> UpdateGameAsync(int id, GameDTO gameDTO)
        {
            var game = await _context.Games.Include(g => g.Author).FirstOrDefaultAsync(a => a.Id == id);
            if (game == null) throw new Exception("Такой игры не существует");
            _logger.LogInformation($"Update game. Old data: Name - {game.Name}, Author - {game.Author?.Name ?? "Empty"}, Id - {game.Id}");
            game.Name = gameDTO.Name;
            var author = _context.Authors.FirstOrDefault(a => a.Id == int.Parse(gameDTO.Author));
            game.Author = author;
            _logger.LogInformation($"Update game. New data: Name - {game.Name}, Author - {game.Author.Name}, Id - {game.Id}");
            await _context.SaveChangesAsync();
            return gameDTO;
        }

        public bool GameExist(int id)
        {
            var search = _context.Games.Any(a => a.Id == id);
            return search;
        }

        public async Task<List<Author>> GetAuthors()
        {
            return await _context.Authors.ToListAsync();
        } 
    }
}
