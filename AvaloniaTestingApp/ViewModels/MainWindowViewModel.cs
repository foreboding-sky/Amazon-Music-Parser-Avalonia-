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
        public string Url => "https://music.amazon.com/playlists/B01M11SBC8";
        PlaylistModel _playlistModel;
        PlaylistModel Playlist
        { 
            get => _playlistModel;
            set => this.RaiseAndSetIfChanged(ref _playlistModel, value);
        }

        public MainWindowViewModel() 
        {
            Playlist= new PlaylistModel();
            //Playlist = GetPlaylist(Url);
        }

        public void GetPlaylist()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl(Url);
            System.Threading.Thread.Sleep(3000); //wait for website to load

            PlaylistModel playlist = new PlaylistModel();
            playlist.Songs = new List<SongModel>();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(driver.FindElement(By.TagName("body")).GetAttribute("outerHTML"));

            var shadowHost = driver.FindElement(By.XPath("//*[@id=\"root\"]/music-app/div[3]/div/div/div/music-detail-header"));
            var shadowRoot = shadowHost.GetShadowRoot();
            var shadowContent = shadowRoot.FindElement(By.ClassName("container"));
            string[] tmp = shadowContent.Text.Split("\n");
            playlist.Name = tmp[1].Split("\r")[0];
            playlist.Description = tmp[2].Split("\r")[0];

            WebClient wc = new WebClient();
            byte[] imgBytes = wc.DownloadData(shadowHost.GetAttribute("image-src").ToString());
            Stream stream = new MemoryStream(imgBytes);
            playlist.Image = Avalonia.Media.Imaging.Bitmap.DecodeToWidth(stream, 200);

            var nodes = doc.DocumentNode.SelectNodes("//music-container/div/div[2]/div/div/music-image-row");
            foreach (var node in nodes)
            {
                SongModel song = new SongModel();

                song.Name = node.Attributes["primary-text"].Value ?? "";
                song.ArtistName = node.Attributes["secondary-text-1"].Value ?? "";
                song.AlbumName = node.Attributes["secondary-text-2"].Value ?? "";
                song.Image = node.Attributes["image-src"].Value ?? "";

                string[] t = node.InnerHtml.Split("col4");
                string[] t1 = t[1].Split("title=\"");
                string[] t2 = t1[1].Split("\"");
                string[] t3 = t2[0].Split(":");
                song.Duration = new TimeOnly(0, int.Parse(t3[0]), int.Parse(t3[1]));

                //code below returns tag <music-link> without its chindren nodes for some reason
                //string[] timeTemp = node.SelectSingleNode("/div/div[4]/music-link").Attributes["title"].Value.Split(':');
                //song.Duration = new TimeOnly(0, int.Parse(timeTemp[0]), int.Parse(timeTemp[1]));

                playlist.Songs.Add(song);
            }

            Playlist = playlist;

            //First attempt of realisation with HtmlAgilityPack only, can't get shadow-root, cant accest website without web browser
            //
            ////var playlistName = doc.DocumentNode.SelectSingleNode("//*[@id=\"root\"]/music-app/div[3]/div/div/div/music-detail-header//div/header/div[2]/h1");
            //var playlistName = doc.DocumentNode.SelectSingleNode("//div[@class='container']");
            //playlist.Name ??= playlistName.InnerText ?? "";

            ////var playlistDesc = doc.DocumentNode.SelectSingleNode("//*[@id=\"root\"]/music-app/div[3]/div/div/div/music-detail-header//div/header/div[2]/p[1]/music-link/span");
            //var playlistDesc = doc.DocumentNode.SelectSingleNode("//div[@class='container']/header/div[2]/p[1]/music-link/span");
            //playlist.Description ??= playlistDesc.InnerText ?? "";

            ////var playlistImg = doc.DocumentNode.SelectSingleNode("//*[@id=\"root\"]/music-app/div[3]/div/div/div/music-detail-header//div/header/div[1]/music-image//picture/img"); 
            //var playlistImg = doc.DocumentNode.SelectSingleNode("//music-detail-header");
            //playlist.Image ??= playlistImg.Attributes["image-src"].Value ?? "";
        }
    }
}
