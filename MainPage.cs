using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;
using System.IO;
using HtmlAgilityPack;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static ПРОТОН.AdministrationForm;
using System.Security.Policy;
using NAudio.Wave;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp.Portable.WebRequest;
using RestSharp.Portable;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Mysqlx;


namespace ПРОТОН
{
    public partial class MainPage : Form
    {
        
  
        public MainPage()
        {
            InitializeComponent();
            CreateUpdatePositionTimer();
        }

        DataTable table = new DataTable();
        public void OpasityInf()
        {

        }

        private async void MainPage_Load(object sender, EventArgs e)
        {
           
           await UpdateTrackList();

        }
        public bool oneClic = true;
        private void LoadAutorizTimer_Tick(object sender, EventArgs e)
        {
            Opacity = 1;
            if (oneClic == true)
            {
                AutorizationFormLoadInformaion autor = new AutorizationFormLoadInformaion(); 
                oneClic = false;
                Opacity = 0.8;
                LoadAutorizTimer.Stop();
                autor.ShowDialog(); // Используем существующий экземпляр формы
            }
        }

        private void MainPage_Activated(object sender, EventArgs e)
        {
            Opacity = 1;
        }

      public async Task UpdateTrackList()
        {
            Search.Visible = false;
            string url = "https://rus.hitmotop.com/genres";
            string selector = "https://rus.hitmotop.com";
            HttpClient client = new HttpClient();
            try
            {
                var response = client.GetAsync(url).Result;
                // Читаем содержимое ответа
                string responseBody = response.Content.ReadAsStringAsync().Result;
                // Создаем объект HtmlDocument и загружаем в него полученное содержимое
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(responseBody);
                // Находим элемент ul с классом album-list
                var albumListNode = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='album-list']");
                // Извлекаем все элементы li внутри этого узла
                var albumItems = albumListNode.SelectNodes(".//li[@class='album-item']");
                foreach (var item in albumItems)
                {
                    var itemw = item.SelectSingleNode(".//span[@class='album-image']/@style")?.GetAttributeValue("style", "");
                    var imageSourceUrl = GetBackgroundImageUrl(itemw);
                    string chec = "http";
                    AlbumTrack albums = new AlbumTrack(this);
                    albums.TrackNameLabel.Text = item.SelectSingleNode(".//span[@class='album-title']")?.InnerText.Trim();
                    albums.TrackScore.Text = item.SelectSingleNode(".//span[@class='album-singer']")?.InnerText.Trim();
                    albums.htmlLabelNonVisible.Text = selector + item.SelectSingleNode(".//a[@class='album-link']")?.Attributes["href"]?.Value;
                    if (imageSourceUrl.Contains(chec))
                    {
                        albums.pictureBox1.ImageLocation = imageSourceUrl;
                    }
                    else
                    {
                        albums.pictureBox1.ImageLocation = "http:" + imageSourceUrl;
                    }
                    albums.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                    Search.Controls.Add(albums);
                    //PopulateTracks();
                   
                }
                Search.Visible = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static string GetBackgroundImageUrl(string style)
        {
            if (string.IsNullOrEmpty(style))
            {
                return "";
            }

            const string prefix = "url('";
            int startIndex = style.IndexOf(prefix) + prefix.Length;
            int endIndex = style.LastIndexOf("')");

            if (startIndex >= 0 && endIndex > startIndex)
            {
                return style.Substring(startIndex, endIndex - startIndex);
            }

            return "";
        }


        public async Task UpdateFlowLayoutPanel(string url)
        {
            Search.Visible = false;
            Search.Controls.Clear();


            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string responseBody = client.GetStringAsync(url).Result;

                    HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.LoadHtml(responseBody);

                    var tracks = htmlDoc.DocumentNode.SelectNodes("//ul[@class='tracks__list']/li");

                    if (tracks == null || tracks.Count == 0)
                    {
                        Console.WriteLine("Не удалось найти элементы 'tracks__list'. Проверьте правильность селектора.");
                        return;
                    }
                      
                    foreach (var track in tracks)
                    {
                        // Извлечение информации о треке
                        var titleElement = track.SelectSingleNode(".//div[@class='track__title']");
                        var titleText = titleElement?.InnerText ?? "(Название отсутствует)";

                        var descriptionElement = track.SelectSingleNode(".//div[@class='track__desc']");
                        var descriptionText = descriptionElement?.InnerText ?? "(Описание отсутствует)";

                        var authorElement = track.SelectSingleNode(".//a[@class='track__download-btn']");
                        var authorHref = authorElement?.Attributes["href"].Value ?? "";


                        var backgroundUrl = "";
                        // Извлечение изображения
                        var imgElement = track.SelectSingleNode(".//div[@class='track__img']");
                        var styleAttribute = imgElement?.Attributes["style"];
                        if (styleAttribute != null && styleAttribute.Value.Contains("background"))
                        {
                            backgroundUrl = styleAttribute.Value.Substring(16, styleAttribute.Value.Length - 19);
                            backgroundUrl = backgroundUrl.Remove(0, 7);
                        }

                        // Извлечение времени
                        var timeElement = track.SelectSingleNode(".//div[@class='track__fulltime']");
                        string timeString = timeElement?.InnerText.Trim() ?? "00:00";
                        string[] parts = timeString.Split(':');
                        int minutes = Int32.Parse(parts[0]);
                        int seconds = Int32.Parse(parts[1]);
                        var totalSeconds = (minutes * 60) + seconds;
                        string Time_and_Sekonds = $"{minutes}:{seconds}";
                        SoundPage musicControl = new SoundPage(this);
                        // Объединение всех значений в одной строке
                        Debug.WriteLine($"{titleText} | {descriptionText} | {backgroundUrl} | {Time_and_Sekonds} | {authorHref}");
                        musicControl.TrackName.Text = titleText.TrimStart();
                        musicControl.CreatorNameList.Text = descriptionText;
                        musicControl.SoundUrlLabel.Text = authorHref;
                        musicControl.trackTime.Text = Time_and_Sekonds+ "мин" ;
                        musicControl.TrackPicture.ImageLocation = backgroundUrl;
                        musicControl.TrackPicture.SizeMode = PictureBoxSizeMode.Zoom;
                        Search.Controls.Add(musicControl);
                    }
                    Search.Visible = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            }

        }
     

