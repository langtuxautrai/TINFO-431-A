using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicProject.Models
{
    public class Song
    {
        [Key]
        public int SongID { get; set; }
        [Display(Name = "Song's Title")]
        public string Title { get; set; }
        public string Genres { get; set; }

        [Display(Name = "Sing by")]
        [ForeignKey("Artists")]
        public int ArtistID { get; set; }   //Foreign key
        public virtual Artist Artists { get; set; } //Navigation


        [Display(Name = "Composer")]
        [ForeignKey("Composers")]
        public int? ComposerID { get; set; } //Foreign key
        public virtual Composer Composers { get; set; } //Navigation

        [Display(Name = "In Album")]
        [ForeignKey("Albums")]
        public int? AlbumID { get; set; }    //Foreign key
        public virtual Album Albums { get; set; }   //Navigation

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]        
        public DateTime? Release { get; set; }

        [Display(Name = "Peak position")]
        public string Peak_position { get; set; }
        
        [AllowHtml]
        public string Lyric { get; set; }
    }
}