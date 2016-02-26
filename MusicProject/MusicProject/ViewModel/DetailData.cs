using MusicProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicProject.ViewModels
{
    public class DetailData
    {
        public IEnumerable<Song> Songs { get; set; }
        public IEnumerable<Album> Albums { get; set; }
    }
}