using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MsgPopupWindow.xaml
    /// </summary>
    public partial class MsgPopupWindow : Window
    {
        public MsgPopupWindow()
        {
            InitializeComponent();
            Stylechanger();
            WpfApplication2.MainForm.PopUpModelBinder binder = new WpfApplication2.MainForm.PopUpModelBinder();
            MsgFrom.Text = binder.Messagefrom;
            msgContent.Text = binder.Messagecontent;
            
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
           
            bi3.UriSource = new Uri(binder.UserImage);
            bi3.EndInit();
            profileimage.Stretch = Stretch.Fill;
            profileimage.Source = bi3;
            StartTimer();
        }

        DispatcherTimer timer = null;
        void StartTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += new EventHandler(timer_Elapsed);
            timer.Start();
        }
        void timer_Elapsed(object sender, EventArgs e)
        {
            timer.Stop();
            PopUpform.Close();
           
        }

        public void Stylechanger()
        {
            Color BgColor = (Color)ColorConverter.ConvertFromString(Settings.PopUpBackroundColor);
            Color FrColor = (Color)ColorConverter.ConvertFromString(Settings.PopUpTextFromcolor);
            Color FrmsgColor = (Color)ColorConverter.ConvertFromString(Settings.PopUpMsgTextcolor);

            Backround.Background = new SolidColorBrush(BgColor);
            MsgFrom.Foreground = new SolidColorBrush(FrColor);
            msgContent.Foreground = new SolidColorBrush(FrmsgColor);
        }
       
    }
}
