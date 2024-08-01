using System.ComponentModel.DataAnnotations;

namespace WebPortal.Dll.Models
{
    public class Genre
    {
        public int GenreId { get; set; }

       
        public string Name { get; set; }

        public List<Song> Songs { get; set; }
    }
}
