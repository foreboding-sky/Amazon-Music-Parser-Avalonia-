using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media.Imaging;
using AvaloniaTestingApp.Models;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using ReactiveUI;
using static System.Net.WebRequestMethods;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace AvaloniaTestingApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string url;
        public string Url
        {
            get => url;
            set => this.RaiseAndSetIfChanged(ref url, value);
        }
        PlaylistModel _playlistModel;
        PlaylistModel Playlist
        { 
            get => _playlistModel;
            set => this.RaiseAndSetIfChanged(ref _playlistModel, value);
        }

        public MainWindowViewModel() 
        {
            Playlist= new PlaylistModel();
            Url = "https://music.amazon.com/playlists/B01M11SBC8";
        }
        public void ParseUrl()
        {
            Playlist = ParseAmazonPlaylist(Url);
        }

        public PlaylistModel ParseAmazonPlaylist(string url)
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(url);
            System.Threading.Thread.Sleep(2000); //wait for website to load

            PlaylistModel playlist = new PlaylistModel();
            playlist.Songs = new List<SongModel>();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(driver.FindElement(By.TagName("body")).GetAttribute("outerHTML"));

            //get playlist data from shadow-root
            var shadowHost = driver.FindElement(By.XPath("//*[@id=\"root\"]/music-app/div[3]/div/div/div/music-detail-header"));
            var shadowRoot = shadowHost.GetShadowRoot();
            var shadowContent = shadowRoot.FindElement(By.ClassName("container"));

            //set up playlist data
            string[] tmp = shadowContent.Text.Split("\n");
            playlist.Name = tmp[1].Split("\r")[0];
            playlist.Description = tmp[2].Split("\r")[0];

            //get playlist image bitmap
            WebClient wc = new WebClient();
            byte[] imgBytes = wc.DownloadData(shadowHost.GetAttribute("image-src").ToString());
            Stream stream = new MemoryStream(imgBytes);
            playlist.Image = Avalonia.Media.Imaging.Bitmap.DecodeToWidth(stream, 200);

            var nodes = doc.DocumentNode.SelectNodes("//music-container/div/div[2]/div/div/music-image-row");
            foreach (var node in nodes)
            {
                SongModel song = new SongModel();

                //set up song data
                song.Name = node.Attributes["primary-text"].Value;
                song.Artist = node.Attributes["secondary-text-1"].Value;
                song.Album = node.Attributes["secondary-text-2"].Value;

                //get song image
                imgBytes = wc.DownloadData(node.Attributes["image-src"].Value);
                stream = new MemoryStream(imgBytes);
                song.Image = Avalonia.Media.Imaging.Bitmap.DecodeToWidth(stream, 50);

                //get song duration
                string[] time = node.InnerHtml.Split("col4")[1].Split("title=\"")[1].Split("\"")[0].Split(":");
                song.Duration = new TimeOnly(0, int.Parse(time[0]), int.Parse(time[1]));

                playlist.Songs.Add(song);
            }
            return playlist;
        }
    }
}
