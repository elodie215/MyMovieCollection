using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyMovies.Models
{
    public class Movie
    {
        public int MovieID { get; set; }

        [Required(ErrorMessage = "Un titre est obligatoire !")]
        [Display(Name = "Titre")]
        [MaxLength(70)]
        public string Title { get; set;  }

        [Display(Name = "Note")]
        [Range(0,10,ErrorMessage = "La note doit être entre 0 et 10")]
        public int Rating { get; set; }

        [Display(Name = "Affiche")]
        public string Cover { get; set; }

        [Display(Name = "Bande-Annonce")]
        public string VideoID { get; set; }

        [Display(Name = "Genre")]
        public int GenreID { get; set; }

        public virtual Genre Genre { get; set; }

        [Display(Name = "Support")]
        public int SupportID { get; set; }

        public virtual Support Support { get; set; }

        public int UserId { get; set; }

        public virtual UserProfile User { get; set; }
    }
}