using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
            StyleChanger();
        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void HeadPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void StyleChanger()
        {
            try
            {
                NameApplication.Text = "About " + Settings.SiteName;
                Color header_hover_border = (Color)ColorConverter.ConvertFromString(Settings.Header_Hover_Border);
                TopPanel.Background = new SolidColorBrush(header_hover_border);
                Rigesterpanel.Background = new SolidColorBrush(header_hover_border); 

                Color Header_Background = (Color)ColorConverter.ConvertFromString(Settings.Header_Background);
                Backroundcolor.Background = new SolidColorBrush(Header_Background);

                string AvatarSplit = Settings.Logo_Url.Split('/').Last();
                var path = Settings.FolderDestination + Settings.SiteName + @"\";
                var LogoImage = path + AvatarSplit;
                if (CashSystem(LogoImage) == "0")
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(Settings.Logo_Url, LogoImage);
                    }
                }

                var Imageuri = new Uri(LogoImage);
                var bitmap = new BitmapImage(Imageuri);
                Logo.Source = bitmap;

            }
            catch { }


        }

        public string CashSystem(string fileName)
        {
            string path = Settings.FolderDestination + @"\\" + Settings.SiteName + "\\";
            if (Directory.Exists(path))
            {
                // Get the file name from the path.
                if (System.IO.File.Exists(fileName))
                { return "1"; }
            }
            return "0";
        }
    }
}