        private void button1_Click(object sender, EventArgs e)
        {
            Search.Controls.Clear();
            UpdateTrackList();
        }
        AudioFileReader audio;
        WaveOutEvent waveOut;
       public void OnMusic(string url)
{
    if (waveOut != null)
    {
        if (waveOut.PlaybackState == PlaybackState.Playing)
        {
            waveOut.Stop();
        }

        waveOut.Dispose();
        waveOut = null;
    }

    if (audio != null)
    {
        audio.Dispose();
        audio = null;
    }

    audio = new AudioFileReader(url);
    waveOut = new WaveOutEvent();

    // Получаем текущую длительность аудиофайла
    double durationInMilliseconds = audio.TotalTime.TotalMilliseconds;

    // Устанавливаем максимальное значение трекбара
    siticoneTrackBar1.Maximum = (int)durationInMilliseconds;

    // Устанавливаем начальное положение трекбара на 0
    siticoneTrackBar1.Value = 0;

    waveOut.Init(audio);
    waveOut.Play();

    // Создаем и запускаем таймер для обновления позиции
    CreateUpdatePositionTimer();
    updatePositionTimer.Start();
}



        public void PuauseMusic(bool state) 
        {
            
        }

        private void pauseBox_Click(object sender, EventArgs e)
        {
            if (waveOut != null)
            {
                switch (waveOut.PlaybackState)
                {
                    case PlaybackState.Playing:
                        waveOut.Pause();
                        break;
                    case PlaybackState.Paused:
                        waveOut.Play();
                        break;
                }
            }
        }
        private System.Windows.Forms.Timer updatePositionTimer;
        private void CreateUpdatePositionTimer()
        {
            updatePositionTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000 // Обновляем каждые полсекун
            };
            updatePositionTimer.Tick += UpdatePositionTimer_Tick;
        }

        private void UpdatePositionTimer_Tick(object sender, EventArgs e)
        {
            if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
            {
                // Обновление текущего значения трекбара
                siticoneTrackBar1.Value = (int)audio.CurrentTime.TotalMilliseconds;
            }
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
            {
                int position = siticoneTrackBar1.Value;
                long newPosition = (long)((position / 0.1 ) * audio.TotalTime.TotalMilliseconds);
                audio.Position = newPosition;
            }
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            string url = "path_to_audio_file"; 
            OnMusic(url);
        }

