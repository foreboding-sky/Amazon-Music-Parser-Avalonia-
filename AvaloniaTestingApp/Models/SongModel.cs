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
        public string Artist { get; set; }
        public string Album { get; set; }
        public Avalonia.Media.Imaging.Bitmap Image { get; set; }
        public TimeOnly Duration { get; set; }
        public string DurationString
        {
            get 
            { 
                return Duration.Minute.ToString() + ":" + Duration.Second.ToString(); 
            }
        }
    }
}
