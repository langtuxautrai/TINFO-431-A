using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MusicProject.Models
{
    public class Composer
    {
        [Key]
        public int ComposerID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Display(Name = "First Name")]
        public string Fname { get; set; }

        [StringLength(50, MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string Lname { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return Fname + " " + Lname; }
        }

        public string Genres { get; set; }

        [Display(Name = "Works for")]
        [ForeignKey("Companies")]
        public int? CompanyID { get; set; } //Foreign key
        public virtual Company Companies { get; set; }  //Navigation

        public string Rewards { get; set; }

        public ICollection<Song> Songs { get; set; }
    }
}