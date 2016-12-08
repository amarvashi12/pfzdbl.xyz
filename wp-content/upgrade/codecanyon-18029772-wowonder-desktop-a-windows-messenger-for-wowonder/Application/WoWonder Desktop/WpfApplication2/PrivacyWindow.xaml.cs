using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for PrivacyWindow.xaml
    /// </summary>
    public partial class PrivacyWindow : Window
    {
        public PrivacyWindow()
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
            NameApplication.Text = Settings.SiteName;
            Color Header_Background = (Color)ColorConverter.ConvertFromString(Settings.Header_Background);
            TopPanel.Background = new SolidColorBrush(Header_Background);
            Color header_color = (Color)ColorConverter.ConvertFromString(Settings.Header_Color);
            NameApplication.Foreground = new SolidColorBrush(header_color);
           
            Color Btn_Background_Color = (Color)ColorConverter.ConvertFromString(Settings.Btn_Background_Color);     

            Color chat_outgoing_background = (Color)ColorConverter.ConvertFromString(Settings.Chat_Outgoing_Background);
           
        }
    }
}