        private async void MusicCharacters_Click(object sender, EventArgs e)
        {
            Search.Controls.Clear();
            PageNameAndRole.Text = "Музыкальные жанры";
          await  UpdateTrackList();
        }

        private void siticoneTrackBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
            {
                int position = siticoneTrackBar1.Value;
                long newPosition = (long)((position / 100.0) * audio.TotalTime.TotalMilliseconds);
                audio.Position = newPosition;
            }
        }

        private  async void emailAuth_ContentChanged(object sender, EventArgs e)
        {
            PageNameAndRole.Text = "Поиск";
            string query = emailAuth.Content;
            SearchM(query);
        }
        /// <summary>
        /// Метод поиска музыки
        /// </summary>
        /// <param name="query"></param>
      public  async void  SearchM(string query)
        {
        // Замените на реальное значение
        Search.Controls.Clear();
     
        // Проверка на пустоту
       
            try
            {
                // URL сайта, на котором находится форма поиска
                string baseUrl = "https://rus.hitmotop.com/search?";

                // Формируем полный URL с параметром q
                string searchUrl = $"{baseUrl}q={query}";

                using (var client = new HttpClient())
                {
                    // Добавляем заголовок User-Agent для эмуляции браузера
                    //client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 Edge/16.16299");

                    // Отправка GET-запроса с параметрами поиска
                    var response = await client.GetAsync(searchUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(content);

                        // Находим все элементы <li> с классом tracks__item
                        var trackItems = htmlDocument.DocumentNode.Descendants("li")
                            .Where(node => node.HasClass("tracks__item") && node.HasClass("track") && node.HasClass("mustoggler"))
                            .ToArray();

                        foreach (var item in trackItems)
                        {
                            // Извлечение ссылки на скачивание
                            var downloadLinkElement = item.Descendants("a").FirstOrDefault(n => n.HasClass("track__download-btn"));
                            string downloadLink = downloadLinkElement?.Attributes["href"]?.Value ?? null;

                            // Извлечение ссылки на изображение
                            var imageElement = item.Descendants("div").FirstOrDefault(n => n.HasClass("track__img"));
                            string backgroundStyle = imageElement?.Attributes["style"]?.Value.Split(new[] { "url('" }, StringSplitOptions.None)[1]?.TrimEnd('\'')?.TrimEnd(')') ?? null;

                            // Извлечение других данных
                            string title = item.Descendants("div").FirstOrDefault(n => n.HasClass("track__title"))?.InnerText.Trim();
                            string artist = item.Descendants("div").FirstOrDefault(n => n.HasClass("track__desc"))?.InnerText.Trim();
                            string duration = item.Descendants("div").FirstOrDefault(n => n.HasClass("track__fulltime"))?.InnerText.Trim();
                            var sound = new SoundPage(this);
                            sound.TrackName.Text = title;
                            sound.TrackPicture.ImageLocation = backgroundStyle;
                            sound.CreatorNameList.Text = artist;
                            sound.SoundUrlLabel.Text = downloadLink;
                            sound.trackTime.Text = duration;
                            Search.Controls.Add(sound);

                        }
                     
                    }

                    else
                    {
                        Debug.WriteLine($"Ошибка: статус-код {response.StatusCode}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        
        }

        private void emailAuth_ContentChanged_1(object sender, EventArgs e)
        {
         
        }

        private void emailAuth_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                PageNameAndRole.Text = "Поиск";
                string query = emailAuth.Content;
                SearchM(query);
            }
        }

        private void SearchPicture_MouseEnter(object sender, EventArgs e)
        {
            SearchPicture.ImageLocation = "SearchRed.png";
        }

        private void SearchPicture_MouseLeave(object sender, EventArgs e)
        {
            SearchPicture.ImageLocation = "Search.png";
        }

        private async void Top500Button_Click(object sender, EventArgs e)
        {
            string url = "https://rus.hitmotop.com/songs/top-today";
          await  UpdateFlowLayoutPanel(url);
        }

        private async void RadioBtn_Click(object sender, EventArgs e)
        {
       
            string url = "https://rus.hitmotop.com/radio";
            await UpdateFlowLayoutPanel(url);
        }
    }
    }
    

    


