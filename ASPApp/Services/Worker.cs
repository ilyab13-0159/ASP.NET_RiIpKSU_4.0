using ASPApp.DTO;
using ASPApp.Services.Interfaces;
using Microsoft.CodeAnalysis.CSharp;
using SQLitePCL;
using System.Collections.Generic;
using System.Threading;

namespace ASPApp.Services
{
    public class Worker : BackgroundService
    {

        private IAuthorService _authorService;
        private IGameService _gameService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Worker> _logger;

        #region private static fields (test data)
        private static readonly List<string> _country = new() { "Russia", "USA", "Japan", "German", "Mexico", "Spain" };
        private static readonly List<string> _actions = new() { "insert", "update", "delete" };
        private static readonly List<string> _authors = new() { "Ubisoft", "EA", "Valve", "2K Games", "Sony", "Konami", "Nintendo", "Sega" };
        private static readonly List<string> _games = new() { "Dota 2", "Warcraft 3", "Watch Dogs 2", "UFC 5", "FIFA2k23", "NBA2k23", "Grand Turismo 7", "Devil May Cry 5", "Mass Effect Remastered" };
        #endregion
        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        private async Task DoWork(CancellationToken cancellationToken)
        {
            bool authorAction = false;
            await using var scope = _serviceProvider.CreateAsyncScope();
            _authorService = scope.ServiceProvider.GetService<IAuthorService>();
            _gameService = scope.ServiceProvider.GetService<IGameService>();
            while (!cancellationToken.IsCancellationRequested)
            {
                var action = _actions[Random.Shared.Next(_actions.Count)];
                _logger.LogInformation($"action : {action}");
                switch (action)
                {
                    case "insert":
                        if (authorAction) await InsertAuthor();
                        else await InsertGame();
                        break;
                    case "update":
                        if (authorAction) await UpdateAuthor();
                        else await UpdateGame();
                        break;
                    case "delete":
                        if (authorAction) await DeleteAuthor();
                        else await DeleteGame();
                        break;
                }

                authorAction = !authorAction;
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }

        }
        private async Task InsertAuthor()
        {
            var authors = await _authorService.GetAuthorsAsync();
            //var freeAuthors = authors.Where(a => !_authors.Contains(a.Name)).ToList();
            var freeAuthors = _authors.Where(a => !authors.Exists(g => g.Name.Equals(a))).ToList();
            if (freeAuthors.Count() == 0)
            {
                await DeleteAuthor();
                return;
            }
            var author = new AuthorDTO
            {
                Name = freeAuthors[Random.Shared.Next(freeAuthors.Count())],
                Country = _country[Random.Shared.Next(_country.Count)],
                CreateDate = RandomDay()
            };
            await _authorService.CreateAuthorAsync(author);
        }

        private async Task InsertGame()
        {
            var games = await _gameService.GetGamesAsync();
            var authors = await _authorService.GetAuthorsAsync();
            if (authors.Count == 0)
            {
                await InsertAuthor();
                authors = await _authorService.GetAuthorsAsync();
            }

            //var freeGames = games.Where(a => !_games.Contains(a.Name)).ToList();
            var freeGames = _games.Where(g => !games.Exists(a => a.Name.Equals(g))).ToList();
            if (freeGames.Count() == 0)
            {
                await DeleteGame();
                return;
            }
            var game = new GameDTO()
            {
                Name = freeGames[Random.Shared.Next(freeGames.Count())],
                Author = authors[Random.Shared.Next(authors.Count)].Id.ToString(),
            };
            await _gameService.CreateGameAsync(game);
        }

        private async Task DeleteGame()
        {
            var games = await _gameService.GetGamesAsync();
            if (games.Count == 0)
            {
                await InsertGame();
                return;
            }
            await _gameService.DeleteGameAsync(games[Random.Shared.Next(games.Count)].Id);
        }
        private async Task DeleteAuthor()
        {
            var authors = await _authorService.GetAuthorsAsync();
            if (authors.Count == 0)
            {
                await InsertAuthor();
                return;
            }
            await _authorService.DeleteAuthorAsync(authors[Random.Shared.Next(authors.Count)].Id);
        }

        private async Task UpdateAuthor()
        {
            var authors = await _authorService.GetAuthorsAsync();
            if (authors.Count == 0)
            {
                await InsertAuthor();
                return;
            }
        }

        private async Task UpdateGame()
        {
            var games = await _gameService.GetGamesAsync();
            if (games.Count == 0)
            {
                await InsertGame();
                return;
            }
            var authors = await _authorService.GetAuthorsAsync();
            if (authors.Count == 0)
            {
                await InsertAuthor();
                authors = await _authorService.GetAuthorsAsync();
            }
            var freeGames = games.Where(a => _games.Contains(a.Name)).ToList();
            var game = new GameDTO()
            {
                Name = freeGames[Random.Shared.Next(freeGames.Count)].Name,
                Author = authors[Random.Shared.Next(authors.Count)].Id.ToString(),
            };
            var id = games[Random.Shared.Next(games.Count)].Id;
            await _gameService.UpdateGameAsync(id, game);
        }

        private static DateTime RandomDay()
        {
            Random gen = new();
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWork(stoppingToken);
        }
    }
}
