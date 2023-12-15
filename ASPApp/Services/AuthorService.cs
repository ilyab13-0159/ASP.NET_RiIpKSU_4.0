using ASPApp.DTO;
using ASPApp.Models.Entity;
using ASPApp.Providers;
using ASPApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ASPApp.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly DBProvider _context;
        private readonly ILogger<AuthorService> _logger;
        public AuthorService(DBProvider context, ILogger<AuthorService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<List<AuthorDTO>> GetAuthorsAsync()
        {
            var authors = await _context.Authors.ToListAsync();
            return authors.Select(a => new AuthorDTO { Id = a.Id, Name = a.Name, Country = a.Country, CreateDate = a.CreateDate }).ToList();
        }

        public async Task<AuthorDTO> CreateAuthorAsync(AuthorDTO authorDTO)
        {
            
            var search = _context.Authors.Any(a => a.Id == authorDTO.Id | a.Name == authorDTO.Name);
            if (search) throw new Exception("Автор с таким идентификатором или именем существует");
            var author = new Author { Id = authorDTO.Id, Name = authorDTO.Name, Country = authorDTO.Country, CreateDate = authorDTO.CreateDate };
            _logger.LogInformation($"Create new author: Name - {authorDTO.Name}, Country - {authorDTO.Country}, CreateDate - {authorDTO.CreateDate}");
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
            return authorDTO;
        }

        public async Task<AuthorDTO> DeleteAuthorAsync(int id)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author == null) throw new Exception("Такого автора не существует");
            _logger.LogInformation($"Delete author: Name - {author.Name}, Country - {author.Country}, CreateDate - {author.CreateDate}, Id - {author.Id}");
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return new AuthorDTO {Id = author.Id, Name = author.Name, Country = author.Country, CreateDate = author.CreateDate };
        }

        public async Task<AuthorDTO> GetAuthorAsync(int id)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author == null) return null;
            return new AuthorDTO {Id = author.Id, Name = author.Name, Country = author.Country, CreateDate = author.CreateDate };
        }

        public async Task<AuthorDTO> UpdateAuthorAsync(int id, AuthorDTO authorDTO)
        {
            var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author == null) throw new Exception("Такого автора не существует");
            _logger.LogInformation($"Update author. Old data: Name - {author.Name}, Country - {author.Country}, CreateDate - {author.CreateDate}, Id - {author.Id}");
            author.Id = authorDTO.Id;
            author.Name = authorDTO.Name;
            author.Country = authorDTO.Country;
            author.CreateDate = authorDTO.CreateDate;
            _logger.LogInformation($"Update author. New data: Name - {author.Name}, Country - {author.Country}, CreateDate - {author.CreateDate}, Id - {author.Id}");
            await _context.SaveChangesAsync();
            return authorDTO;
        }

        public bool AuthorExist(int id)
        {
            var search = _context.Authors.Any(a => a.Id == id);
            return search;
        }
    }
}
