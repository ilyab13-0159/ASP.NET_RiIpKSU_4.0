using ASPApp.DTO;
using ASPApp.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace ASPApp.Services.Interfaces
{
    public interface IAuthorService
    {
        public Task<List<AuthorDTO>> GetAuthorsAsync();

        public Task<AuthorDTO> CreateAuthorAsync(AuthorDTO authorDTO);

        public Task<AuthorDTO> DeleteAuthorAsync(int id);

        public Task<AuthorDTO> GetAuthorAsync(int id);

        public Task<AuthorDTO> UpdateAuthorAsync(int id, AuthorDTO authorDTO);

        public bool AuthorExist(int id);
    }
}
