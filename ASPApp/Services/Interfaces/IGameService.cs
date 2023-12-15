using ASPApp.DTO;
using ASPApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace ASPApp.Services.Interfaces
{
    public interface IGameService
    {
        public Task<List<GameDTO>> GetGamesAsync();

        public Task<GameDTO> CreateGameAsync(GameDTO gameDTO);

        public Task<GameDTO> DeleteGameAsync(int id);

        public Task<GameDTO> GetGameAsync(int id);

        public Task<GameDTO> UpdateGameAsync(int id, GameDTO gameDTO);

        public bool GameExist(int id);

        public Task<List<Author>> GetAuthors();
    }
}
