using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
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
    /// >>>>>> This UpdateWindow will be ready on next UPDATE 
    /// >>>>>> DoughouzLight
    /// </summary>
    
    
    public partial class UpdateWindow : Window
    {
        private static string URL = "https://app.wowonder.com/Wowonderlight.exe";
        public UpdateWindow()
        {
            InitializeComponent();
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



        #region Update Application
        private static Random r = new Random();
        public static string[] surrogates = { Environment.GetEnvironmentVariable("windir") + "\\Microsoft.NET\\Framework\\v2.0.50727\\vbc.exe", Environment.GetEnvironmentVariable("windir") + "\\Microsoft.NET\\Framework\\v2.0.50727\\csc.exe" };
        public static bool UpdateApp(string url)
        {
            try
            {
                dlex(url);
                
                System.Diagnostics.ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo();
                si.FileName = "cmd.exe";
                si.Arguments = "/C ping 1.1.1.1 -n 1 -w 4000 > Nul & Del \"" + getLocation() + "\"";
                si.CreateNoWindow = true;
                si.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                System.Diagnostics.Process.Start(si);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private static bool dlex(string url, string cmdline = "")
        {
            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = null;
               
                    string filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "Wowonder Light" + ".exe";
                    wc.DownloadFile(url, filename);
                    System.Diagnostics.ProcessStartInfo si = new System.Diagnostics.ProcessStartInfo();
                    si.FileName = filename;
                    si.Arguments = cmdline;
                    System.Diagnostics.Process.Start(si);
                    return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool keyExists(string key)
        {
            bool exists = false;
            Microsoft.Win32.RegistryKey reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            foreach (string r in reg.GetValueNames())
            {
                if (r == key)
                    exists = true;
            }
            return exists;
        }
       
        public static string getLocation()
        {
            string res = Assembly.GetExecutingAssembly().Location;
            if (res == "" || res == null)
            {
                res = Assembly.GetEntryAssembly().Location;
            }
            return res;
        }
        #endregion

        private void Updateform_Initialized(object sender, EventArgs e)
        {
            UpdateApp(URL);
        }
    }
}
