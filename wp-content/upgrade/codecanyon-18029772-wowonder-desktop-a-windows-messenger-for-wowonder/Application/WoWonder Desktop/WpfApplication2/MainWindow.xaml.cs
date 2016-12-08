using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.Collections;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading;
using System.Net;
using System.Windows.Threading;
using System.IO;
using System.ComponentModel;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static BackgroundWorker GetProfileWorker = new BackgroundWorker();

        public MainWindow()
        {
            InitializeComponent();
            //Adding the Style From the website settings  
            StyleChanger();

            //Downloading Emoji Icons To The AppData Folder
            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => Emogi_Icon_Checker();
            bw.RunWorkerCompleted += (o, args) => Emogi_Icon_Complete();
            bw.RunWorkerAsync();
        }

        #region Controls Events
        private void panelmove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Settings.WebsiteUrl + "/#");
        }

        private void Username_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Username.Text == "Username")
            {
                Username.Text = "";
                Username.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Password.Password == "Password")
            {
                Password.Password = "";
                Password.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void Loginbutton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String timeStamp = GetTimestamp(DateTime.Now);
                Loginbutton.Content = "Please wait..";
                System.Windows.Application.Current.Dispatcher.BeginInvoke(
                   DispatcherPriority.Background,
                   new Action(() =>
                   {
                       using (var client = new WebClient())
                       {

                           //Get and set the time Zone on Pc 
                           var TimeZoneContry = "UTC";
                           var Loader = new NameValueCollection();
                           Loader["s"] = timeStamp;
                           try
                           {
                               var responseTimeZone = client.UploadValues("http://ip-api.com/json/", Loader);
                               var responseTimeZoneString = Encoding.Default.GetString(responseTimeZone);
                               var dictTimeZone = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(responseTimeZoneString);
                               string ResulttimeZone = dictTimeZone["status"].ToString();
                               if (ResulttimeZone == "success")
                                   TimeZoneContry = dictTimeZone["timezone"].ToString();
                           }
                           catch { }
                           

                           //Send all Login credentials to the main server
                           var values = new NameValueCollection();
                           values["username"] = Username.Text;
                           values["password"] = Password.Password;
                           values["s"] = timeStamp;
                           values["timezone"] = TimeZoneContry;

                           //Get respone from the server
                           var response = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=user_login", values);
                           var responseString = Encoding.Default.GetString(response);
                           var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(responseString);
                           string ApiStatus = dict["api_status"].ToString();

                           if (ApiStatus == "200")
                           {
                               errorpanel.Visibility = Visibility.Hidden;
                               if (Keepmechk.IsChecked == true) { string[] lines3 = { "1" }; System.IO.File.WriteAllLines(Settings.KeepMe, lines3); }
                               string user_id = dict["user_id"].ToString();
                               string[] lines2 = { timeStamp };
                               System.IO.File.WriteAllLines(Settings.SessionID, lines2);
                               string[] lines = { user_id };
                               System.IO.File.WriteAllLines(Settings.UserID, lines);
                               Loginbutton.Content = "Loading Users..";
                               //Start Loading
                               GetProfileWorker.WorkerReportsProgress = true;
                               GetProfileWorker.WorkerSupportsCancellation = true;
                               GetProfileWorker.DoWork += GetProfileWorker_DoWork;
                               GetProfileWorker.ProgressChanged += GetProfileWorker_ProgressChanged;
                               GetProfileWorker.RunWorkerCompleted += GetProfileWorker_RunWorkerCompleted;
                               GetProfileWorker.RunWorkerAsync();
                               MainForm Mainform = new MainForm();
                               Mainform.Show();
                               this.Hide();
                           }
                           else if (ApiStatus == "400")
                           {
                               Loginbutton.Content = "Retrying..";
                               JObject errors = JObject.FromObject(dict["errors"]);
                               var errorID = errors["error_id"].ToString();
                               var errortext = errors["error_text"];
                               errorpanel.Visibility = Visibility.Visible;
                               Loginbutton.Content = "Login";

                               if (errorID == "3") { ErrorLable.Text = "Please write your username."; }
                               else if (errorID == "4") { ErrorLable.Text = "Please write your password."; }
                               else if (errorID == "6") { ErrorLable.Text = "Username doesn't exists"; }
                               else if (errorID == "7") { ErrorLable.Text = "Incorrect username or password."; }
                               else if (errorID == "8") { ErrorLable.Text = "Error found, please try again later."; }

                               Loginbutton.Content = "Login";
                           }
                           else
                           { errorpanel.Visibility = Visibility.Visible; ErrorLable.Text = "Cannot Connect"; }


                       }
                   }));
            }
            catch { };
        }

        private void formLogin_ContentRendered(object sender, EventArgs e)
        {
            errorpanel.Visibility = Visibility.Hidden;

        }

        private void Username_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var uie = e.OriginalSource as UIElement;

            if (e.Key == Key.Tab)
            {
                if (Username.Text == "")
                {
                    Username.Text = "Username";
                    Username.Foreground = new SolidColorBrush(Colors.Gray);
                }
                e.Handled = true;
                uie.MoveFocus(
                new TraversalRequest(
                FocusNavigationDirection.Previous));
            }


        }

        private void Password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (Password.Password == "")
                {
                    Password.Password = "Password";
                    Password.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }

        #endregion

        #region Get Contacts Profile Data Worker Function
        //this Function gets all profile and data from the user contact list and saved in a cash system

        void GetProfileWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (GetProfileWorker.CancellationPending == true)
                {
                    e.Cancel = true;
                }
                else
                {
                    string aa = File.ReadAllText(Settings.UserID);
                    string bb = File.ReadAllText(Settings.SessionID);
                    string cc = File.ReadAllText(Settings.UserID);
                    string TextID = aa.Replace("\r\n", "");
                    string sessionIDText = bb.Replace("\r\n", "");
                    string userprofile = bb.Replace("\r\n", "");

                    using (var client = new WebClient())
                    {
                        var values = new NameValueCollection();
                        values["user_id"] = TextID;
                        values["user_profile_id"] = userprofile;
                        values["s"] = sessionIDText;

                        var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_users_list", values);
                        var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                        var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                        string ApiStatus = dictChatusersList["api_status"].ToString();

                        if (ApiStatus == "200")
                        {
                            var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                            var gg = dictChatusersList["users"];
                            var Profiles = JObject.Parse(ChatusersListresponseString).SelectToken("users").ToString();
                            Object obj = s.DeserializeObject(Profiles);
                            JArray Profileusers = JArray.Parse(Profiles);

                            string PostProfiles = "";

                            foreach (var ProfileUser in Profileusers)
                            {
                                JObject ProfileUserdata = JObject.FromObject(ProfileUser);
                                var Profile_User_ID = ProfileUserdata["user_id"].ToString();
                                PostProfiles += Profile_User_ID + ",";
                            }

                            //Result of all profiles and friends of the user Account splited by (,)
                            string result = PostProfiles;

                            values["user_id"] = TextID;
                            values["usersIDs"] = result;
                            values["s"] = sessionIDText;
                            var ProfileListPost = client.UploadValues(Settings.WebsiteUrl + "app_api.php?type=get_multi_users", values);
                            var ProfileListPostresponseString = Encoding.Default.GetString(ProfileListPost);
                            var dictProfileList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ProfileListPostresponseString);
                            string ApiStatus2 = dictProfileList["api_status"].ToString();

                            if (ApiStatus2 == "200")
                            {
                                gg = dictProfileList["users"];
                                Profiles = JObject.Parse(ProfileListPostresponseString).SelectToken("users").ToString();
                                obj = s.DeserializeObject(Profiles);
                                Profileusers = JArray.Parse(Profiles);
                                foreach (var ProfileUser in Profileusers)
                                {
                                    JObject ProfileUserdata = JObject.FromObject(ProfileUser);

                                    var Profile_User_ID = ProfileUserdata["user_id"].ToString();
                                    var path = Settings.FolderDestination + Settings.SiteName + @"\Data\" + Profile_User_ID + @"\";
                                    var path2 = Settings.FolderDestination + Settings.SiteName + @"\Data\" + Profile_User_ID + @"\" + Profile_User_ID + ".json";
                                    if (CashSystem(path2) == "0")
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(path);
                                        string Json = ProfileUser.ToString();
                                        System.IO.File.WriteAllText(path2, Json);
                                    }
                                    else
                                    {
                                        string Json = ProfileUser.ToString();
                                        System.IO.File.WriteAllText(path2, Json);
                                    }
                                    var Profile_Cover = ProfileUserdata["cover"].ToString();
                                    string lastItemOfSplit = Profile_Cover.Split('/').Last();
                                    var Coverpath = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + Profile_User_ID + @"\";
                                    var Coverpath2 = Coverpath + lastItemOfSplit;
                                    if (CashSystem(Coverpath2) == "0")
                                    {
                                        if (!Directory.Exists(Coverpath))
                                        {
                                            DirectoryInfo di = Directory.CreateDirectory(Coverpath);
                                        }

                                        client.DownloadFile(Profile_Cover, Coverpath2);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch { }
        }
        void GetProfileWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Loginbutton.Content = "Loading Data.." + e.ProgressPercentage;
        }
        void GetProfileWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (GetProfileWorker.WorkerSupportsCancellation == true)
            { GetProfileWorker.CancelAsync(); }

        }
        #endregion

        private void StyleChanger()
        {
            try
            {
                ApplicationName.Text = Settings.SiteName;
                Color header_hover_border = (Color)ColorConverter.ConvertFromString(Settings.Header_Hover_Border);
                panelmove.Background = new SolidColorBrush(header_hover_border);
                Rigesterpanel.Background = new SolidColorBrush(header_hover_border);

                Color Header_Background = (Color)ColorConverter.ConvertFromString(Settings.Header_Background);
                formLogin.Background = new SolidColorBrush(Header_Background);

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
                Logowebsite.Source = bitmap;

                Color Btn_Background_Color = (Color)ColorConverter.ConvertFromString(Settings.Btn_Background_Color);
                this.Resources["FGColor"] = new SolidColorBrush(Btn_Background_Color);

                Color btn_hover_background_color = (Color)ColorConverter.ConvertFromString(Settings.Btn_Hover_Background_Color);
                this.Resources["FGHoverColor"] = new SolidColorBrush(btn_hover_background_color);
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
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        #region Get Emoji Icons From the server
        public void Emogi_Icon_Checker()
        {
          try{
                using (var client = new WebClient())
                {
                    #region Icons

                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\verified.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\");
                        client.DownloadFile(Settings.WebsiteUrl + "/themes/wowonder/img/windows_app/verified.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\verified.png");
                    }

                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\");
                        client.DownloadFile(Settings.WebsiteUrl + "/themes/wowonder/img/windows_app/Download.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png");
                    }

                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\");
                        client.DownloadFile(Settings.WebsiteUrl + "/themes/wowonder/img/windows_app/Audio File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png");
                    }

                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\");
                        client.DownloadFile(Settings.WebsiteUrl + "/themes/wowonder/img/windows_app/Video File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png");
                    }
                    #endregion

                    #region Emoji Icons
                    var WebsiteEmojiUrl = Settings.WebsiteUrl + "/themes/" + Settings.Themes + "/img/windows_app/emoji/";
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Angel Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "Angel Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Angel Emoji.png");
                    }

                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Angry Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "Angry Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Angry Emoji.png");
                    }

                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Angry Face.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "Angry Face.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Angry Face.png");
                    }

                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Bawling Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "Bawling Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Bawling Emoji.png");
                    }

                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Breaking Heart Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "Breaking Heart Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Breaking Heart Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Cheeky Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "Cheeky Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Cheeky Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Cool Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Cool Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Cool Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Crazy Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Crazy Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Crazy Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Cringe Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Cringe Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Cringe Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Crying Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Crying Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Crying Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Devil Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Devil Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Devil Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Devil Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Devil Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Devil Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Dissatisfied Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Dissatisfied Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Dissatisfied Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Face With Straight Mouth Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Face With Straight Mouth Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Face With Straight Mouth Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Flirting Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Flirting Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Flirting Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Happy Face Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Happy Face Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Happy Face Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Heart Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Heart Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Heart Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Heart Eyes Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Heart Eyes Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Heart Eyes Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Heavy Exclamation.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Heavy Exclamation.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Heavy Exclamation.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Kissing Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Kissing Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Kissing Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Laughing Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Laughing Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Laughing Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Lovestruck Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Lovestruck Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Lovestruck Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\NormalSmile.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/NormalSmile.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\NormalSmile.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Pained Face Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Pained Face Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Pained Face Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Puzzled Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Puzzled Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Puzzled Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Red Face Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Red Face Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Red Face Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\sad.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/sad.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\sad.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Sadface Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Sadface Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Sadface Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Scream Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Scream Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Scream Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Smile.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Smile.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Smile.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Sparkling Heart.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Sparkling Heart.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Sparkling Heart.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Star Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Star Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Star Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Straight Faced Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Straight Faced Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Straight Faced Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Surprised Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Surprised Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Surprised Emoji.png");
                    }
                    if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Emogis\Wink Emoji.png") == "0")
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Emogis\");
                        client.DownloadFile(WebsiteEmojiUrl + "/Wink Emoji.png", Settings.FolderDestination + Settings.SiteName + @"\Emogis\Wink Emoji.png");
                    }
                    #endregion
                }
             }
            catch { }
        }
        public void Emogi_Icon_Complete()
        {
           //Do Nothing 
        }
        #endregion

       
    }
}
