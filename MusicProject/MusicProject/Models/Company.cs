using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicProject.Models
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }
        public string Address { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        //[RegularExpression(@"^\+2 \(\d{3}\) \d{3}-\d{4}$", ErrorMessage = "Phone Number must in (###) ###-####")]
        [RegularExpression(@"\(\d{3}\) \d{3} \d{4}$", ErrorMessage = "Phone Number must in (###) ### ####")]
        public string phone { get; set; }
       
        public string Website { get; set; }

        [Display(Name = "Found In")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Found { get; set; }

        public ICollection<Artist> Artists { get; set; }
        public ICollection<Album> Albums { get; set; }
        public ICollection<Composer> Composers { get; set; }
    }
}