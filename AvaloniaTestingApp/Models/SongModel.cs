using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTestingApp.Models
{
    public class SongModel
    {
        public string Name { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string Image { get; set; }
        public TimeOnly Duration { get; set; }
    }
}
