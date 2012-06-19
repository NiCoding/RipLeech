﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.GData.YouTube;
using Google.GData.Extensions.MediaRss;
using Google.YouTube;
using System.Net;
using System.IO;
using System.Web;
using System.Diagnostics;
using YoutubeExtractor;



namespace TubeRip
{
    public partial class mainpage : Form
    {
        string viddling = "";
        string vidout = "";
        string mp4out = "";
        YouTubeRequestSettings settings = new YouTubeRequestSettings("TubeRip", "AI39si5SxBrG0x12TqlsrGpnsoCcqgV9-diBgRS5xOhcDB01sSH6WVSTJjhlQenMSt4qH_UC87Y8kqYaf4Ykgw-poTZ7yF2zDw");
        public mainpage()
        {
            InitializeComponent();
        }

        private void mainpage_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((!String.IsNullOrEmpty(textBox1.Text)) || (textBox1.Text != "Example: qbagLrDTzGY"))
            {
                // Our test youtube link
                string link = "http://www.youtube.com/watch?v=" + textBox1.Text;

                /*
                 * Get the available video formats.
                 * We'll work with them in the video and audio download examples.
                 */
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(link);
                try
                {
                    VideoInfo video = videoInfos.First(info => info.VideoFormat == VideoFormat.HighDefinition720);
                    mp4out = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\" + video.Title + ".mp4";
                    label11.Text = video.Title;
                    label17.Text = "720p";
                    WebClient client = new WebClient();
                    string saveto = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\" + video.Title + video.VideoExtension;
                    vidout = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\" + video.Title + ".mp3";
                    viddling = saveto;
                    client.Encoding = System.Text.Encoding.UTF8;
                    Uri update = new Uri(video.DownloadUrl);
                    client.DownloadFileAsync(update, saveto);
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                }
                catch
                {
                    VideoInfo video = videoInfos.First(info => info.VideoFormat == VideoFormat.Standard360);
                    mp4out = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\" + video.Title + ".mp4";
                    label11.Text = video.Title;
                    label17.Text = "360p";
                    WebClient client = new WebClient();
                    string saveto = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos) + @"\" + video.Title + video.VideoExtension;
                    vidout = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\" + video.Title + ".mp3";
                    viddling = saveto;
                    client.Encoding = System.Text.Encoding.UTF8;
                    Uri update = new Uri(video.DownloadUrl);
                    client.DownloadFileAsync(update, saveto);
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                }
            }
            else
            {
                MessageBox.Show("Please Enter a valid Video ID before trying to download the video!");
            }
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                if (!viddling.Contains("flv"))
                {
                    label1.Text = "Status: Ready";
                }
                else
                {
                    this.Cursor = Cursors.WaitCursor;
                    ProcessStartInfo psi = new ProcessStartInfo();
                    string startdir = Application.StartupPath;
                    if (Environment.Is64BitOperatingSystem)
                    {
                        psi.FileName = startdir + @"\Data\ffmpeg-64.exe";
                    }
                    else
                    {
                        psi.FileName = startdir + @"\Data\ffmpeg-32.exe";
                    }
                    string convert = viddling;
                    psi.Arguments = string.Format("-i \"{0}\" -y -sameq -ar 22050 \"{1}\"", viddling, mp4out);
                    psi.WindowStyle = ProcessWindowStyle.Normal;
                    Process p = Process.Start(psi);
                    p.WaitForExit();
                    if (p.HasExited == true)
                    {
                        this.Cursor = Cursors.Default;
                        File.Delete(viddling);
                    }
                }
            }
            else if (radioButton2.Checked == true)
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                string startdir = Application.StartupPath;
                if (Environment.Is64BitOperatingSystem)
                {
                    psi.FileName = startdir + @"\Data\ffmpeg-64.exe";
                }
                else
                {
                    psi.FileName = startdir + @"\Data\ffmpeg-32.exe";
                }
                string convert = viddling;
                string bitrate = TubeRip.Properties.Settings.Default.audioquality;
                psi.Arguments = string.Format("-i \"{0}\" -vn -y -f mp3 -ab {2}k \"{1}\"", viddling, vidout, bitrate);
                psi.WindowStyle = ProcessWindowStyle.Normal;
                Process p = Process.Start(psi);
                p.WaitForExit();
                if (p.HasExited == true)
                {
                    //this.Cursor = Cursors.Default;
                    File.Delete(viddling);
                    //MessageBox.Show("All Done!");
                }
            }
            else
            {

            }
            if (timer1.Enabled == true)
            {
                timer1.Stop();
                timer1.Enabled = false;
            }
            label1.Text = "Status: Complete";
            progressBar1.Value = 0;
            button4.Visible = true;
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            /*if (timer1.Enabled == false)
            {
                timer1.Enabled = true;
                timer1.Start();
            }*/
            long dlsize = e.TotalBytesToReceive / 1024 / 1024;
            long dldone = e.BytesReceived / 1024 / 1024;
            label1.Text = "Status: Downloading";
            label12.Text = Convert.ToString(dlsize) + "MB";
            label13.Text = "";
            label14.Text = "";
            label15.Text = Convert.ToString(dldone) +  "MB (" + e.ProgressPercentage.ToString() + "%)";
            progressBar1.Value = e.ProgressPercentage;
        }

        public void printVideoEntry(Video video)
        {
            textBox6.Text = video.Description;
            if (video.Media != null && video.Media.Rating != null)
            {
                textBox6.Text = video.Description + "\r\n Restricted in: " + video.Media.Rating.Country.ToString();
            }
        }
        private void getstuff(string videoid)
        {
            webBrowser1.Navigate("http://youtube.com/v/" + videoid + "&hd=1");
            YouTubeRequest request = new YouTubeRequest(settings);
            try
            {
                Uri videoEntryUrl = new Uri("http://gdata.youtube.com/feeds/api/videos/" + videoid);
                Video video = request.Retrieve<Video>(videoEntryUrl);
                Feed<Video> relatedVideos = request.GetRelatedVideos(video);
                printVideoFeed(relatedVideos);
                printVideoEntry(video);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            label5.Text = "Related Videos:";
            listView1.Items.Clear();
            getstuff(textBox1.Text);
        }
        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            progressBar1.MarqueeAnimationSpeed = 80;
            progressBar1.Style = ProgressBarStyle.Marquee;
        }
        void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            progressBar1.MarqueeAnimationSpeed = 0;
            progressBar1.Style = ProgressBarStyle.Blocks;

            label1.Text = "Status: Ready";
        }
        void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (!webBrowser1.IsBusy)
            {
                progressBar1.MarqueeAnimationSpeed = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;

                label1.Text = "Status: Ready";
            }
        }
        public void printVideoFeed(Feed<Video> feed)
        {
            foreach (Video entry in feed.Entries)
            {
                ListViewItem lvi = new ListViewItem(entry.Title);
                lvi.SubItems.Add(entry.ViewCount.ToString());
                lvi.SubItems.Add(entry.Uploader.ToString());
                lvi.SubItems.Add(entry.VideoId.ToString());
                listView1.Items.Add(lvi);
            }
        }
        private void listView1_ItemActivate(object sender, EventArgs e)
        {
            string focused = listView1.FocusedItem.SubItems[3].Text;
            listView1.Items.Clear();
            getstuff(focused);
            textBox1.Text = focused;
        }

        private void listView1_ItemActivate_1(object sender, EventArgs e)
        {
            string focused = listView1.FocusedItem.SubItems[3].Text;
            listView1.Items.Clear();
            getstuff(focused);
            textBox1.Text = focused;
        }

        private void mainpage_Load(object sender, EventArgs e)
        {
            if (File.Exists("updater.exe"))
            {
                File.Delete("updater.exe");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            label5.Text = "Search Results:"; 
            listView1.Items.Clear();
            YouTubeRequest request = new YouTubeRequest(settings);
            YouTubeQuery query = new YouTubeQuery(YouTubeQuery.DefaultVideoUri);

            if (!String.IsNullOrEmpty(textBox2.Text) || (textBox2.Text != "Enter"))
            {
                AtomCategory category1 = new AtomCategory(textBox2.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category1));
            }
            else if ((textBox3.Text != "") || (textBox3.Text != "Up To"))
            {
                AtomCategory category1 = new AtomCategory(textBox2.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category1));
                AtomCategory category2 = new AtomCategory(textBox3.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category2));
            }
            else if ((textBox4.Text != "") || (textBox4.Text != "4 Search"))
            {
                AtomCategory category1 = new AtomCategory(textBox2.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category1));
                AtomCategory category2 = new AtomCategory(textBox3.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category2));
                AtomCategory category3 = new AtomCategory(textBox4.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category3));
            }
            else if ((textBox5.Text != "") || (textBox5.Text != "Terms Here"))
            {
                AtomCategory category1 = new AtomCategory(textBox2.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category1));
                AtomCategory category2 = new AtomCategory(textBox3.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category2));
                AtomCategory category3 = new AtomCategory(textBox4.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category3));
                AtomCategory category4 = new AtomCategory(textBox5.Text, YouTubeNameTable.KeywordSchema);
                query.Categories.Add(new QueryCategory(category4));
            }
            else
            {
                MessageBox.Show("Please enter at least one search term!");
            }

            Feed<Video> videoFeed = request.Get<Video>(query);
            printVideoFeed(videoFeed);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string dlspeed = Directory.GetCurrentDirectory() + "\\" + viddling;
                FileInfo info = new FileInfo(dlspeed);
                if (info.Length > 0)
                {
                    int length = Convert.ToInt32(info.Length / 1000);
                    //label13.Text = Convert.ToString(speed) + " kb/s";
                }
                else
                {
                    timer1.Stop();
                    label13.Text = "Unable to get current speed";
                }
            }
            catch (Exception dlex)
            {
                MessageBox.Show(dlex.Message);
                timer1.Stop();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                Process.Start(viddling);
            }
            else if (viddling.Contains("flv"))
            {
                Process.Start(mp4out);
            }
            else
            {
                Process.Start(vidout);
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            preferences prefs = new preferences();
            prefs.Show();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void mP4ToMP3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFLV = new OpenFileDialog();
            OFLV.Title = "Choose an MP4 File to Convert!";
            OFLV.Filter = "	MPEG-4 Video|*.mp4";
            DialogResult result = OFLV.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveFileDialog SMP3 = new SaveFileDialog();
                SMP3.Title = "Name the converted MP3 File";
                SMP3.Filter = "MP3 Format Sound|*.mp3";
                DialogResult saveresult = SMP3.ShowDialog();
                if (saveresult == DialogResult.OK)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    string startdir = Application.StartupPath;
                    if (Environment.Is64BitOperatingSystem)
                    {
                        psi.FileName = startdir + @"\Data\ffmpeg-64.exe";
                    }
                    else
                    {
                        psi.FileName = startdir + @"\Data\ffmpeg-32.exe";
                    }
                    string convert = viddling;
                    string bitrate = "320";
                    psi.Arguments = string.Format("-i \"{0}\" -vn -y -f mp3 -ab {2}k \"{1}\"", OFLV.FileName, SMP3.FileName, bitrate);
                    psi.WindowStyle = ProcessWindowStyle.Normal;
                    Process p = Process.Start(psi);
                    p.WaitForExit();
                    if (p.HasExited == true)
                    {
                        MessageBox.Show("Conversion Complete!");
                    }
                }
                else
                {
                    MessageBox.Show("Conversion Canceled!");
                }
            }
            else
            {
                MessageBox.Show("Conversion Canceled!");
            }
        }

        private void fLVToMP4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFLV = new OpenFileDialog();
            OFLV.Title = "Choose a FLV File to Convert!";
            OFLV.Filter = "Flash Video|*.flv";
            DialogResult result = OFLV.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveFileDialog SMP3 = new SaveFileDialog();
                SMP3.Title = "Name the converted MP4 File";
                SMP3.Filter = "MPEG-4 Video|*.mp4";
                DialogResult saveresult = SMP3.ShowDialog();
                if (saveresult == DialogResult.OK)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    string startdir = Application.StartupPath;
                    if (Environment.Is64BitOperatingSystem)
                    {
                        psi.FileName = startdir + @"\Data\ffmpeg-64.exe";
                    }
                    else
                    {
                        psi.FileName = startdir + @"\Data\ffmpeg-32.exe";
                    }
                    string convert = viddling;
                    psi.Arguments = string.Format("-i \"{0}\" -y -sameq -ar 22050 \"{1}\"", OFLV.FileName, SMP3.FileName);
                    psi.WindowStyle = ProcessWindowStyle.Normal;
                    Process p = Process.Start(psi);
                    p.WaitForExit();
                    if (p.HasExited == true)
                    {
                        MessageBox.Show("Conversion Complete!");
                    }
                }
                else
                {
                    MessageBox.Show("Conversion Canceled!");
                }
            }
            else
            {
                MessageBox.Show("Conversion Canceled!");
            }
        }

        private void fLVToMP3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFLV = new OpenFileDialog();
            OFLV.Title = "Choose a FLV File to Convert!";
            OFLV.Filter = "Flash Video|*.flv";
            DialogResult result = OFLV.ShowDialog();
            if (result == DialogResult.OK)
            {
                SaveFileDialog SMP3 = new SaveFileDialog();
                SMP3.Title = "Name the converted MP3 File";
                SMP3.Filter = "MP3 Format Sound|*.m3";
                DialogResult saveresult = SMP3.ShowDialog();
                if (saveresult == DialogResult.OK)
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    string startdir = Application.StartupPath;
                    if (Environment.Is64BitOperatingSystem)
                    {
                        psi.FileName = startdir + @"\Data\ffmpeg-64.exe";
                    }
                    else
                    {
                        psi.FileName = startdir + @"\Data\ffmpeg-32.exe";
                    }
                    string convert = viddling;
                    string bitrate = "320";
                    psi.Arguments = string.Format("-i \"{0}\" -vn -y -f mp3 -ab {2}k \"{1}\"", OFLV.FileName, SMP3.FileName, bitrate);
                    psi.WindowStyle = ProcessWindowStyle.Normal;
                    Process p = Process.Start(psi);
                    p.WaitForExit();
                    if (p.HasExited == true)
                    {
                        MessageBox.Show("Conversion Complete!");
                    }
                }
                else
                {
                    MessageBox.Show("Conversion Canceled!");
                }
            }
            else
            {
                MessageBox.Show("Conversion Canceled!");
            }
        }
    }
}