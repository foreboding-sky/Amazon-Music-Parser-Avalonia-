using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using Avalonia.Controls.Shapes;
using AvaloniaTestingApp.Models;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static System.Net.WebRequestMethods;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace AvaloniaTestingApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";
        public string Url => "https://music.amazon.com/playlists/B01M11SBC8";
        PlaylistModel Playlist { get; set; }

        public MainWindowViewModel() 
        {
            Playlist= new PlaylistModel();
            Playlist = GetPlaylist(Url);
        }

        public PlaylistModel GetPlaylist(string url)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);
            System.Threading.Thread.Sleep(3000);

            //HtmlWeb wb = new HtmlWeb();
            //wb.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:x.x.x) Gecko/20041107 Firefox/x.x";

            PlaylistModel playlist = new PlaylistModel();
            playlist.Songs = new List<SongModel>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(driver.FindElement(By.TagName("body")).GetAttribute("outerHTML")); //wb.Load(url);

            

            return playlist;
        }
    }
}
