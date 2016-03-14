
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MusicProject.Models
{
    public class Artist
    {
        [Key]
        public int ArtistID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Display(Name = "First Name")]
        public string Fname { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        [Display(Name = "Last Name")]
        public string Lname { get; set; }

        [Display(Name = "Artist's Name")]
        public string FullName {
            get { return Fname + " " + Lname; }
        }

        public string Genres { get; set; }

        [Display (Name = "Debut In")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Debut_in { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }        
         
        [Display(Name = "Manages by")]        
        [ForeignKey("Companies")]      
        public int? CompanyID { get; set; }  //Foreign Key
        public virtual Company Companies { get; set; }  //Navigation

        [DataType(DataType.MultilineText)]
        public string Rewards { get; set; }

        [Display(Name = ("Name of File"))]
        public string ImageName { get; set; }

        public byte[] Image { get; set; }

        [Display(Name = "Created or Updated On")]
        public DateTime CreateOrUpdate { get; set; }

        public ICollection<Album> Albums { get; set; }
        public ICollection<Song> Songs { get; set; }
    }
}