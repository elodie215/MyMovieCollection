using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMovies.Models
{
    public class Support : IComparable
    {
        public int SupportID { get; set; }

        [Required(ErrorMessage = "Un nom est obligatoire !")]
        [Display(Name = "Support")]
        [MaxLength(30)]
        public string Name { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }

        public int CompareTo(object obj)
        {
            Support s = (Support)obj;
            return String.Compare(this.Name, s.Name);
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}