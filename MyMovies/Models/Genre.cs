using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMovies.Models
{
    public class Genre : IComparable
    {
        public int GenreID { get; set; }

        [Required(ErrorMessage = "Un nom est obligatoire !")]
        [Display(Name = "Genre")]
        [MaxLength(30)]
        public string Name { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }

        public int CompareTo(object obj)
        {
            Genre g = (Genre)obj;
            return String.Compare(this.Name, g.Name);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}