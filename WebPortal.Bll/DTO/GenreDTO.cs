using WebPortal.Dll.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebPortal.Bll.DTO
{
    public class GenreDTO
    {
        public int GenreId { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
       
    }
}
