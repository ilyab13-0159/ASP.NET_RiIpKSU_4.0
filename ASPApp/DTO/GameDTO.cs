using ASPApp.Models.Entity;
using System.ComponentModel.DataAnnotations;

namespace ASPApp.DTO
{
    public class GameDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Author")]
        public string Author { get; set; }
    }
}
