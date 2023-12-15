using ASPApp.Models.Entity;
using Microsoft.EntityFrameworkCore;
using ASPApp.DTO;

namespace ASPApp.Providers
{
    public class DBProvider : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Game> Games { get; set; }

        public DBProvider(DbContextOptions<DBProvider> options)
            : base(options)
        {
           // Database.EnsureDeleted();
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}
