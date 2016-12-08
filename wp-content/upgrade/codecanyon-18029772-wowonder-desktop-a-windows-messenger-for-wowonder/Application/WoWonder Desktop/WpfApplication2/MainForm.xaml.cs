using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml;


namespace WpfApplication2
{

    public partial class MainForm : Window
    {
        #region  # >>> Variables Declaration
        public static Dictionary<string, string> _Messages = new Dictionary<string, string> { };
        private static string session_ID = "";
        private static string IDUser = "";
        private static string SearchText = "";
        private static string UserPostID = "";
        private static string friendtext = "";
        private static string LastMessageid = "";
        private static string FirstMessageid = "";
        private static string LoadmoremessgaCount = "";
        private static string Profilename = "";
        private static string ProfilePicSourse = "";
        private static string CatchMessageCompare = "";
        private static string ContentMsg = "";
        private static string chatuserid = "";
        
        #endregion

        #region  # >>> Lists Items Declaration
        private static List<TodoItem> items = new List<TodoItem>();
        private static List<PostItem> Postitems = new List<PostItem>();
        private static List<FriendsItem> Frendlistitems = new List<FriendsItem>();
        private static List<PopUpModelBinder> PopupItems = new List<PopUpModelBinder>();
        private static ObservableCollection<MessageItem> MSGitems = new ObservableCollection<MessageItem>();
        #endregion

        #region  # >>> Background Workers Declaration
        private static BackgroundWorker SearchBoxWorker = new BackgroundWorker();
        private static BackgroundWorker GetProfileWorker = new BackgroundWorker();
        private static BackgroundWorker workerTimers = new BackgroundWorker();
        private static BackgroundWorker PostWorker = new BackgroundWorker();
        private static BackgroundWorker CachSystemWorker = new BackgroundWorker();
        private static BackgroundWorker MessageWorker = new BackgroundWorker();
        private static BackgroundWorker MessageUpdaterTimer = new BackgroundWorker();
        private static BackgroundWorker LoadMoreMessageWorker = new BackgroundWorker();
        #endregion

        private static object _syncLock = new object();
        public MainForm()
        {
            string aa = File.ReadAllText(Settings.UserID); string bb = File.ReadAllText(Settings.SessionID);
            string ID = aa.Replace("\r\n", ""); string session_ID_Text = bb.Replace("\r\n", "");
            session_ID = session_ID_Text; IDUser = ID;
            try
            {
                FirstRun();
                InitializeComponent();
                StyleChanger();
            }

            catch { }
           

            BindingOperations.EnableCollectionSynchronization(MSGitems, _syncLock);
            try
            {
                CheckCaching(IDUser); CacheUserID = IDUser;
                CachSystemWorker = new BackgroundWorker();
                CachSystemWorker.WorkerReportsProgress = true;
                CachSystemWorker.WorkerSupportsCancellation = true;
                CachSystemWorker.DoWork += CacheSystemWorker_DoWork;
                CachSystemWorker.ProgressChanged += CacheSystemWorker_ProgressChanged;
                CachSystemWorker.RunWorkerCompleted += CacheSystemWorker_RunWorkerCompleted;
                CachSystemWorker.RunWorkerAsync();

                workerTimers.WorkerReportsProgress = true;
                workerTimers.WorkerSupportsCancellation = true;
                workerTimers.DoWork += workerTimers_DoWork;
                workerTimers.ProgressChanged += workerTimers_ProgressChanged;
                workerTimers.RunWorkerCompleted += workerTimers_RunWorkerCompleted;
                workerTimers.RunWorkerAsync();

                UserPostID = IDUser;
                PostWorker = new BackgroundWorker();
                PostWorker.WorkerReportsProgress = true;
                PostWorker.WorkerSupportsCancellation = true;
                PostWorker.DoWork += PostFriendWorker_DoWork;
                PostWorker.ProgressChanged += PostFriendWorker_ProgressChanged;
                PostWorker.RunWorkerCompleted += PostFriendWorker_RunWorkerCompleted;
                PostWorker.RunWorkerAsync();

                MessageWorker.WorkerReportsProgress = true;
                MessageWorker.WorkerSupportsCancellation = true;
                MessageWorker.DoWork += MessageWorker_DoWork;
                MessageWorker.ProgressChanged += MessageWorker_ProgressChanged;
                MessageWorker.RunWorkerCompleted += MessageWorker_RunWorkerCompleted;

                MessageUpdaterTimer.WorkerReportsProgress = true;
                MessageUpdaterTimer.WorkerSupportsCancellation = true;
                MessageUpdaterTimer.DoWork += MessageUpdaterTimer_DoWork;
                MessageUpdaterTimer.ProgressChanged += MessageUpdaterTimer_ProgressChanged;
                MessageUpdaterTimer.RunWorkerCompleted += MessageUpdaterTimer_RunWorkerCompleted;
            }
            catch { }
        }

        private void HeadPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        #region Profile Panel Control and animation 
        private void UsernameText_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Settings.WebsiteUrl + "/" + UsernameText.Content);
            }
            catch { }

        }
        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                       () =>
                       {
                           ShowHideMenu("sbShowRightMenu", Profileclose, Profile, ProfilePanel);
                       }));
            }
            catch { }
        }
        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, Border pnl)
        {
            try
            {
                Storyboard sb = Resources[Storyboard] as Storyboard;
                sb.Begin(pnl);
                if (Storyboard.Contains("Show"))
                {
                    Profileclose.Visibility = System.Windows.Visibility.Visible;
                    Profile.Visibility = System.Windows.Visibility.Hidden;
                }
                else if (Storyboard.Contains("Hide"))
                {
                    Profileclose.Visibility = System.Windows.Visibility.Hidden;
                    Profile.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch { }
        }
        private void Profileclose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                        () =>
                        {
                            ShowHideMenu("sbHideRightMenu", Profileclose, Profile, ProfilePanel);
                        }));
            }
            catch { }
        }
        #endregion

        private void Window_ContentRendered_1(object sender, EventArgs e)
        {
            try
            {
                if (PostWorker.WorkerSupportsCancellation == true)
                { PostWorker.CancelAsync(); }
                LoadmoreMessages.Visibility = System.Windows.Visibility.Hidden;
            }
            catch { }
        }

        private void MessageBoxText_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextRange textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd);
                if (textRange.Text == "Write your Message\r\n")
                {
                    textRange.Text = "";
                   MessageBoxText.Foreground = new SolidColorBrush(Colors.DarkGray);
                }
            }

            catch { }
        }

        #region Important Functions
        public void FirstRun()
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    using (var client = new WebClient())
                    {
                        var values = new NameValueCollection();
                        values["user_id"] = IDUser;
                        values["user_profile_id"] = IDUser;
                        values["s"] = session_ID;
                        var response = client.UploadValues(Settings.WebsiteUrl + "app_api.php?type=get_user_data", values);
                        var responseString = Encoding.Default.GetString(response);
                        var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(responseString);
                        string ApiStatus = dict["api_status"].ToString();
                        if (ApiStatus == "200")
                        {
                            JObject userdata = JObject.FromObject(dict["user_data"]);
                            LoginUserName.Text = userdata["name"].ToString();
                        }
                    }
                }));
            }
            catch { CheckCaching(IDUser); }
        }

        public string CashSystem(string fileName)
        {
            try
            {
                string path = Settings.FolderDestination + @"\\" + Settings.SiteName + @"\\";
                if (Directory.Exists(path))
                {
                    if (System.IO.File.Exists(fileName))
                    { return "1"; }
                }
                return "0";
            }
            catch { return "0"; }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    { return true; }
                }
            }
            catch { return false; }
        }

        public static string GetMessageSeen(string text)
        {
            string match = string.Empty;
            foreach (KeyValuePair<string, string> pair in _Messages)
            {
                if (text.Contains(pair.Key))
                {
                    match = pair.Key;
                }
            }
            return match;
        }

        private void StyleChanger()
        {
            try
            {
                NameApplication.Text = Settings.SiteName;
                Color Header_Background = (Color)ColorConverter.ConvertFromString(Settings.Header_Background);
                TopPanel.Background = new SolidColorBrush(Header_Background);
                Color header_color = (Color)ColorConverter.ConvertFromString(Settings.Header_Color);
                NameApplication.Foreground = new SolidColorBrush(header_color);
                LoginUserName.Foreground = new SolidColorBrush(header_color);
                Color btn_color = (Color)ColorConverter.ConvertFromString(Settings.Btn_Color);
                ChatTitleChange.Foreground = new SolidColorBrush(btn_color);
                Color Btn_Background_Color = (Color)ColorConverter.ConvertFromString(Settings.Btn_Background_Color);
                ChatInfoPanel.Background = new SolidColorBrush(Btn_Background_Color);
                this.Resources["FGColor"] = new SolidColorBrush(Btn_Background_Color);
                Color chat_outgoing_background = (Color)ColorConverter.ConvertFromString(Settings.Chat_Outgoing_Background);
                this.Resources["FGChatoutgoingColor"] = new SolidColorBrush(chat_outgoing_background);
                this.Resources["FGSettingsMenueColor"] = new SolidColorBrush(Header_Background);
                if (Settings.Show_SettingsMenue) { SettingsMenu.Visibility = System.Windows.Visibility.Visible; }
            }
            catch { }
        }
        public bool CheckCaching(string CacheUser)
        {
            try
            {
                using (var client = new WebClient())
                {
                    //client.QueryString = null;
                    var path2 = Settings.FolderDestination + Settings.SiteName + @"\Data\" + CacheUser + @"\" + CacheUser + ".json";
                    if (CashSystem(path2) == "1")
                    {
                        string bb = File.ReadAllText(path2);
                        string ProfileResult = bb.Replace("\r\n", "");
                        var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ProfileResult);
                        JObject userdata = JObject.FromObject(dict);
                        var avatar = userdata["avatar"].ToString();
                        var cover = userdata["cover"].ToString();
                        var name = userdata["name"].ToString();
                        var username = userdata["username"].ToString();
                        var gender = userdata["gender"].ToString();
                        var birth = userdata["birthday"].ToString();
                        var email = userdata["email"].ToString();
                        var address = userdata["address"].ToString();
                        var JSlastseen_status = userdata["lastseen_status"].ToString();
                        var JSlastseen = userdata["lastseen_text"].ToString();
                        string AvatarSplit = avatar.Split('/').Last();
                        string CoverSplit = cover.Split('/').Last();
                        var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + CacheUser + @"\";
                        var ProfileImage = path + AvatarSplit;
                        var CoverImage = path + CoverSplit;
                        if (CashSystem(ProfileImage) == "0")
                        {
                            client.DownloadFile(avatar, ProfileImage);
                        }
                        if (CashSystem(CoverImage) == "0")
                        {
                            client.DownloadFile(cover, CoverImage);
                        }
                        var Profileuri = new Uri(ProfileImage);
                        var Profilebitmap = new BitmapImage(Profileuri);
                        var Avataruri = new Uri(CoverImage);
                        var Avatarbitmap = new BitmapImage(Avataruri);
                        ProfilePicture.Source = Profilebitmap;
                        CoverPicture.Source = Avatarbitmap;
                        FullNameText.Text = name;
                        FullName.Text = name;
                        UsernameText.Content = "@" + username;
                        GenderText.Text = gender;
                        if (birth.Contains("000"))
                        {
                            birth = "Private";
                        }
                        CalenderText.Text = birth;
                        EmailText.Text = email;
                        PlaceeText.Text = address;
                        if (PlaceeText.Text == "")
                        { PlaceeText.Text = "Location: Unknown"; }
                        if (JSlastseen_status == "online")
                        {
                            LastseenText.Text = "Online";
                            LastseenText.Foreground = new SolidColorBrush(Colors.Green);
                        }
                        else
                        {
                            LastseenText.Text = JSlastseen;
                            Color color = (Color)ColorConverter.ConvertFromString("#555");
                            LastseenText.Foreground = new SolidColorBrush(color);

                        }
                        BlurimageLoadingpanel.Visibility = System.Windows.Visibility.Hidden;
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }


            }
            catch { return false; }
        }
        public void GetCashedMsg(string user_id)
        {
            try
            {

                var pathUser = Settings.FolderDestination + Settings.SiteName + @"\Data\" + user_id + @"\" + user_id + "_M.json";

                if (CashSystem(pathUser) == "1")
                {
                    string bb = File.ReadAllText(pathUser);
                    string MsgResult = bb.Replace("\r\n", "");


                    JArray ChatMessages = JArray.Parse(bb);

                    MSGitems.Clear();
                    foreach (var MessageInfo in ChatMessages)
                    {
                        JObject ChatlistUserdata = JObject.FromObject(MessageInfo);
                        var Blal = ChatlistUserdata["messageUser"];
                        var avatar = Blal["avatar"].ToString();
                        var Position = MessageInfo["position"].ToString();
                        var TextMsg = MessageInfo["text"].ToString();
                        var Type = MessageInfo["type"].ToString();
                        var TimeMsg = MessageInfo["time_text"].ToString();
                        var msgID = MessageInfo["id"].ToString();

                        StringWriter myWriter = new StringWriter();
                        HttpUtility.HtmlDecode(TextMsg, myWriter);

                        TextMsg = myWriter.ToString();

                        var Widthresult = "";
                        var VisibiltyEvent = "";
                        if (TextMsg != "")
                        {
                            VisibiltyEvent = "Visible";
                            if (TextMsg.Length >= 0 && TextMsg.Length <= 3)
                            { Widthresult = "35"; }
                            if (TextMsg.Length > 3 && TextMsg.Length <= 6)
                            { Widthresult = "50"; }
                            if (TextMsg.Length > 6 && TextMsg.Length <= 9)
                            { Widthresult = "85"; }
                            if (TextMsg.Length > 9 && TextMsg.Length <= 13)
                            { Widthresult = "105"; }
                            if (TextMsg.Length > 10 && TextMsg.Length <= 13)
                            { Widthresult = "105"; }
                            if (TextMsg.Length >= 10 && TextMsg.Length <= 15)
                            { Widthresult = "140"; }
                            if (TextMsg.Length > 15 && TextMsg.Length <= 18)
                            { Widthresult = "155"; }
                            if (TextMsg.Length > 18 && TextMsg.Length <= 23)
                            { Widthresult = "155"; }
                            if (TextMsg.Length >= 20 && TextMsg.Length <= 26)
                            { Widthresult = "175"; }
                            if (TextMsg.Length >= 26 && TextMsg.Length <= 30)
                            { Widthresult = "200"; }
                            if (TextMsg.Length >= 31 && TextMsg.Length <= 35)
                            { Widthresult = "250"; }
                            if (TextMsg.Length >= 36 && TextMsg.Length <= 45)
                            { Widthresult = "300"; }
                        }
                        else
                        {
                            VisibiltyEvent = "Collapsed";
                            Widthresult = "0";
                        }

                        if (Position == "left")
                        {
                            try
                            {
                                #region Left Text
                                if (Type == "left_text")
                                {
                                    string lastItemOfSplit = avatar.Split('/').Last();
                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + user_id + @"\";
                                    var path2 = path + lastItemOfSplit;

                                    var ProfileImage2 = new BitmapImage();
                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                    {
                                        ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 40; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                        ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                    }
                                    ProfileImage2.Freeze();
                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, Type = Type, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, UserImage = ProfileImage2, messageID = msgID });

                                }
                                #endregion
                                #region Left Image
                                if (Type == "left_image")
                                {
                                    var ImageUrl = MessageInfo["media"].ToString();
                                    string lastItemOfSplit = ImageUrl.Split('/').Last();
                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + user_id + @"\";
                                    var path2 = path + lastItemOfSplit;

                                    var Image = new BitmapImage();
                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                    {
                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                    }

                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, Type = Type, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID, DownloadFileUrl = path2 });

                                }
                                #endregion
                                #region Left File
                                if (Type == "left_file")
                                {
                                    var fileUrl = MessageInfo["media"].ToString();
                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                    var MediaSize = MessageInfo["file_size"].ToString();
                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                    if (FileName.Length >= 35)
                                    {
                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                    }
                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                    string lastItemOfSplit = avatar.Split('/').Last();
                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + user_id + @"\";
                                    var path2 = path + lastItemOfSplit;

                                    var ProfileImage2 = new BitmapImage();
                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                    {
                                        ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                        ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                    }
                                    ProfileImage2.Freeze();


                                    var Image = new BitmapImage();
                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png", FileMode.Open, FileAccess.Read))
                                    {
                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                    }

                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                                }
                                #endregion
                                #region Left Video
                                if (Type == "left_video")
                                {
                                    var fileUrl = MessageInfo["media"].ToString();
                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                    var MediaSize = MessageInfo["file_size"].ToString();
                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                    if (FileName.Length >= 35)
                                    {
                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                    }
                                    string lastItemOfSplit = avatar.Split('/').Last();
                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + user_id + @"\";
                                    var path2 = path + lastItemOfSplit;

                                    var ProfileImage2 = new BitmapImage();
                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                    {
                                        ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                        ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                    }
                                    ProfileImage2.Freeze();


                                    var Image = new BitmapImage();
                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png", FileMode.Open, FileAccess.Read))
                                    {
                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                    }

                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                                }
                                #endregion
                                #region Left Sound
                                if (Type == "left_audio")
                                {
                                    var fileUrl = MessageInfo["media"].ToString();
                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                    var MediaSize = MessageInfo["file_size"].ToString();
                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                    if (FileName.Length >= 35)
                                    {
                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                    }

                                    string lastItemOfSplit = avatar.Split('/').Last();
                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + user_id + @"\";
                                    var path2 = path + lastItemOfSplit;

                                    var ProfileImage2 = new BitmapImage();
                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                    {
                                        ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                        ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                    }
                                    ProfileImage2.Freeze();

                                    var Image = new BitmapImage();
                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png", FileMode.Open, FileAccess.Read))
                                    {
                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                    }

                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                                }
                                #endregion
                            }
                            catch
                            { }
                        }
                        else
                        {
                            try
                            {
                                #region right text
                                if (Type == "right_text")
                                {
                                    MSGitems.Add(new MessageItem() { Content = TextMsg, Type = Type, CreatedAt = TimeMsg, WidthWrap = Widthresult, messageID = msgID });
                                }
                                #endregion
                                #region right Image
                                if (Type == "right_image")
                                {
                                    var ImageUrl = MessageInfo["media"].ToString();
                                    string lastItemOfSplit = ImageUrl.Split('/').Last();
                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + user_id + @"\";
                                    var path2 = path + lastItemOfSplit;

                                    var Image = new BitmapImage();
                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                    {
                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                    }

                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, WidthWrap = Widthresult, Type = Type, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID, DownloadFileUrl = path2 });
                                }
                                #endregion
                                #region right File
                                if (Type == "right_file")
                                {
                                    var fileUrl = MessageInfo["media"].ToString();
                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                    var MediaSize = MessageInfo["file_size"].ToString();
                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                    if (FileName.Length >= 35)
                                    {
                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                    }
                                    var Image = new BitmapImage();
                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png", FileMode.Open, FileAccess.Read))
                                    {
                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                    }

                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });

                                }
                                #endregion
                                #region right Video
                                if (Type == "right_video")
                                {
                                    var fileUrl = MessageInfo["media"].ToString();
                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                    var MediaSize = MessageInfo["file_size"].ToString();
                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                    if (FileName.Length >= 35)
                                    {
                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                    }
                                    var Image = new BitmapImage();
                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png", FileMode.Open, FileAccess.Read))
                                    {
                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                    }

                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });
                                }
                                #endregion
                                #region Left Sound
                                if (Type == "right_audio")
                                {
                                    var fileUrl = MessageInfo["media"].ToString();
                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                    var MediaSize = MessageInfo["file_size"].ToString();
                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                    if (FileName.Length >= 35)
                                    {
                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                    }
                                    var Image = new BitmapImage();
                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png", FileMode.Open, FileAccess.Read))
                                    {
                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                    }
                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });
                                }
                                #endregion
                            }
                            catch
                            { }
                        }

                    }
                    try
                    {
                        if (MSGitems.Count == 0)
                        {
                            return;
                        }
                        else
                        {

                            ChatMessgaeslist.ItemsSource = MSGitems;
                           // ChatMessgaeslist.Items.Refresh();
                            ChatMessgaeslist.SelectedIndex = ChatMessgaeslist.Items.Count - 1;
                            ChatMessgaeslist.ScrollIntoView(ChatMessgaeslist.SelectedItem);
                            ListBoxAutomationPeer svAutomation = (ListBoxAutomationPeer)ScrollViewerAutomationPeer.CreatePeerForElement(ChatMessgaeslist);
                            IScrollProvider scrollInterface = (IScrollProvider)svAutomation.GetPattern(PatternInterface.Scroll);
                            System.Windows.Automation.ScrollAmount scrollVertical = System.Windows.Automation.ScrollAmount.LargeIncrement;
                            System.Windows.Automation.ScrollAmount scrollHorizontal = System.Windows.Automation.ScrollAmount.NoAmount;
                            //If the vertical scroller is not available, the operation cannot be performed, which will raise an exception. 
                            if (scrollInterface.VerticallyScrollable)
                                scrollInterface.Scroll(scrollHorizontal, scrollVertical);
                        }

                        if (MSGitems.Last().messageID != "")
                        {
                            CatchMessageCompare = MSGitems.Last().messageID;
                        }
                        else
                        {

                        }
                    }
                    catch
                    { }
                }
                else
                {
                    return;
                }

            }
            catch
            { }


        }
        public string Truncate(string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
        public byte[] UploadFiles(string address, IEnumerable<UploadFile> files, NameValueCollection values)
        {

            var request = WebRequest.Create(address);
            request.Method = "POST";
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "--" + boundary;

            using (var requestStream = request.GetRequestStream())
            {
                // Write the values
                foreach (string name in values.Keys)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"{1}{1}", name, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(values[name] + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                // Write the files
                foreach (var file in files)
                {
                    var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", file.Name, file.Filename, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", file.ContentType, Environment.NewLine));
                    requestStream.Write(buffer, 0, buffer.Length);
                    file.Stream.CopyTo(requestStream);
                    buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                    requestStream.Write(buffer, 0, buffer.Length);
                }

                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }

            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var stream = new MemoryStream())
            {
                responseStream.CopyTo(stream);
                return stream.ToArray();
            }
        }
        #endregion

        private void lbTodoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (lbTodoList.SelectedItem != null)
                {
                    if (PostWorker.WorkerSupportsCancellation == true)
                    {
                        PostWorker.CancelAsync();
                    }
                    try
                    {
                        TextRange textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd);
                        var TextMsg = textRange.Text;
                     
                        if (TextMsg == "")
                        {
                            textRange.Text = "Write your Message";
                            MessageBoxText.Foreground = new SolidColorBrush(Colors.DarkGray);
                        }
                        try
                        {
                            var bw = new BackgroundWorker();
                            bw.DoWork += (o, args) => TypingEvent("removing");
                            bw.RunWorkerAsync();
                        }
                        catch { }

                        loadingfrendsellipse.Visibility = System.Windows.Visibility.Visible;
                        loadingpostsellipse.Visibility = System.Windows.Visibility.Visible;
                        FriendIcon.Visibility = System.Windows.Visibility.Hidden;
                        LastPostIcon.Visibility = System.Windows.Visibility.Hidden;
                        NoMessagePanel.Visibility = System.Windows.Visibility.Hidden;
                        BlurimageLoadingpanel.Visibility = System.Windows.Visibility.Visible;
                        LoadmoreMessages.Visibility = System.Windows.Visibility.Hidden;
                        ChatSeen.Text = "";
                        MSGitems.Clear();
                        Frendlistitems.Clear();
                        var NoMessageBlock = this.FindResource("NoMessageBlock") as TextBlock;
                        NoMessageBlock.Text = "No messages here yet...";
                        ChatTitleChange.Text = (lbTodoList.SelectedItem as TodoItem).username;
                        UserPostID = (lbTodoList.SelectedItem as TodoItem).user_id;
                        LoadmoreMessages.Content = "Load More Messages";
                        CacheUserID = (lbTodoList.SelectedItem as TodoItem).user_id;
                        chatuserid = (lbTodoList.SelectedItem as TodoItem).user_id;
                    }
                    catch{}

                    try{CheckCaching(UserPostID);}catch{}
                    try{GetCashedMsg(CacheUserID);
                        ListBoxAutomationPeer svAutomation = (ListBoxAutomationPeer)ScrollViewerAutomationPeer.CreatePeerForElement(ChatMessgaeslist);
                        IScrollProvider scrollInterface = (IScrollProvider)svAutomation.GetPattern(PatternInterface.Scroll);
                        System.Windows.Automation.ScrollAmount scrollVertical = System.Windows.Automation.ScrollAmount.LargeIncrement;
                        System.Windows.Automation.ScrollAmount scrollHorizontal = System.Windows.Automation.ScrollAmount.NoAmount;
                        //If the vertical scroller is not available, the operation cannot be performed, which will raise an exception. 
                        if (scrollInterface.VerticallyScrollable)
                            scrollInterface.Scroll(scrollHorizontal, scrollVertical);
                        ChatMessgaeslist.SelectedIndex = ChatMessgaeslist.Items.Count - 1;
                        ChatMessgaeslist.ScrollIntoView(ChatMessgaeslist.SelectedItem);


                    } catch{}
                   
                    if (MessageWorker.IsBusy && MessageWorker.WorkerSupportsCancellation == true)MessageWorker.CancelAsync();
                    if (CachSystemWorker.IsBusy && CachSystemWorker.WorkerSupportsCancellation == true)CachSystemWorker.CancelAsync();
                   
                    try {MessageWorker.RunWorkerAsync();}
                    catch 
                    {
                        if (CachSystemWorker.IsBusy && CachSystemWorker.WorkerSupportsCancellation == true) { CachSystemWorker.CancelAsync(); } 

                        MessageWorker = new BackgroundWorker();
                        MessageWorker.WorkerReportsProgress = true;
                        MessageWorker.WorkerSupportsCancellation = true;
                        MessageWorker.DoWork += MessageWorker_DoWork;
                        MessageWorker.ProgressChanged += MessageWorker_ProgressChanged;
                        MessageWorker.RunWorkerCompleted += MessageWorker_RunWorkerCompleted;
                        MessageWorker.RunWorkerAsync();
                    }

                    ChatMessgaeslist.SelectedIndex = ChatMessgaeslist.Items.Count - 1;
                    if (ChatMessgaeslist.SelectedItem != null) {LastMessageid = (ChatMessgaeslist.SelectedItem as MessageItem).messageID; }
                              
                    try{StartPostDataLoad(1000);}catch {}
                    try{StartMessageUpdater(10000);}catch {}
                    try {StartCachSystemAdd(15000); } catch { }
                }
            }
            catch {}
        }

        private async void StartCachSystemAdd(int milliseconds)
        {
            try
            {
                // Simulate work.
                await Task.Delay(milliseconds);

                CachSystemWorker = new BackgroundWorker();
                CachSystemWorker.WorkerReportsProgress = true;
                CachSystemWorker.WorkerSupportsCancellation = true;
                CachSystemWorker.DoWork += CacheSystemWorker_DoWork;
                CachSystemWorker.ProgressChanged += CacheSystemWorker_ProgressChanged;
                CachSystemWorker.RunWorkerCompleted += CacheSystemWorker_RunWorkerCompleted;
                CachSystemWorker.RunWorkerAsync();
            }
            catch { }


        }
        private async void StartPostDataLoad(int milliseconds)
        {
            try
            {
                // Simulate work.
                await Task.Delay(milliseconds);

                // Report completion.
                bool uiAccess = PostList.Dispatcher.CheckAccess();

                PostWorker = new BackgroundWorker();
                PostWorker.WorkerReportsProgress = true;
                PostWorker.WorkerSupportsCancellation = true;
                PostWorker.DoWork += PostFriendWorker_DoWork;
                PostWorker.ProgressChanged += PostFriendWorker_ProgressChanged;
                PostWorker.RunWorkerCompleted += PostFriendWorker_RunWorkerCompleted;
                PostWorker.RunWorkerAsync();
            }
            catch { }



        }
        private async void StartMessageUpdater(int milliseconds)
        {
            try
            {
                // Simulate work.
                await Task.Delay(milliseconds);
                if (MessageUpdaterTimer.WorkerSupportsCancellation == true)
                {
                    MessageUpdaterTimer.CancelAsync();
                }
                //MessageUpdaterTimer = new BackgroundWorker();

                MessageUpdaterTimer.RunWorkerAsync();
            }
            catch { }

        }


        #region Update Chat Users List Every #BackroundWorker
        void workerTimers_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {       
                if (workerTimers.CancellationPending == true)
                {
                    e.Cancel = true;
                }
                while (!workerTimers.CancellationPending)
                {
                    Thread.Sleep(Settings.Update_ContactList_INT);
                    if (SearchText == "" || SearchText == "Search" && CheckForInternetConnection())
                    {
                        if (workerTimers.CancellationPending == true)
                        { e.Cancel = true; }
                        else
                        {
                            try
                            {
                                using (var client = new WebClient())
                                {
                                    var values = new NameValueCollection();

                                    values["user_id"] = IDUser;
                                    values["user_profile_id"] = IDUser;
                                    values["s"] = session_ID;
                                    values["list_type"] = ListType;

                                    var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_users_list", values);
                                    var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                                    var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                                    string ApiStatus = dictChatusersList["api_status"].ToString();

                                    if (ApiStatus == "200")
                                    {
                                        string ThemeUrl = dictChatusersList["theme_url"].ToString();
                                        var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                                        var gg = dictChatusersList["users"];
                                        var users = JObject.Parse(ChatusersListresponseString).SelectToken("users").ToString();
                                        Object obj = s.DeserializeObject(users);

                                        var jss = new JavaScriptSerializer();
                                        JArray Chatusers = JArray.Parse(users);

                                        items.Clear();
                                        if (workerTimers.CancellationPending == true)
                                        {
                                            e.Cancel = true;
                                        }
                                        foreach (var ChatUser in Chatusers)
                                        {
                                            JObject ChatlistUserdata = JObject.FromObject(ChatUser);
                                            var ChatUser_User_ID = ChatlistUserdata["user_id"].ToString();
                                            var ChatUser_avatar = ChatlistUserdata["profile_picture"].ToString();
                                            var ChatUser_name = ChatlistUserdata["name"].ToString();
                                            var ChatUser_lastseen = ChatlistUserdata["lastseen"].ToString();
                                            var ChatUser_lastseen_Time_Text = ChatlistUserdata["lastseen_time_text"].ToString();
                                            var ChatUser_verified = ChatlistUserdata["verified"].ToString();
                                            string lastItemOfSplit = ChatUser_avatar.Split('/').Last();
                                            var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + ChatUser_User_ID + @"\";
                                            var path2 = path + lastItemOfSplit;

                                            if (CashSystem(path2) == "0")
                                            {
                                                DirectoryInfo di = Directory.CreateDirectory(path);
                                                string[] files = System.IO.Directory.GetFiles(path);
                                                client.DownloadFile(ChatUser_avatar, path2);
                                            }

                                            JObject ChatlistuserLastMessage = JObject.FromObject(ChatlistUserdata["last_message"]);
                                            var listuserLastMessage_Text = ChatlistuserLastMessage["text"].ToString();
                                            var listuserLastMessage_date_time = ChatlistuserLastMessage["date_time"].ToString();

                                            StringWriter myWriter = new StringWriter();
                                            HttpUtility.HtmlDecode(listuserLastMessage_Text, myWriter);

                                            listuserLastMessage_Text = myWriter.ToString();

                                            var bi = new BitmapImage();

                                            using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                            {
                                                bi.BeginInit();
                                                bi.DecodePixelWidth = 250; bi.CacheOption = BitmapCacheOption.OnLoad; bi.StreamSource = stream;
                                                bi.EndInit();
                                                stream.Close();
                                            }

                                            bi.Freeze();
                                            var ChatUser_verified_bitmap = new BitmapImage();
                                            var IconPathCheked = Settings.FolderDestination + Settings.SiteName + @"\Icons\verified.png";
                                            if (CashSystem(IconPathCheked) == "0")
                                            {
                                                DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\");
                                                client.DownloadFile(ThemeUrl + "/img/windows_app/verified.png", IconPathCheked);
                                            }

                                            using (var stream33 = new FileStream(IconPathCheked, FileMode.Open, FileAccess.Read))
                                            {
                                                if (ChatUser_verified == "1")
                                                {
                                                    ChatUser_verified_bitmap.BeginInit();
                                                    ChatUser_verified_bitmap.DecodePixelWidth = 15;
                                                    ChatUser_verified_bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                                    ChatUser_verified_bitmap.StreamSource = stream33;
                                                    ChatUser_verified_bitmap.EndInit();
                                                    stream33.Close();
                                                }
                                            }
                                            ChatUser_verified_bitmap.Freeze();

                                            var ChatUser_bitmap = new BitmapImage();
                                            var IconOffline = Settings.FolderDestination + Settings.SiteName + @"\Icons\offline.png";
                                            var IconOnline = Settings.FolderDestination + Settings.SiteName + @"\Icons\online.png";
                                            if (ChatUser_lastseen == "off")
                                            {
                                                if (CashSystem(IconOffline) == "0")
                                                {
                                                    DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\");
                                                    client.DownloadFile(ThemeUrl + "/img/windows_app/offline.png", IconOffline);
                                                }
                                                using (var stream = new FileStream(IconOffline, FileMode.Open, FileAccess.Read))
                                                {

                                                    ChatUser_bitmap.BeginInit();
                                                    ChatUser_bitmap.DecodePixelWidth = 25;
                                                    ChatUser_bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                                    ChatUser_bitmap.StreamSource = stream;
                                                    ChatUser_bitmap.EndInit();
                                                    stream.Close();
                                                }
                                            }
                                            else
                                            {
                                                if (CashSystem(IconOnline) == "0")
                                                {
                                                    DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\IconsWowonder\");
                                                    client.DownloadFile(ThemeUrl + "/img/windows_app/online.png", IconOnline);
                                                }
                                                using (var stream = new FileStream(IconOnline, FileMode.Open, FileAccess.Read))
                                                {

                                                    ChatUser_bitmap.BeginInit();
                                                    ChatUser_bitmap.DecodePixelWidth = 25;
                                                    ChatUser_bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                                    ChatUser_bitmap.StreamSource = stream;
                                                    ChatUser_bitmap.EndInit();
                                                    stream.Close();
                                                }
                                            }
                                            ChatUser_bitmap.Freeze();
                                            var fromId = ChatlistuserLastMessage["from_id"].ToString();
                                            var listuserSeenOrNo = ChatlistuserLastMessage["seen"].ToString();
                                            var seencolor = "Transparent";
                                            if (fromId != IDUser)
                                            {
                                                if (listuserSeenOrNo == "0")
                                                {
                                                    seencolor = "#ededed";
                                                    Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                                                    {
                                                        if (this.WindowState == WindowState.Minimized)
                                                        {
                                                            string Text = GetMessageSeen(listuserLastMessage_Text);
                                                            if (string.IsNullOrEmpty(Text))
                                                            {
                                                                ContentMsg = listuserLastMessage_Text;
                                                                Profilename = "New Message from " + ChatUser_name;
                                                                ProfilePicSourse = path2;
                                                                _Messages.Add(ContentMsg, ChatUser_User_ID);
                                                                MsgPopupWindow PopUp = new MsgPopupWindow();
                                                                PopUp.Activate();
                                                                PopUp.Show();
                                                                PopUp.Activate();
                                                                var workingAreaaa = System.Windows.SystemParameters.WorkArea;
                                                                var transform = PresentationSource.FromVisual(PopUp).CompositionTarget.TransformFromDevice;
                                                                var corner = transform.Transform(new Point(workingAreaaa.Right, workingAreaaa.Bottom));

                                                                PopUp.Left = corner.X - PopUp.ActualWidth - 10;
                                                                PopUp.Top = corner.Y - PopUp.ActualHeight;
                                                                String SoundPath = Path.GetFullPath("New-message.mp3");
                                                                mediaPlayer.Open(new Uri(SoundPath));
                                                                mediaPlayer.Play();

                                                            }

                                                        }
                                                    }));
                                                }
                                            }



                                            items.Add(new TodoItem()
                                            {
                                                username = ChatUser_name,
                                                lastseen = ChatUser_bitmap,
                                                profile_picture = bi,
                                                text = listuserLastMessage_Text,
                                                lastseenunixtimetext = ChatUser_lastseen_Time_Text,
                                                LastMessage_date_time = listuserLastMessage_date_time,
                                                verified = ChatUser_verified_bitmap,
                                                user_id = ChatUser_User_ID,
                                                SeenMessageOrNo = seencolor
                                            });


                                        }

                                    }
                                    if (workerTimers.CancellationPending == true)
                                    {
                                        e.Cancel = true;
                                    }
                                }
                            }
                            catch 
                            { }

                            workerTimers.CancelAsync();
                        }
                    }
                }
                if (workerTimers.CancellationPending == true)
                {
                    e.Cancel = true;
                }
                //#workerTimers.CancelAsync();
            }
            catch 
            { }
        }
        void workerTimers_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object userObject = e.UserState;
            int percentage = e.ProgressPercentage;
        }
        void workerTimers_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          try
            {
                if (OnlineOflinePanel.Visibility == System.Windows.Visibility.Visible)
                {
                    OnlineOflinePanel.Visibility = System.Windows.Visibility.Hidden;
                    loading.Visibility = System.Windows.Visibility.Collapsed;
                }
                lbTodoList.ItemsSource = items;
                lbTodoList.Items.Refresh();
                workerTimers.CancelAsync();
                workerTimers.RunWorkerAsync();
            }
            catch {}
        }
        #endregion

        #region SearchBox Worker Control
        void SearchBoxWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            if (SearchBoxWorker.CancellationPending == true)
            {
                e.Cancel = true;

            }
            else
            {

                if (CheckForInternetConnection())
                {
                    using (var client = new WebClient())
                    {
                        client.Proxy = null;
                        client.QueryString = null;

                        var values = new NameValueCollection();
                        values["user_id"] = IDUser;
                        values["user_profile_id"] = IDUser;
                        values["s"] = session_ID;
                        values["search_key"] = SearchText;
                        var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_users_list", values);

                        var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                        var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                        string ApiStatus = dictChatusersList["api_status"].ToString();

                        if (ApiStatus == "200")
                        {
                            string ThemeUrl = dictChatusersList["theme_url"].ToString();
                            var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                            var gg = dictChatusersList["users"];
                            var users = JObject.Parse(ChatusersListresponseString).SelectToken("users").ToString();
                            Object obj = s.DeserializeObject(users);
                            JArray Chatusers = JArray.Parse(users);

                            items.Clear();

                            foreach (var ChatUser in Chatusers)
                            {
                                JObject ChatlistUserdata = JObject.FromObject(ChatUser);
                                var ChatUser_User_ID = ChatlistUserdata["user_id"].ToString();
                                var ChatUser_avatar = ChatlistUserdata["profile_picture"].ToString();
                                var ChatUser_name = ChatlistUserdata["name"].ToString();
                                var ChatUser_lastseen = ChatlistUserdata["lastseen"].ToString();
                                var ChatUser_lastseen_Time_Text = ChatlistUserdata["lastseen_time_text"].ToString();
                                var ChatUser_verified = ChatlistUserdata["verified"].ToString();
                                string lastItemOfSplit = ChatUser_avatar.Split('/').Last();
                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + ChatUser_User_ID + @"\";
                                var path2 = path + lastItemOfSplit;
                                if (CashSystem(path2) == "0")
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(path);
                                    string[] files = System.IO.Directory.GetFiles(path);

                                    client.DownloadFile(ChatUser_avatar, path2);
                                    path2 = path;
                                }

                                JObject ChatlistuserLastMessage = JObject.FromObject(ChatlistUserdata["last_message"]);

                                var listuserLastMessage_Text = ChatlistuserLastMessage["text"].ToString();
                                var listuserLastMessage_date_time = ChatlistuserLastMessage["date_time"].ToString();

                                var bi = new BitmapImage();

                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                {
                                    bi.BeginInit(); bi.DecodePixelWidth = 250; bi.CacheOption = BitmapCacheOption.OnLoad; bi.StreamSource = stream; bi.EndInit();
                                    stream.Close();
                                }

                                bi.Freeze();

                                var ChatUser_verified_bitmap = new BitmapImage();
                                var IconPathCheked = Settings.FolderDestination + Settings.SiteName + @"\Icons\verified.png";
                                if (CashSystem(IconPathCheked) == "0")
                                {
                                    DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\");
                                    client.DownloadFile(ThemeUrl + "/img/windows_app/verified.png", IconPathCheked);
                                }

                                using (var stream33 = new FileStream(IconPathCheked, FileMode.Open, FileAccess.Read))
                                {
                                    if (ChatUser_verified == "1")
                                    {
                                        ChatUser_verified_bitmap.BeginInit();
                                        ChatUser_verified_bitmap.DecodePixelWidth = 15;
                                        ChatUser_verified_bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                        ChatUser_verified_bitmap.StreamSource = stream33;
                                        ChatUser_verified_bitmap.EndInit();
                                    }
                                }
                                ChatUser_verified_bitmap.Freeze();

                                var ChatUser_bitmap = new BitmapImage();
                                var IconOffline = Settings.FolderDestination + Settings.SiteName + @"\Icons\offline.png";
                                var IconOnline = Settings.FolderDestination + Settings.SiteName + @"\Icons\online.png";
                                if (ChatUser_lastseen == "off")
                                {
                                    if (CashSystem(IconOffline) == "0")
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\");
                                        client.DownloadFile(ThemeUrl + "/img/windows_app/offline.png", IconOffline);
                                    }
                                    using (var stream = new FileStream(IconOffline, FileMode.Open, FileAccess.Read))
                                    {

                                        ChatUser_bitmap.BeginInit();
                                        ChatUser_bitmap.DecodePixelWidth = 25;
                                        ChatUser_bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                        ChatUser_bitmap.StreamSource = stream;
                                        ChatUser_bitmap.EndInit();
                                    }
                                }
                                else
                                {
                                    if (CashSystem(IconOnline) == "0")
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\");
                                        client.DownloadFile(ThemeUrl + "/img/windows_app/online.png", IconOnline);
                                    }
                                    using (var stream = new FileStream(IconOnline, FileMode.Open, FileAccess.Read))
                                    {

                                        ChatUser_bitmap.BeginInit();
                                        ChatUser_bitmap.DecodePixelWidth = 25;
                                        ChatUser_bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                        ChatUser_bitmap.StreamSource = stream;
                                        ChatUser_bitmap.EndInit();
                                    }
                                }
                                ChatUser_bitmap.Freeze();

                                items.Add(new TodoItem()
                                {
                                    username = ChatUser_name,
                                    lastseen = ChatUser_bitmap,
                                    profile_picture = bi,
                                    text = listuserLastMessage_Text,
                                    lastseenunixtimetext = ChatUser_lastseen_Time_Text,
                                    LastMessage_date_time = listuserLastMessage_date_time,
                                    verified = ChatUser_verified_bitmap,
                                    user_id = ChatUser_User_ID
                                });


                            }

                        }

                    }
                }

            }

        }

        void SearchBoxWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if (workerTimers.WorkerSupportsCancellation == true)
            { workerTimers.CancelAsync(); }
        }

        void SearchBoxWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (SearchBoxWorker.WorkerSupportsCancellation == true)
                {
                    SearchBoxWorker.CancelAsync();
                }
                if (workerTimers.WorkerSupportsCancellation == true)
                { workerTimers.CancelAsync(); }

                lbTodoList.ItemsSource = items;
                IconSearch.Visibility = System.Windows.Visibility.Visible;
                loadingSearch.Visibility = System.Windows.Visibility.Hidden;
                lbTodoList.Items.Refresh();
                GetProfileWorker.WorkerReportsProgress = true;
                GetProfileWorker.WorkerSupportsCancellation = true;
                GetProfileWorker.DoWork += GetProfileWorker_DoWork;
                GetProfileWorker.ProgressChanged += GetProfileWorker_ProgressChanged;
                GetProfileWorker.RunWorkerCompleted += GetProfileWorker_RunWorkerCompleted;
                GetProfileWorker.RunWorkerAsync();

            }
            catch { }

        }
        #endregion 

        #region SearchBox Events Control
        private void UserSearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserSearchTextBox.Text == "Search")
            {
                UserSearchTextBox.Text = "";
                UserSearchTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        private void UserSearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                SearchText = UserSearchTextBox.Text;

                if (UserSearchTextBox.Text != "Search")
                {
                    if (SearchBoxWorker.IsBusy == false)
                    {
                        SearchBoxWorker = new BackgroundWorker();
                        SearchBoxWorker.WorkerReportsProgress = true;
                        SearchBoxWorker.WorkerSupportsCancellation = true;
                        SearchBoxWorker.DoWork += SearchBoxWorker_DoWork;
                        SearchBoxWorker.ProgressChanged += SearchBoxWorker_ProgressChanged;
                        SearchBoxWorker.RunWorkerCompleted += SearchBoxWorker_RunWorkerCompleted;

                        if (workerTimers.WorkerSupportsCancellation == true)
                        { workerTimers.CancelAsync(); }
                        SearchText = UserSearchTextBox.Text;
                        SearchBoxWorker.RunWorkerAsync();
                        IconSearch.Visibility = System.Windows.Visibility.Hidden;
                        loadingSearch.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        if (SearchBoxWorker.WorkerSupportsCancellation == true)
                        { SearchBoxWorker.CancelAsync(); }
                    }
                }
                SearchText = UserSearchTextBox.Text;
            }
            catch { }
        }
        private void UserSearchTextBox_KeyDown_1(object sender, KeyEventArgs e)
        {
            SearchText = UserSearchTextBox.Text;
        }
        private void UserSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchText = UserSearchTextBox.Text;
        }
        private void UserSearchTextBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            SearchText = UserSearchTextBox.Text;
        }
        private void UserSearchTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            SearchText = UserSearchTextBox.Text;
        }

        #endregion

        #region Get Clicked User Profile #BackroundWorker
        void GetProfileWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            if (GetProfileWorker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            else
            {
                if (CheckForInternetConnection())
                {
                    string PostProfiles = "";
                    var asd = "";
                    foreach (var ProfileItem in lbTodoList.Items)
                    {
                        asd = (ProfileItem as TodoItem).user_id;
                        PostProfiles += asd + ",";
                    }
                    var result = PostProfiles;
                    using (var client = new WebClient())
                    {
                        client.Proxy = null;
                        client.QueryString = null;

                        var values = new NameValueCollection();
                        values["user_id"] = IDUser;
                        values["usersIDs"] = result;
                        values["s"] = session_ID;
                        var ProfileListPost = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_multi_users", values);
                        var ProfileListPostresponseString = Encoding.Default.GetString(ProfileListPost);
                        var dictProfileList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ProfileListPostresponseString);
                        string ApiStatus = dictProfileList["api_status"].ToString();
                        if (ApiStatus == "200")
                        {
                            var gg = dictProfileList["users"];
                            var Profiles = JObject.Parse(ProfileListPostresponseString).SelectToken("users").ToString();
                            JArray Profileusers = JArray.Parse(Profiles);

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
        void GetProfileWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

            if (workerTimers.WorkerSupportsCancellation == true)
            { workerTimers.CancelAsync(); }
        }
        void GetProfileWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (SearchBoxWorker.WorkerSupportsCancellation == true)
                {
                    SearchBoxWorker.CancelAsync();
                }

            }
            catch { }

        }
        #endregion

        #region Get clicked User FriendsList\LastPosts #BackroundWorker
        void PostFriendWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (PostWorker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            try
            {
                if (CheckForInternetConnection())
                {
                    using (var client = new WebClient())
                    {
                        client.Proxy = null;
                        client.QueryString = null;
                        var values = new NameValueCollection();

                        //Json request Controls the date respone from the server filterd by this values
                        values["user_id"] = IDUser;
                        values["user_profile_id"] = UserPostID;
                        values["s"] = session_ID;
                        values["not_include"] = "get_post_comments,publisher,photo_album";
                        values["sub_text_limit"] = "40";
                        values["filter_by"] = "text";
                        values["friends"] = "true";
                        values["limit_friends"] = "6";

                        var PostListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_user_posts", values);
                        var PostListresponseString = Encoding.Default.GetString(PostListresponse);
                        var dictPostList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(PostListresponseString);
                        string ApiStatus = dictPostList["api_status"].ToString();
                        if (ApiStatus == "200")
                        {
                            var gg = dictPostList["posts"];
                            var users = JObject.Parse(PostListresponseString).SelectToken("posts").ToString();
                            JArray ProfilePosts = JArray.Parse(users);

                            Postitems.Clear();
                            foreach (var Post in ProfilePosts)
                            {
                                JObject PostUserdata = JObject.FromObject(Post);
                                var JS_User_ID_ = PostUserdata["user_id"].ToString();
                                var JS_postText = PostUserdata["Orginaltext"].ToString();
                                var JS_url = PostUserdata["url"].ToString();
                                var JS_TimeText = PostUserdata["time_text"].ToString();
                                var JS_PostYoutube = PostUserdata["postYoutube"].ToString();
                                var JS_PostDailymotion = PostUserdata["postDailymotion"].ToString();
                                string result = Regex.Replace(JS_postText, @"\r\n?|\n", "");
                                string JsCorrectingText = result.Replace("   ", " ").Replace("  ", " ");

                                Postitems.Add(new PostItem() { user_id = JS_User_ID_, time_text = JS_TimeText, url = JS_url, Orginaltext = JsCorrectingText, postYoutube = JS_PostYoutube, postDailymotion = JS_PostDailymotion, });
                            }

                            if (ProfilePosts.Count() <= 0)
                            {
                                Postitems.Clear();
                                Postitems.Add(new PostItem() { user_id = UserPostID, time_text = "Unavailable ", url = "no", Orginaltext = "No Post Found", postYoutube = "No Post Found", postDailymotion = "No Post Found", });
                            }
                        }

                        else
                        {
                            Postitems.Clear();
                            Postitems.Add(new PostItem() { user_id = UserPostID, time_text = "Unavailable", url = "no", Orginaltext = "No Post Found", postYoutube = "No Post Found", postDailymotion = "No Post Found", });
                        }
                        var FrendsStatus = dictPostList["friends"];
                        var Frends = JObject.Parse(PostListresponseString).SelectToken("friends").ToString();
                        JArray FrendsPosts = JArray.Parse(Frends);
                        if (FrendsPosts.Count() > 0)
                        {
                            Frendlistitems.Clear();
                            string connectivity_system = dictPostList["connectivity_system"].ToString();
                            friendtext = connectivity_system;

                            foreach (var friend in FrendsPosts)
                            {
                                JObject FrendsUserdata = JObject.FromObject(friend);
                                var JSuser_id = FrendsUserdata["user_id"].ToString();
                                var JSusername = FrendsUserdata["username"].ToString();
                                var JSname = FrendsUserdata["name"].ToString();
                                var JSavatar = FrendsUserdata["avatar"].ToString();
                                var JSurl = FrendsUserdata["url"].ToString();
                                var ProfileImage2 = new BitmapImage();

                                string lastItemOfSplit = JSavatar.Split('/').Last();
                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + JSuser_id + @"\";
                                var path2 = path + lastItemOfSplit;
                                if (CashSystem(path2) == "0")
                                { DirectoryInfo di = Directory.CreateDirectory(path); client.DownloadFile(JSavatar, path2); }


                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                {
                                    ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                    ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                }
                                ProfileImage2.Freeze();

                                Frendlistitems.Add(new FriendsItem() { user_id = JSuser_id, name = JSname, username = JSusername, url = JSurl, avatar = ProfileImage2 });
                            }
                        }
                        else
                        {
                            if (FrendsPosts.Count() <= 0)
                            {
                                string connectivity_system = dictPostList["connectivity_system"].ToString();
                                friendtext = connectivity_system;
                                Frendlistitems.Clear();
                                Frendlistitems.Add(new FriendsItem() { user_id = "0000", name = "Not available", username = "User friends are not available or set as private", url = "Not available", });
                            }
                        }

                    }
                }
              

               
            }
            catch { }
        }
        void PostFriendWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        void PostFriendWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (PostWorker.WorkerSupportsCancellation == true)
            {
                PostWorker.CancelAsync();
            }
            try
            {
                ChatMessgaeslist.Items.Refresh();
                if (Frendlistitems.Count() <= 1)
                {
                    FrendListNotavailble.Visibility = System.Windows.Visibility.Visible;
                    FrendsList.Visibility = System.Windows.Visibility.Hidden;

                    if (friendtext == "")
                    {
                        FriendText.Text = "Friends";
                    }
                    else
                    {
                        FriendText.Text = friendtext;
                    }
                    FrendListNotavailble.ItemsSource = Frendlistitems;
                    FrendListNotavailble.Items.Refresh();
                }
                else
                {
                    FrendListNotavailble.Visibility = System.Windows.Visibility.Hidden;
                    FrendsList.Visibility = System.Windows.Visibility.Visible;
                    if (friendtext == "")
                    {
                        FriendText.Text = "Friends";
                    }
                    else
                    {
                        FriendText.Text = friendtext;
                    }
                    FrendsList.ItemsSource = Frendlistitems;
                    FrendsList.Items.Refresh();
                }
                loadingfrendsellipse.Visibility = System.Windows.Visibility.Hidden;
                loadingpostsellipse.Visibility = System.Windows.Visibility.Hidden;
                FriendIcon.Visibility = System.Windows.Visibility.Visible;
                LastPostIcon.Visibility = System.Windows.Visibility.Visible;

                PostList.ItemsSource = Postitems;
                PostList.Items.Refresh();

            }
            catch { }


        }
        #endregion

        #region Get Messages from UserList #Backround Worker
        void MessageWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (MessageWorker.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }
            try
            {
                if (!MessageWorker.CancellationPending && CheckForInternetConnection())
                {
                    using (var client = new WebClient())
                    {
                        var values = new NameValueCollection();
                        values["user_id"] = IDUser;
                        values["recipient_id"] = chatuserid;
                        values["s"] = session_ID;
                        values["before_message_id"] = "0";
                        values["after_message_id"] = "0";

                        var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_user_messages", values);
                        var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                        var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                        string ApiStatus = dictChatusersList["api_status"].ToString();

                        if (ApiStatus == "200")
                        {
                            if (MessageWorker.CancellationPending == true)
                            {
                                e.Cancel = true;
                                return;
                            }
                            var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                            var gg = dictChatusersList["messages"];
                            var messages = JObject.Parse(ChatusersListresponseString).SelectToken("messages").ToString();

                            var pathData = Settings.FolderDestination + Settings.SiteName + @"\\Data\\" + chatuserid + "\\";
                            var pathData2 = pathData + chatuserid + "_M.json";
                            DirectoryInfo di2 = Directory.CreateDirectory(pathData);

                            System.IO.File.WriteAllText(pathData2, messages);

                            JArray ChatMessages = JArray.Parse(messages);
                            if (ChatMessages.Count() == 0)
                            {
                                return;
                            }
                            else
                            {

                                if (CatchMessageCompare == ChatMessages.Last().First().First().ToString())
                                {
                                    return;
                                }
                                else
                                {
                                    #region Foreach
                                    MSGitems.Clear();
                                    foreach (var MessageInfo in ChatMessages)
                                    {
                                        JObject ChatlistUserdata = JObject.FromObject(MessageInfo);
                                        var Blal = ChatlistUserdata["messageUser"];
                                        var avatar = Blal["avatar"].ToString();
                                        var Position = MessageInfo["position"].ToString();
                                        var TextMsg = MessageInfo["text"].ToString();
                                        var Type = MessageInfo["type"].ToString();
                                        var TimeMsg = MessageInfo["time_text"].ToString();
                                        var msgID = MessageInfo["id"].ToString();

                                        StringWriter myWriter = new StringWriter();
                                        HttpUtility.HtmlDecode(TextMsg, myWriter);


                                        TextMsg = myWriter.ToString();

                                        var Widthresult = "";
                                        var VisibiltyEvent = "";
                                        if (TextMsg != "")
                                        {
                                            VisibiltyEvent = "Visible";
                                            if (TextMsg.Length >= 0 && TextMsg.Length <= 3)
                                            { Widthresult = "35"; }
                                            if (TextMsg.Length > 3 && TextMsg.Length <= 6)
                                            { Widthresult = "45"; }
                                            if (TextMsg.Length > 6 && TextMsg.Length <= 9)
                                            { Widthresult = "80"; }
                                            if (TextMsg.Length > 9 && TextMsg.Length <= 13)
                                            { Widthresult = "105"; }
                                            if (TextMsg.Length > 10 && TextMsg.Length <= 13)
                                            { Widthresult = "105"; }
                                            if (TextMsg.Length >= 10 && TextMsg.Length <= 15)
                                            { Widthresult = "140"; }
                                            if (TextMsg.Length > 15 && TextMsg.Length <= 18)
                                            { Widthresult = "155"; }
                                            if (TextMsg.Length > 18 && TextMsg.Length <= 23)
                                            { Widthresult = "155"; }
                                            if (TextMsg.Length >= 20 && TextMsg.Length <= 26)
                                            { Widthresult = "175"; }
                                            if (TextMsg.Length >= 26 && TextMsg.Length <= 30)
                                            { Widthresult = "200"; }
                                            if (TextMsg.Length >= 31 && TextMsg.Length <= 35)
                                            { Widthresult = "250"; }
                                            if (TextMsg.Length >= 36 && TextMsg.Length <= 45)
                                            { Widthresult = "300"; }
                                        }
                                        else
                                        {
                                            VisibiltyEvent = "Collapsed";
                                            Widthresult = "0";
                                        }


                                        if (Position == "left")
                                        {
                                            try
                                            {
                                                #region Left Text
                                                if (Type == "left_text")
                                                {
                                                    string lastItemOfSplit = avatar.Split('/').Last();
                                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                    var path2 = path + lastItemOfSplit;

                                                    var ProfileImage2 = new BitmapImage();
                                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                    {
                                                        ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 40; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                                        ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                                    }
                                                    ProfileImage2.Freeze();
                                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, Type = Type, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, UserImage = ProfileImage2, messageID = msgID });

                                                }
                                                #endregion
                                                #region Left Image
                                                if (Type == "left_image")
                                                {
                                                    var ImageUrl = MessageInfo["media"].ToString();
                                                    string lastItemOfSplit = ImageUrl.Split('/').Last();
                                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                    var path2 = path + lastItemOfSplit;

                                                    var Image = new BitmapImage();
                                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                    {
                                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                    }

                                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, Type = Type, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID, DownloadFileUrl = path2 });

                                                }
                                                #endregion
                                                #region Left File
                                                if (Type == "left_file")
                                                {
                                                    var fileUrl = MessageInfo["media"].ToString();
                                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                                    var MediaSize = MessageInfo["file_size"].ToString();
                                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                                    if (FileName.Length >= 35)
                                                    {
                                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                                    }
                                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                                    string lastItemOfSplit = avatar.Split('/').Last();
                                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                    var path2 = path + lastItemOfSplit;

                                                    var ProfileImage2 = new BitmapImage();
                                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                    {
                                                        ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                                        ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                                    }
                                                    ProfileImage2.Freeze();


                                                    var Image = new BitmapImage();
                                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png", FileMode.Open, FileAccess.Read))
                                                    {
                                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                    }

                                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                                                }
                                                #endregion
                                                #region Left Video
                                                if (Type == "left_video")
                                                {
                                                    var fileUrl = MessageInfo["media"].ToString();
                                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                                    var MediaSize = MessageInfo["file_size"].ToString();
                                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                                    if (FileName.Length >= 35)
                                                    {
                                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                                    }
                                                    string lastItemOfSplit = avatar.Split('/').Last();
                                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                    var path2 = path + lastItemOfSplit;

                                                    var ProfileImage2 = new BitmapImage();
                                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                    {
                                                        ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                                        ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                                    }
                                                    ProfileImage2.Freeze();


                                                    var Image = new BitmapImage();
                                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png", FileMode.Open, FileAccess.Read))
                                                    {
                                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                    }

                                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                                                }
                                                #endregion
                                                #region Left Sound
                                                if (Type == "left_audio")
                                                {
                                                    var fileUrl = MessageInfo["media"].ToString();
                                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                                    var MediaSize = MessageInfo["file_size"].ToString();
                                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                                    if (FileName.Length >= 35)
                                                    {
                                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                                    }

                                                    string lastItemOfSplit = avatar.Split('/').Last();
                                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                    var path2 = path + lastItemOfSplit;

                                                    var ProfileImage2 = new BitmapImage();
                                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                    {
                                                        ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                                        ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                                    }
                                                    ProfileImage2.Freeze();

                                                    var Image = new BitmapImage();
                                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png", FileMode.Open, FileAccess.Read))
                                                    {
                                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                    }

                                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                                                }
                                                #endregion
                                            }
                                            catch 
                                            { }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                #region right text
                                                if (Type == "right_text")
                                                {
                                                    MSGitems.Add(new MessageItem() { Content = TextMsg, Type = Type, CreatedAt = TimeMsg, WidthWrap = Widthresult, messageID = msgID });
                                                }
                                                #endregion
                                                #region right Image
                                                if (Type == "right_image")
                                                {
                                                    var ImageUrl = MessageInfo["media"].ToString();
                                                    string lastItemOfSplit = ImageUrl.Split('/').Last();
                                                    var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                    var path2 = path + lastItemOfSplit;

                                                    var Image = new BitmapImage();
                                                    using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                    {
                                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                    }

                                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, WidthWrap = Widthresult, Type = Type, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID, DownloadFileUrl = path2 });
                                                }
                                                #endregion
                                                #region right File
                                                if (Type == "right_file")
                                                {
                                                    var fileUrl = MessageInfo["media"].ToString();
                                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                                    var MediaSize = MessageInfo["file_size"].ToString();
                                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                                    if (FileName.Length >= 35)
                                                    {
                                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                                    }
                                                    var Image = new BitmapImage();
                                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png", FileMode.Open, FileAccess.Read))
                                                    {
                                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                    }

                                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });

                                                }
                                                #endregion
                                                #region Left Video
                                                if (Type == "right_video")
                                                {
                                                    var fileUrl = MessageInfo["media"].ToString();
                                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                                    var MediaSize = MessageInfo["file_size"].ToString();
                                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                                    if (FileName.Length >= 35)
                                                    {
                                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                                    }
                                                    var Image = new BitmapImage();
                                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png", FileMode.Open, FileAccess.Read))
                                                    {
                                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                    }

                                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });
                                                }
                                                #endregion
                                                #region Left Sound
                                                if (Type == "left_audio")
                                                {
                                                    var fileUrl = MessageInfo["media"].ToString();
                                                    var FileName = MessageInfo["mediaFileName"].ToString();
                                                    var MediaSize = MessageInfo["file_size"].ToString();
                                                    var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;
                                                    var f = FileName.Split(new char[] { ' ', '_', '-', '+', '.' });
                                                    if (FileName.Length >= 35)
                                                    {
                                                        if (f.Count() >= 3) { FileName = f.First() + ".." + f.Last(); }
                                                        else { var Truncater = Truncate(FileName, 30); FileName = Truncater + f.Last(); }
                                                    }
                                                    var Image = new BitmapImage();
                                                    using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png", FileMode.Open, FileAccess.Read))
                                                    {
                                                        Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                        Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                    }
                                                    MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });
                                                }
                                                #endregion
                                            }
                                            catch 
                                            { }
                                        }

                                    }
                                    if (MessageWorker.CancellationPending == true)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }
                                    #endregion
                                }

                            }
                        }
                    }
                }
            }
            catch 
            { }

        }
        void MessageWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        void MessageWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (MessageWorker.WorkerSupportsCancellation == true)
            {
                MessageWorker.CancelAsync();
            }
            try
            {
                ChatSeen.Text = LastseenText.Text;
                if (MSGitems.Count() == 0)
                {
                    NoMessagePanel.Visibility = System.Windows.Visibility.Visible;

                    var NoMessageBlock = this.FindResource("NoMessageBlock") as TextBlock;
                    NoMessageBlock.Text = "No messages yet";
                    return;
                }
                else
                {
                    if (CatchMessageCompare == MSGitems.Last().messageID)
                    {

                    }
                    else
                    {
                        ChatMessgaeslist.ItemsSource = MSGitems;
                    }

                    ListBoxAutomationPeer svAutomation = (ListBoxAutomationPeer)ScrollViewerAutomationPeer.CreatePeerForElement(ChatMessgaeslist);
                    IScrollProvider scrollInterface = (IScrollProvider)svAutomation.GetPattern(PatternInterface.Scroll);
                    System.Windows.Automation.ScrollAmount scrollVertical = System.Windows.Automation.ScrollAmount.LargeIncrement;
                    System.Windows.Automation.ScrollAmount scrollHorizontal = System.Windows.Automation.ScrollAmount.NoAmount;
                    //If the vertical scroller is not available, the operation cannot be performed, which will raise an exception. 
                    if (scrollInterface.VerticallyScrollable)
                        scrollInterface.Scroll(scrollHorizontal, scrollVertical);
                    ChatMessgaeslist.SelectedIndex = ChatMessgaeslist.Items.Count - 1;
                    ChatMessgaeslist.ScrollIntoView(ChatMessgaeslist.SelectedItem);

                }



            }
            catch { }


        }
        #endregion

        #region Messages Updater\Checker #Backround Worker
        private static string TypingStatus = "Online";
        void MessageUpdaterTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            if (MessageUpdaterTimer.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }
            BackgroundWorker worker = (BackgroundWorker)sender;
            if (CheckForInternetConnection()==false)
            {
                return;
            }
            while (!worker.CancellationPending)
            {
                Thread.Sleep(Settings.Update_Message_Receiver_INT);
                try
                {
                    if (MSGitems.Count() == 0)
                    {
                        return;
                    }
                    else
                    {
                        var Updater = MSGitems.SingleOrDefault(d => d.messageID == time2);

                        if (Updater != null)
                        {
                            return;
                        }
                        else
                        {
                            LastMessageid = MSGitems.Last().messageID.ToString();
                        }
                        using (var client = new WebClient())
                        {
                            if (MessageUpdaterTimer.CancellationPending == true)
                            {
                                e.Cancel = true;
                                return;
                            }
                            var values = new NameValueCollection();
                            values["user_id"] = IDUser;
                            values["recipient_id"] = chatuserid;
                            values["s"] = session_ID;
                            values["before_message_id"] = "0";
                            values["after_message_id"] = LastMessageid;

                            var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_user_messages", values);
                            var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                            var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                            string ApiStatus = dictChatusersList["api_status"].ToString();

                            if (ApiStatus == "200")
                            {
                                var TypingEvent = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=check_typing", values);
                                var TypingEventResponseString = Encoding.Default.GetString(ChatusersListresponse);
                                var DictTypingEvent = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                                string ApiStatus2 = dictChatusersList["api_status"].ToString();
                                if (ApiStatus2 == "200")
                                {
                                    string TypingEventStatus = dictChatusersList["typing"].ToString();
                                    if (TypingEventStatus == "1") { TypingStatus = "Typping"; }
                                    else { TypingStatus = "Normal"; }
                                }
                                var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                                var gg = dictChatusersList["messages"];
                                var messages = JObject.Parse(ChatusersListresponseString).SelectToken("messages").ToString();
                                JArray ChatMessages = JArray.Parse(messages);
                                if (ChatMessages.Count == 0)
                                {
                                    LoadmoremessgaCount = "0";
                                    return;
                                }
                                else
                                {
                                    LoadmoremessgaCount = "1";
                                    foreach (var MessageInfo in ChatMessages)
                                    {
                                        JObject ChatlistUserdata = JObject.FromObject(MessageInfo);
                                        var Blal = ChatlistUserdata["messageUser"];
                                        var avatar = Blal["avatar"].ToString();
                                        var Position = MessageInfo["position"].ToString();
                                        var TextMsg = MessageInfo["text"].ToString();
                                        var Type = MessageInfo["type"].ToString();
                                        var TimeMsg = MessageInfo["time_text"].ToString();
                                        var msgID = MessageInfo["id"].ToString();

                                        StringWriter myWriter = new StringWriter();
                                        HttpUtility.HtmlDecode(TextMsg, myWriter);
                                        var pathData = Settings.FolderDestination + Settings.SiteName + @"\\Data\\" + chatuserid + "\\";
                                        var pathData2 = pathData + CacheUserID + "_M.json";
                                        DirectoryInfo di2 = Directory.CreateDirectory(pathData);
                                        string Json = messages.ToString();
                                        System.IO.File.WriteAllText(pathData2, Json);

                                        TextMsg = myWriter.ToString();

                                        var Widthresult = "";
                                        var VisibiltyEvent = "";
                                        if (TextMsg != "")
                                        {
                                            VisibiltyEvent = "Visible";
                                            if (TextMsg.Length >= 0 && TextMsg.Length <= 3)
                                            { Widthresult = "35"; }
                                            if (TextMsg.Length > 3 && TextMsg.Length <= 6)
                                            { Widthresult = "50"; }
                                            if (TextMsg.Length > 6 && TextMsg.Length <= 9)
                                            { Widthresult = "85"; }
                                            if (TextMsg.Length > 9 && TextMsg.Length <= 13)
                                            { Widthresult = "105"; }
                                            if (TextMsg.Length > 10 && TextMsg.Length <= 13)
                                            { Widthresult = "105"; }
                                            if (TextMsg.Length >= 10 && TextMsg.Length <= 15)
                                            { Widthresult = "140"; }
                                            if (TextMsg.Length > 15 && TextMsg.Length <= 18)
                                            { Widthresult = "155"; }
                                            if (TextMsg.Length > 18 && TextMsg.Length <= 23)
                                            { Widthresult = "155"; }
                                            if (TextMsg.Length >= 20 && TextMsg.Length <= 26)
                                            { Widthresult = "175"; }
                                            if (TextMsg.Length >= 26 && TextMsg.Length <= 30)
                                            { Widthresult = "200"; }
                                            if (TextMsg.Length >= 31 && TextMsg.Length <= 35)
                                            { Widthresult = "250"; }
                                            if (TextMsg.Length >= 36 && TextMsg.Length <= 45)
                                            { Widthresult = "300"; }
                                        }
                                        else
                                        {
                                            VisibiltyEvent = "Collapsed";
                                            Widthresult = "0";
                                        }
                                        if (MessageUpdaterTimer.CancellationPending == true)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }
                                        if (Position == "left")
                                        {
                                            #region Left Text
                                            if (Type == "left_text")
                                            {
                                                string lastItemOfSplit = avatar.Split('/').Last();
                                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                var path2 = path + lastItemOfSplit;
                                                if (CashSystem(path2) == "0")
                                                {
                                                    if (!Directory.Exists(path2))
                                                    {
                                                        DirectoryInfo di = Directory.CreateDirectory(path2);
                                                    }

                                                    client.DownloadFile(avatar, path2);
                                                }
                                                var ProfileImage2 = new BitmapImage();
                                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                {
                                                    ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                                    ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                                }
                                                ProfileImage2.Freeze();
                                                MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, Type = Type, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, UserImage = ProfileImage2, messageID = msgID });

                                            }
                                            #endregion
                                            #region Left Image
                                            if (Type == "left_image")
                                            {
                                                var ImageUrl = MessageInfo["media"].ToString();
                                                string lastItemOfSplit = ImageUrl.Split('/').Last();
                                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                var path2 = path + lastItemOfSplit;
                                                if (CashSystem(path2) == "0")
                                                {
                                                    if (!Directory.Exists(path))
                                                    { DirectoryInfo di = Directory.CreateDirectory(path2); }
                                                    client.DownloadFile(ImageUrl, path2);
                                                }

                                                var Image = new BitmapImage();
                                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                {
                                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                }

                                                MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, Type = Type, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID, DownloadFileUrl = path2 });

                                            }
                                            #endregion
                                            #region Left File
                                            if (Type == "left_file")
                                            {
                                                var fileUrl = MessageInfo["media"].ToString();
                                                var FileName = MessageInfo["mediaFileName"].ToString();
                                                var MediaSize = MessageInfo["file_size"].ToString();

                                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                                string lastItemOfSplit = avatar.Split('/').Last();
                                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                var path2 = path + lastItemOfSplit;
                                                if (CashSystem(path2) == "0")
                                                {
                                                    if (!Directory.Exists(path2))
                                                    {
                                                        DirectoryInfo di = Directory.CreateDirectory(path2);
                                                    }

                                                    client.DownloadFile(avatar, path2);
                                                }
                                                var ProfileImage2 = new BitmapImage();
                                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                {
                                                    ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                                    ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                                }
                                                ProfileImage2.Freeze();

                                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png") == "0")
                                                {
                                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                                    client.DownloadFile(Settings.WebsiteUrl + @"\Download.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png");
                                                }

                                                var Image = new BitmapImage();
                                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png", FileMode.Open, FileAccess.Read))
                                                {
                                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                }

                                                MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                                            }
                                            #endregion
                                            #region Left Video
                                            if (Type == "left_video")
                                            {
                                                var fileUrl = MessageInfo["media"].ToString();
                                                var FileName = MessageInfo["mediaFileName"].ToString();
                                                var MediaSize = MessageInfo["file_size"].ToString();
                                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                                string lastItemOfSplit = avatar.Split('/').Last();
                                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                var path2 = path + lastItemOfSplit;
                                                if (CashSystem(path2) == "0")
                                                {
                                                    if (!Directory.Exists(path2))
                                                    {
                                                        DirectoryInfo di = Directory.CreateDirectory(path2);
                                                    }

                                                    client.DownloadFile(avatar, path2);
                                                }
                                                var ProfileImage2 = new BitmapImage();
                                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                {
                                                    ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                                    ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                                }
                                                ProfileImage2.Freeze();

                                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png") == "0")
                                                {
                                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                                    client.DownloadFile(Settings.WebsiteUrl + @"\Video File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png");
                                                }

                                                var Image = new BitmapImage();
                                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png", FileMode.Open, FileAccess.Read))
                                                {
                                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                }

                                                MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                                            }
                                            #endregion
                                            #region Left Sound
                                            if (Type == "left_audio")
                                            {
                                                var fileUrl = MessageInfo["media"].ToString();
                                                var FileName = MessageInfo["mediaFileName"].ToString();
                                                var MediaSize = MessageInfo["file_size"].ToString();
                                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                                string lastItemOfSplit = avatar.Split('/').Last();
                                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                var path2 = path + lastItemOfSplit;
                                                if (CashSystem(path2) == "0")
                                                {
                                                    if (!Directory.Exists(path2))
                                                    {
                                                        DirectoryInfo di = Directory.CreateDirectory(path2);
                                                    }

                                                    client.DownloadFile(avatar, path2);
                                                }
                                                var ProfileImage2 = new BitmapImage();
                                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                {
                                                    ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                                    ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                                }
                                                ProfileImage2.Freeze();

                                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png") == "0")
                                                {
                                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                                    client.DownloadFile(Settings.WebsiteUrl + @"\Audio File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png");
                                                }

                                                var Image = new BitmapImage();
                                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png", FileMode.Open, FileAccess.Read))
                                                {
                                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                }

                                                MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                                            }
                                            #endregion
                                            if (MessageUpdaterTimer.CancellationPending == true)
                                            {
                                                e.Cancel = true;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            #region right text
                                            if (Type == "right_text")
                                            {
                                                MSGitems.Add(new MessageItem() { Content = TextMsg, Type = Type, CreatedAt = TimeMsg, WidthWrap = Widthresult, messageID = msgID });
                                            }
                                            #endregion
                                            #region right Image
                                            if (Type == "right_image")
                                            {
                                                var ImageUrl = MessageInfo["media"].ToString();
                                                string lastItemOfSplit = ImageUrl.Split('/').Last();
                                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                                var path2 = path + lastItemOfSplit;
                                                if (CashSystem(path2) == "0")
                                                {
                                                    if (!Directory.Exists(path))
                                                    { DirectoryInfo di = Directory.CreateDirectory(path2); }
                                                    client.DownloadFile(ImageUrl, path2);
                                                }

                                                var Image = new BitmapImage();
                                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                                {
                                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                }

                                                MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, WidthWrap = Widthresult, Type = Type, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID, DownloadFileUrl = path2 });
                                            }
                                            #endregion
                                            #region right File
                                            if (Type == "right_file")
                                            {
                                                var fileUrl = MessageInfo["media"].ToString();
                                                var FileName = MessageInfo["mediaFileName"].ToString();
                                                var MediaSize = MessageInfo["file_size"].ToString();
                                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png") == "0")
                                                {
                                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                                    client.DownloadFile(Settings.WebsiteUrl + @"\Download.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png");
                                                }

                                                var Image = new BitmapImage();
                                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png", FileMode.Open, FileAccess.Read))
                                                {
                                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                }

                                                MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });

                                            }
                                            #endregion
                                            #region right Video
                                            if (Type == "right_video")
                                            {
                                                var fileUrl = MessageInfo["media"].ToString();
                                                var FileName = MessageInfo["mediaFileName"].ToString();
                                                var MediaSize = MessageInfo["file_size"].ToString();
                                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png") == "0")
                                                {
                                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                                    client.DownloadFile(Settings.WebsiteUrl + @"\Video File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png");
                                                }

                                                var Image = new BitmapImage();
                                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png", FileMode.Open, FileAccess.Read))
                                                {
                                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                }

                                                MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });
                                            }
                                            #endregion
                                            #region right Sound
                                            if (Type == "right_audio")
                                            {
                                                var fileUrl = MessageInfo["media"].ToString();
                                                var FileName = MessageInfo["mediaFileName"].ToString();
                                                var MediaSize = MessageInfo["file_size"].ToString();
                                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png") == "0")
                                                {
                                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                                    client.DownloadFile(Settings.WebsiteUrl + @"\Audio File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png");
                                                }

                                                var Image = new BitmapImage();
                                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png", FileMode.Open, FileAccess.Read))
                                                {
                                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                                }
                                                MSGitems.Add(new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });
                                            }
                                            #endregion
                                            if (MessageUpdaterTimer.CancellationPending == true)
                                            {
                                                e.Cancel = true;
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    worker.Dispose();
                    worker.CancelAsync();
                }
                catch
                {}

            }

        }
        void MessageUpdaterTimer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object userObject = e.UserState;
            int percentage = e.ProgressPercentage;
        }
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private const String APP_ID = "Microsoft.Samples.DesktopToastsSample";
        void MessageUpdaterTimer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
          
            var bw2 = new BackgroundWorker();
            bw2.DoWork += (o, args) => AddNewMessagetoCash();
            bw2.RunWorkerAsync();
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (MessageUpdaterTimer.WorkerSupportsCancellation == true)
                {
                    MessageUpdaterTimer.CancelAsync();
                }
                try
                {
                    if (TypingStatus == "Typping")
                    {
                        ChatSeen.Text = "Typping...";
                    }
                    else
                    {
                        ChatSeen.Text = LastseenText.Text;
                    }
                    if (LoadmoremessgaCount == "0")
                    {
                        ChatMessgaeslist.SelectedIndex = ChatMessgaeslist.Items.Count - 1;
                        if (ChatMessgaeslist.SelectedItem != null)
                            LastMessageid = (ChatMessgaeslist.SelectedItem as MessageItem).messageID;
                        //ChatMessgaeslist.ScrollIntoView(ChatMessgaeslist.SelectedItem);
                        MessageUpdaterTimer.RunWorkerAsync();
                        return;
                    }
                    else
                    {
                        ChatMessgaeslist.ItemsSource = MSGitems;
                        //ChatMessgaeslist.Items.Refresh();
                       
                        ChatMessgaeslist.SelectedIndex = ChatMessgaeslist.Items.Count - 1;
                        if (ChatMessgaeslist.SelectedItem != null)
                            LastMessageid = MSGitems.Last().messageID;

                        ListBoxAutomationPeer svAutomation = (ListBoxAutomationPeer)ScrollViewerAutomationPeer.CreatePeerForElement(ChatMessgaeslist);
                        IScrollProvider scrollInterface = (IScrollProvider)svAutomation.GetPattern(PatternInterface.Scroll);
                        System.Windows.Automation.ScrollAmount scrollVertical = System.Windows.Automation.ScrollAmount.LargeIncrement;
                        System.Windows.Automation.ScrollAmount scrollHorizontal = System.Windows.Automation.ScrollAmount.NoAmount;
                        //If the vertical scroller is not available, the operation cannot be performed, which will raise an exception. 
                        if (scrollInterface.VerticallyScrollable)
                            scrollInterface.Scroll(scrollHorizontal, scrollVertical);

                        if (MessageUpdaterTimer.WorkerSupportsCancellation == true)
                        { MessageUpdaterTimer.CancelAsync(); }


                        if (this.WindowState == WindowState.Minimized)
                        {
                            ContentMsg = MSGitems.Last().Content;
                            var msgfrom = FullNameText.Text;
                            var userImage = MSGitems.Last().UserImage;
                            Profilename = "New Message from " + FullNameText.Text;
                            ProfilePicSourse = ProfilePicture.Source.ToString();
                            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
                            {
                                MsgPopupWindow PopUp = new MsgPopupWindow();
                                PopUp.Activate();
                                PopUp.Show();
                                PopUp.Activate();
                                var workingAreaaa = System.Windows.SystemParameters.WorkArea;
                                var transform = PresentationSource.FromVisual(PopUp).CompositionTarget.TransformFromDevice;
                                var corner = transform.Transform(new Point(workingAreaaa.Right, workingAreaaa.Bottom));

                                PopUp.Left = corner.X - PopUp.ActualWidth - 10;
                                PopUp.Top = corner.Y - PopUp.ActualHeight;
                                String SoundPath = Path.GetFullPath("New-message.mp3");
                                mediaPlayer.Open(new Uri(SoundPath));
                                mediaPlayer.Play();
                            }));
                        }

                        MessageUpdaterTimer.RunWorkerAsync();
                    }
                    //MessageUpdaterTimer = new BackgroundWorker();
                    //MessageUpdaterTimer.WorkerReportsProgress = true;
                    //MessageUpdaterTimer.WorkerSupportsCancellation = true;
                    //MessageUpdaterTimer.DoWork += MessageUpdaterTimer_DoWork;
                    //MessageUpdaterTimer.ProgressChanged += MessageUpdaterTimer_ProgressChanged;
                    //MessageUpdaterTimer.RunWorkerCompleted += MessageUpdaterTimer_RunWorkerCompleted;

                }
                catch { }
            }));

        }
        #endregion

        #region LoadMore Message  #Backround Worker
        void LoadMoreMessageWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (LoadMoreMessageWorker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            if (CheckForInternetConnection() == false)
            {
                return;
            }
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();

                values["user_id"] = IDUser;
                values["recipient_id"] = chatuserid;
                values["s"] = session_ID;
                values["before_message_id"] = FirstMessageid;
                values["after_message_id"] = "0";

                var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_user_messages", values);
                var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                string ApiStatus = dictChatusersList["api_status"].ToString();

                if (ApiStatus == "200")
                {
                    var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var gg = dictChatusersList["messages"];
                    var messages = JObject.Parse(ChatusersListresponseString).SelectToken("messages").ToString();

                    JArray ChatMessages = JArray.Parse(messages);
                    if (ChatMessages.Count == 0)
                    {
                        LoadmoremessgaCount = "0";
                    }
                    else
                    {
                        LoadmoremessgaCount = "1";
                    }
                    foreach (var MessageInfo in ChatMessages)
                    {
                        if (LoadMoreMessageWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                        }

                        JObject ChatlistUserdata = JObject.FromObject(MessageInfo);
                        var Blal = ChatlistUserdata["messageUser"];
                        var avatar = Blal["avatar"].ToString();
                        var Position = MessageInfo["position"].ToString();
                        var TextMsg = MessageInfo["text"].ToString();
                        var Type = MessageInfo["type"].ToString();
                        var TimeMsg = MessageInfo["time_text"].ToString();
                        var msgID = MessageInfo["id"].ToString();

                        StringWriter myWriter = new StringWriter();
                        HttpUtility.HtmlDecode(TextMsg, myWriter);
                        var pathData = Settings.FolderDestination + Settings.SiteName + @"\\Data\\" + chatuserid + "\\";
                        var pathData2 = pathData + CacheUserID + "_M.json";
                        DirectoryInfo di2 = Directory.CreateDirectory(pathData);
                        string Json = messages.ToString();
                        System.IO.File.WriteAllText(pathData2, Json);

                        TextMsg = myWriter.ToString();

                        var Widthresult = "";
                        var VisibiltyEvent = "";
                        if (TextMsg != "")
                        {
                            VisibiltyEvent = "Visible";
                            if (TextMsg.Length >= 0 && TextMsg.Length <= 3)
                            { Widthresult = "35"; }
                            if (TextMsg.Length > 3 && TextMsg.Length <= 6)
                            { Widthresult = "45"; }
                            if (TextMsg.Length > 6 && TextMsg.Length <= 9)
                            { Widthresult = "80"; }
                            if (TextMsg.Length > 9 && TextMsg.Length <= 13)
                            { Widthresult = "105"; }
                            if (TextMsg.Length > 10 && TextMsg.Length <= 13)
                            { Widthresult = "105"; }
                            if (TextMsg.Length >= 10 && TextMsg.Length <= 15)
                            { Widthresult = "140"; }
                            if (TextMsg.Length > 15 && TextMsg.Length <= 18)
                            { Widthresult = "155"; }
                            if (TextMsg.Length > 18 && TextMsg.Length <= 23)
                            { Widthresult = "155"; }
                            if (TextMsg.Length >= 20 && TextMsg.Length <= 26)
                            { Widthresult = "175"; }
                            if (TextMsg.Length >= 26 && TextMsg.Length <= 30)
                            { Widthresult = "200"; }
                            if (TextMsg.Length >= 31 && TextMsg.Length <= 35)
                            { Widthresult = "250"; }
                            if (TextMsg.Length >= 36 && TextMsg.Length <= 45)
                            { Widthresult = "300"; }
                        }
                        else
                        {
                            VisibiltyEvent = "Collapsed";
                            Widthresult = "0";
                        }


                        if (Position == "left")
                        {
                            #region Left Text
                            if (Type == "left_text")
                            {
                                string lastItemOfSplit = avatar.Split('/').Last();
                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                var path2 = path + lastItemOfSplit;
                                if (CashSystem(path2) == "0")
                                {
                                    if (!Directory.Exists(path2))
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(path2);
                                    }

                                    client.DownloadFile(avatar, path2);
                                }
                                var ProfileImage2 = new BitmapImage();
                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                {
                                    ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                    ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                }
                                ProfileImage2.Freeze();
                                MSGitems.Insert(0, new MessageItem() { Content = TextMsg, Type = Type, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, UserImage = ProfileImage2, messageID = msgID });

                            }
                            #endregion
                            #region Left Image
                            if (Type == "left_image")
                            {
                                var ImageUrl = MessageInfo["media"].ToString();
                                string lastItemOfSplit = ImageUrl.Split('/').Last();
                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                var path2 = path + lastItemOfSplit;
                                if (CashSystem(path2) == "0")
                                {
                                    if (!Directory.Exists(path))
                                    { DirectoryInfo di = Directory.CreateDirectory(path2); }
                                    client.DownloadFile(ImageUrl, path2);
                                }

                                var Image = new BitmapImage();
                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                {
                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                }

                                MSGitems.Insert(0, new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, Type = Type, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID, DownloadFileUrl = path2 });

                            }
                            #endregion
                            #region Left File
                            if (Type == "left_file")
                            {
                                var fileUrl = MessageInfo["media"].ToString();
                                var FileName = MessageInfo["mediaFileName"].ToString();
                                var MediaSize = MessageInfo["file_size"].ToString();

                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                string lastItemOfSplit = avatar.Split('/').Last();
                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                var path2 = path + lastItemOfSplit;
                                if (CashSystem(path2) == "0")
                                {
                                    if (!Directory.Exists(path2))
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(path2);
                                    }

                                    client.DownloadFile(avatar, path2);
                                }
                                var ProfileImage2 = new BitmapImage();
                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                {
                                    ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                    ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                }
                                ProfileImage2.Freeze();

                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png") == "0")
                                {
                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                    client.DownloadFile(Settings.WebsiteUrl + @"\Download.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png");
                                }

                                var Image = new BitmapImage();
                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png", FileMode.Open, FileAccess.Read))
                                {
                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                }

                                MSGitems.Insert(0, new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                            }
                            #endregion
                            #region Left Video
                            if (Type == "left_video")
                            {
                                var fileUrl = MessageInfo["media"].ToString();
                                var FileName = MessageInfo["mediaFileName"].ToString();
                                var MediaSize = MessageInfo["file_size"].ToString();
                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                string lastItemOfSplit = avatar.Split('/').Last();
                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                var path2 = path + lastItemOfSplit;
                                if (CashSystem(path2) == "0")
                                {
                                    if (!Directory.Exists(path2))
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(path2);
                                    }

                                    client.DownloadFile(avatar, path2);
                                }
                                var ProfileImage2 = new BitmapImage();
                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                {
                                    ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                    ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                }
                                ProfileImage2.Freeze();

                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png") == "0")
                                {
                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                    client.DownloadFile(Settings.WebsiteUrl + @"\Video File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png");
                                }

                                var Image = new BitmapImage();
                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png", FileMode.Open, FileAccess.Read))
                                {
                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                }

                                MSGitems.Insert(0, new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                            }
                            #endregion
                            #region Left Sound
                            if (Type == "left_audio")
                            {
                                var fileUrl = MessageInfo["media"].ToString();
                                var FileName = MessageInfo["mediaFileName"].ToString();
                                var MediaSize = MessageInfo["file_size"].ToString();
                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                string lastItemOfSplit = avatar.Split('/').Last();
                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                var path2 = path + lastItemOfSplit;
                                if (CashSystem(path2) == "0")
                                {
                                    if (!Directory.Exists(path2))
                                    {
                                        DirectoryInfo di = Directory.CreateDirectory(path2);
                                    }

                                    client.DownloadFile(avatar, path2);
                                }
                                var ProfileImage2 = new BitmapImage();
                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                {
                                    ProfileImage2.BeginInit(); ProfileImage2.DecodePixelWidth = 50; ProfileImage2.CacheOption = BitmapCacheOption.OnLoad;
                                    ProfileImage2.StreamSource = stream; ProfileImage2.EndInit(); stream.Close();
                                }
                                ProfileImage2.Freeze();

                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png") == "0")
                                {
                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                    client.DownloadFile(Settings.WebsiteUrl + @"\Audio File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png");
                                }

                                var Image = new BitmapImage();
                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png", FileMode.Open, FileAccess.Read))
                                {
                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                }

                                MSGitems.Insert(0, new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, UserImage = ProfileImage2, messageID = msgID });

                            }
                            #endregion
                        }
                        else
                        {
                            #region right text
                            if (Type == "right_text")
                            {
                                MSGitems.Insert(0, new MessageItem() { Content = TextMsg, Type = Type, CreatedAt = TimeMsg, WidthWrap = Widthresult, messageID = msgID });
                            }
                            #endregion
                            #region right Image
                            if (Type == "right_image")
                            {
                                var ImageUrl = MessageInfo["media"].ToString();
                                string lastItemOfSplit = ImageUrl.Split('/').Last();
                                var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + chatuserid + @"\";
                                var path2 = path + lastItemOfSplit;
                                if (CashSystem(path2) == "0")
                                {
                                    if (!Directory.Exists(path))
                                    { DirectoryInfo di = Directory.CreateDirectory(path2); }
                                    client.DownloadFile(ImageUrl, path2);
                                }

                                var Image = new BitmapImage();
                                using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                                {
                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                }

                                MSGitems.Insert(0, new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, WidthWrap = Widthresult, Type = Type, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID, DownloadFileUrl = path2 });
                            }
                            #endregion
                            #region right File
                            if (Type == "right_file")
                            {
                                var fileUrl = MessageInfo["media"].ToString();
                                var FileName = MessageInfo["mediaFileName"].ToString();
                                var MediaSize = MessageInfo["file_size"].ToString();
                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png") == "0")
                                {
                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                    client.DownloadFile(Settings.WebsiteUrl + @"\Download.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png");
                                }

                                var Image = new BitmapImage();
                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Download.png", FileMode.Open, FileAccess.Read))
                                {
                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                }

                                MSGitems.Insert(0, new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });

                            }
                            #endregion
                            #region right Video
                            if (Type == "right_video")
                            {
                                var fileUrl = MessageInfo["media"].ToString();
                                var FileName = MessageInfo["mediaFileName"].ToString();
                                var MediaSize = MessageInfo["file_size"].ToString();
                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png") == "0")
                                {
                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                    client.DownloadFile(Settings.WebsiteUrl + @"\Video File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png");
                                }

                                var Image = new BitmapImage();
                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Video File-50.png", FileMode.Open, FileAccess.Read))
                                {
                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                }

                                MSGitems.Insert(0, new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });
                            }
                            #endregion
                            #region right Sound
                            if (Type == "right_audio")
                            {
                                var fileUrl = MessageInfo["media"].ToString();
                                var FileName = MessageInfo["mediaFileName"].ToString();
                                var MediaSize = MessageInfo["file_size"].ToString();
                                var DownloadUrl = "DownloadUrlFrom>>>" + fileUrl;

                                if (CashSystem(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png") == "0")
                                {
                                    if (!Directory.Exists(Settings.FolderDestination + Settings.SiteName + @"\Icons\"))
                                    { DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + Settings.SiteName + @"\Icons\"); } ////////////// Need come back 
                                    client.DownloadFile(Settings.WebsiteUrl + @"\Audio File-50.png", Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png");
                                }

                                var Image = new BitmapImage();
                                using (var stream = new FileStream(Settings.FolderDestination + Settings.SiteName + @"\Icons\Audio File-50.png", FileMode.Open, FileAccess.Read))
                                {
                                    Image.BeginInit(); Image.CacheOption = BitmapCacheOption.OnLoad;
                                    Image.StreamSource = stream; Image.EndInit(); stream.Close(); Image.Freeze();
                                }
                                MSGitems.Insert(0, new MessageItem() { Visibilty = VisibiltyEvent, Content = TextMsg, MediaName = FileName, MediaSize = MediaSize, Type = Type, DownloadFileUrl = DownloadUrl, Position = Position, WidthWrap = Widthresult, CreatedAt = TimeMsg, ImageMedia = Image, messageID = msgID });
                            }
                            #endregion
                        }
                        if (LoadMoreMessageWorker.CancellationPending == true)
                        {
                            e.Cancel = true;
                        }
                    }
                }
            }
        }
        void LoadMoreMessageWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        void LoadMoreMessageWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                if (LoadMoreMessageWorker.WorkerSupportsCancellation == true)
                {
                    LoadMoreMessageWorker.CancelAsync();
                }
                try
                {
                    if (LoadmoremessgaCount == "0")
                    {
                        LoadmoreMessages.Content = "No More Messages  ";
                    }
                    else
                    {
                        LoadmoreMessages.Content = "Load More Messages";
                        ChatMessgaeslist.ItemsSource = MSGitems;
                        ChatMessgaeslist.Items.Refresh();
                    }

                }
                catch { }

            }));


        }
        #endregion

        #region Cache System Checker #BackroundWorker
        private static string CacheUserID = "";
        void CacheSystemWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
            if (CachSystemWorker.CancellationPending == true)
            {
                e.Cancel = true;
            }
            if (CheckForInternetConnection() == false)
            {
                return;
            }
            using (var client = new WebClient())
            {
                var path2 = Settings.FolderDestination + Settings.SiteName + @"\Data\" + CacheUserID + @"\" + CacheUserID + ".json";
                var values = new NameValueCollection();
                values["user_id"] = IDUser;
                values["usersIDs"] = CacheUserID;
                values["s"] = session_ID;
                var ProfileListPost = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_multi_users", values);
                var ProfileListPostresponseString = Encoding.Default.GetString(ProfileListPost);
                var dictProfileList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ProfileListPostresponseString);
                string ApiStatus = dictProfileList["api_status"].ToString();
                if (ApiStatus == "200")
                {
                    var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                    var gg = dictProfileList["users"];
                    var Profiles = JObject.Parse(ProfileListPostresponseString).SelectToken("users").ToString();
                    Object obj = s.DeserializeObject(Profiles);

                    var jss = new JavaScriptSerializer();
                    JArray Profileusers = JArray.Parse(Profiles);


                    foreach (var ProfileUser in Profileusers)
                    {
                        JObject ProfileUserdata = JObject.FromObject(ProfileUser);

                        var Profile_User_ID = ProfileUserdata["user_id"].ToString();
                        var path = Settings.FolderDestination + Settings.SiteName + @"\Data\" + Profile_User_ID + @"\";

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
            catch { }
        }
        void CacheSystemWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        void CacheSystemWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           try
            {
                if (CachSystemWorker.WorkerSupportsCancellation == true)
                {
                    CachSystemWorker.CancelAsync();
                }

                CheckCaching(CacheUserID);
            }
            catch { }

        }
        #endregion

        private void PostList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (PostList.SelectedItem != null)
                {
                    Process.Start((PostList.SelectedItem as PostItem).url);
                }
            }
            catch { }

        }

        private void FrendsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (FrendsList.SelectedItem != null)
                {
                    UserPostID = (FrendsList.SelectedItem as FriendsItem).user_id;

                    if (PostWorker.WorkerSupportsCancellation == true)
                    { PostWorker.CancelAsync(); }

                    loadingfrendsellipse.Visibility = System.Windows.Visibility.Visible;
                    loadingpostsellipse.Visibility = System.Windows.Visibility.Visible;
                    BlurimageLoadingpanel.Visibility = System.Windows.Visibility.Visible; 
                    FriendIcon.Visibility = System.Windows.Visibility.Hidden;
                    LastPostIcon.Visibility = System.Windows.Visibility.Hidden;

                    PostWorker = new BackgroundWorker();
                    PostWorker.WorkerReportsProgress = true;
                    PostWorker.WorkerSupportsCancellation = true;
                    PostWorker.DoWork += PostFriendWorker_DoWork;
                    PostWorker.ProgressChanged += PostFriendWorker_ProgressChanged;
                    PostWorker.RunWorkerCompleted += PostFriendWorker_RunWorkerCompleted;
                    PostWorker.RunWorkerAsync();

                    if (CachSystemWorker.WorkerSupportsCancellation == true)
                    { CachSystemWorker.CancelAsync(); }
                    CacheUserID = (FrendsList.SelectedItem as FriendsItem).user_id;
                    CheckCaching(CacheUserID);
                    CachSystemWorker = new BackgroundWorker();
                    CachSystemWorker.WorkerReportsProgress = true;
                    CachSystemWorker.WorkerSupportsCancellation = true;
                    CachSystemWorker.DoWork += CacheSystemWorker_DoWork;
                    CachSystemWorker.ProgressChanged += CacheSystemWorker_ProgressChanged;
                    CachSystemWorker.RunWorkerCompleted += CacheSystemWorker_RunWorkerCompleted;
                    CachSystemWorker.RunWorkerAsync();

                }
            }
            catch { }

        }


        #region ChatTextBox Control
        private void SmileButton_Click(object sender, RoutedEventArgs e)
        {
            imogiPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private static string FileNameAttachment = "";
        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension by default its select from 
            dlg.DefaultExt = "All files";

            try
            {
                string Extenstions = Settings.allowedExtenstion.Replace(",", ";*.");
                dlg.Filter = "All files (Documents,Images,Media,Archive)|" + Extenstions + "";
            }
            catch
            {
               dlg.Filter ="Documents (.txt;*.pdf)|*.txt;*.pdf|Image files (*.png;*.jpg;*.gif;*.ico;*.jpeg)|*.png;*.jpg;*.gif;*.ico;*.jpeg|Media files (*.mp4;*.mp3;*.avi;*.3gp;*.mp2;*.wmv;*.mkv;*.mpg;*.flv;*.wav)|*.mp4;*.mp3;*.avi;*.3gp;*.mp2;*.wmv;*.mkv;*.mpg;*.flv;*.wav|Archive files (.rar;*.zip;*.iso;*.tar;*.gz)|.rar;*.zip;*.iso;*.tar;*.gz|All files (*.*)|*.*";
            }

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;

                var f = filename.Split(new char[] { '.' });
                var allowedExtenstion = Settings.allowedExtenstion.Split(new char[] { ',' });
                if (allowedExtenstion.Contains(f.Last()))
                {
                    try
                    {
                        FileNameAttachment = filename;
                        TextRange textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd);
                        var TextMsg = textRange.Text;
                        StringWriter myWriter = new StringWriter();
                        HttpUtility.HtmlDecode(textRange.Text, myWriter);
                        TextMsg = myWriter.ToString();
                        if (TextMsg == "Write your Message\r\n")
                        { TextMsg = ""; }

                        TextMesage = TextMsg;

                        uplouding.Visibility = System.Windows.Visibility.Visible;
                        NoMessagePanel.Visibility = System.Windows.Visibility.Hidden;
                        var bw = new BackgroundWorker();
                        bw.DoWork += (o, args) => MethodTosendMesssage(TextMsg);
                        bw.RunWorkerCompleted += (o, args) => ScrollAndUpdateChatbox();
                        bw.RunWorkerAsync();
                    }
                    catch { }
                }
                else
                {
                    MessageBox.Show("The selected file extenstion is forbidden !", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }


            }

        }
        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            TextRange textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd);
            var TextMsg = textRange.Text;
            var NoMessageBlock = this.FindResource("NoMessageBlock") as TextBlock;
            if (TextMsg == "" || TextMsg == "Write your Message\r\n" || ChatTitleChange.Text == "" || NoMessageBlock.Text == "Please select a recipient to start the chat")
            {
                return;
            }
            else
            {
                StringWriter myWriter = new StringWriter();
                HttpUtility.HtmlDecode(textRange.Text, myWriter);
                TextMsg = myWriter.ToString();
                TextMesage = TextMsg;
                var Updater2 = items.Where(d => d.user_id == IDUser).FirstOrDefault();
                if (Updater2 != null)
                {
                    Updater2.text = TextMsg;
                    lbTodoList.Items.Refresh();
                }
                NoMessagePanel.Visibility = System.Windows.Visibility.Hidden;
                var bw = new BackgroundWorker();
                bw.DoWork += (o, args) => MethodTosendMesssage(TextMsg);
                bw.RunWorkerCompleted += (o, args) => ScrollAndUpdateChatbox();
                bw.RunWorkerAsync();
               

            }
        }
        private static string TextMesage = "";
        private void MessageBoxText_OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
               

            }


        }
        private void MessageBoxText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextRange textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd);
            if (textRange.Text.Length >= 1 && textRange.Text.Length <= 2)
            {
                var bw = new BackgroundWorker();
                bw.DoWork += (o, args) => TypingEvent("Typping");
                bw.RunWorkerAsync();
            }
            else if (textRange.Text == "")
            {
                var bw = new BackgroundWorker();
                bw.DoWork += (o, args) => TypingEvent("removing");
                bw.RunWorkerAsync();
            }
        }
        private void MessageBoxText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
                var TextMsg = textRange;
                var Spaces = Regex.Matches(textRange, "\r\n").Count;

                if (Spaces < 3)
                {
                    TextMsg = textRange.Replace("\r\n", "");
                }
                var NoMessageBlock = this.FindResource("NoMessageBlock") as TextBlock;
                if (TextMsg == "" || TextMsg == "Write your Message\r\n" || NoMessageBlock.Text == "Please select a recipient to start the chat" || ChatTitleChange.Text == "")
                {
                    return;
                }
                else
                {
                    StringWriter myWriter = new StringWriter();
                    HttpUtility.HtmlDecode(TextMsg, myWriter);
                    TextMsg = myWriter.ToString();
                    TextMesage = TextMsg;
                    var Updater2 = items.Where(d => d.user_id == IDUser).FirstOrDefault();
                    if (Updater2 != null)
                    {
                        Updater2.text = TextMsg;
                        lbTodoList.Items.Refresh();
                    }

                    NoMessagePanel.Visibility = System.Windows.Visibility.Hidden;
                    var bw = new BackgroundWorker();
                    bw.DoWork += (o, args) => MethodTosendMesssage(TextMsg);
                    bw.RunWorkerCompleted += (o, args) => ScrollAndUpdateChatbox();
                    bw.RunWorkerAsync();

                }
                e.Handled = true;
            }
        }

        #endregion

        #region Send Message Control
        private static Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        string time2 = unixTimestamp.ToString();
        private void MethodTosendMesssage(string TextMsg)
        {
            unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            time2 = unixTimestamp.ToString();

            if (FileNameAttachment == "")
            {
                var Widthresult = "";
                var time = DateTime.Now.ToString("hh:mm");
                var VisibiltyEvent = "";
                if (TextMsg != "")
                {
                    VisibiltyEvent = "Visible";
                    if (TextMsg.Length >= 0 && TextMsg.Length <= 3)
                    { Widthresult = "35"; }
                    if (TextMsg.Length > 3 && TextMsg.Length <= 6)
                    { Widthresult = "50"; }
                    if (TextMsg.Length > 6 && TextMsg.Length <= 9)
                    { Widthresult = "85"; }
                    if (TextMsg.Length > 9 && TextMsg.Length <= 13)
                    { Widthresult = "105"; }
                    if (TextMsg.Length > 10 && TextMsg.Length <= 13)
                    { Widthresult = "105"; }
                    if (TextMsg.Length >= 10 && TextMsg.Length <= 15)
                    { Widthresult = "140"; }
                    if (TextMsg.Length > 15 && TextMsg.Length <= 18)
                    { Widthresult = "155"; }
                    if (TextMsg.Length > 18 && TextMsg.Length <= 23)
                    { Widthresult = "155"; }
                    if (TextMsg.Length >= 20 && TextMsg.Length <= 26)
                    { Widthresult = "175"; }
                    if (TextMsg.Length >= 26 && TextMsg.Length <= 30)
                    { Widthresult = "200"; }
                    if (TextMsg.Length >= 31 && TextMsg.Length <= 35)
                    { Widthresult = "250"; }
                    if (TextMsg.Length >= 36 && TextMsg.Length <= 45)
                    { Widthresult = "300"; }
                }
                else
                {
                    VisibiltyEvent = "Collapsed";
                    Widthresult = "0";
                }

                MSGitems.Add(new MessageItem() { Content = TextMsg, Position = "right", Type = "right_text", CreatedAt = time, WidthWrap = Widthresult, messageID = time2 });

            }


        }
        private void MethodToUpdateLastMesssage(string TextMsg)
        {
            try
            {
                if (CheckForInternetConnection()== false)
                {
                    return;
                }
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();

                    values["user_id"] = IDUser;
                    values["recipient_id"] = chatuserid;
                    values["s"] = session_ID;
                    values["text"] = TextMsg;
                    values["send_time"] = time2;
                    var text = TextMsg;
                    var ChatusersListresponseString = "";
                    if (FileNameAttachment != "")
                    {
                        using (var stream = File.Open(FileNameAttachment, FileMode.Open))
                        {
                            var files = new[] { new UploadFile { Name = "message_file", Filename = Path.GetFileName(FileNameAttachment), ContentType = "text/plain", Stream = stream } };

                            byte[] result = UploadFiles(Settings.WebsiteUrl + "/app_api.php?type=insert_new_message", files, values);
                            ChatusersListresponseString = System.Text.Encoding.UTF8.GetString(result, 0, result.Length);

                        }
                    }
                    else
                    {
                        var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=insert_new_message", values);
                        ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                    }

                    var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                    string ApiStatus = dictChatusersList["api_status"].ToString();
                    if (ApiStatus == "200")
                    {
                        var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var gg = dictChatusersList["messages"];
                        var messages = JObject.Parse(ChatusersListresponseString).SelectToken("messages").ToString();
                        JArray ChatMessages = JArray.Parse(messages);

                        foreach (var MessageInfo in ChatMessages)
                        {
                            JObject ChatlistUserdata = JObject.FromObject(MessageInfo);
                            var Blal = ChatlistUserdata["messageUser"];
                            var Position = MessageInfo["position"].ToString();
                            var TextMsg2 = MessageInfo["text"].ToString();
                            var Type = MessageInfo["type"].ToString();
                            var TimeMsg = MessageInfo["time_text"].ToString();
                            var msgID = MessageInfo["id"].ToString();
                            var send_time = MessageInfo["send_time"].ToString();

                            updater(send_time, TimeMsg, msgID, Type);

                        }
                    }
                    else
                    {
                        if (CheckForInternetConnection() == false)
                        {
                            MessageBox.Show("Cannot send Message , check your Internet Connection !", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else
                        {
                            MessageBox.Show("Cannot send Message , No respone from the server ", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }


                    }
                }
            }
            catch { MessageBox.Show("Cannot send Message ,check your Internet Connection ! or please try again later ", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation); }
            FileNameAttachment = "";
        }
        private void updater(string send_time, string TimeMsg, string msgID, string Type)
        {
            if (FileNameAttachment == "")
            {
                var Updater = MSGitems.Where(d => d.messageID == send_time).FirstOrDefault();
                if (Updater != null)
                {
                    Updater.messageID = msgID;
                    Updater.CreatedAt = TimeMsg;
                    Updater.Type = Type;
                    Updater.Position = "right";
                }

            }
        }
        private void UpdateLastMessgaeControl()
        {
            ChatMessgaeslist.ItemsSource = MSGitems;
            ChatMessgaeslist.Items.Refresh();
            uplouding.Visibility = System.Windows.Visibility.Hidden;
            LoadmoreMessages.Visibility = System.Windows.Visibility.Hidden;

        }
        private void ScrollAndUpdateChatbox()
        {
          
            MessageBoxText.Document.Blocks.Clear();
            ChatMessgaeslist.ItemsSource = MSGitems;

            var bw2 = new BackgroundWorker();
            bw2.DoWork += (o, args) => MethodToUpdateLastMesssage(TextMesage);
            bw2.RunWorkerCompleted += (o, args) => UpdateLastMessgaeControl();
            bw2.RunWorkerAsync();

            LoadmoreMessages.Visibility = System.Windows.Visibility.Hidden;
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
             {
                 ListBoxAutomationPeer svAutomation = (ListBoxAutomationPeer)ScrollViewerAutomationPeer.CreatePeerForElement(ChatMessgaeslist);
                 IScrollProvider scrollInterface = (IScrollProvider)svAutomation.GetPattern(PatternInterface.Scroll);
                 System.Windows.Automation.ScrollAmount scrollVertical = System.Windows.Automation.ScrollAmount.LargeIncrement;
                 System.Windows.Automation.ScrollAmount scrollHorizontal = System.Windows.Automation.ScrollAmount.NoAmount;
                 //If the vertical scroller is not available, the operation cannot be performed, which will raise an exception. 
                 if (scrollInterface.VerticallyScrollable)
                     scrollInterface.Scroll(scrollHorizontal, scrollVertical);
             }));
        }
        public void AddNewMessagetoCash()
        {
            try
            {
                if (CheckForInternetConnection() == false)
                {
                    return;
                }
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["user_id"] = IDUser;
                    values["recipient_id"] = chatuserid;
                    values["s"] = session_ID;
                    values["before_message_id"] = "0";
                    values["after_message_id"] = "0";

                    var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_user_messages", values);
                    var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                    var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                    string ApiStatus = dictChatusersList["api_status"].ToString();

                    if (ApiStatus == "200")
                    {

                        var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var gg = dictChatusersList["messages"];
                        var messages = JObject.Parse(ChatusersListresponseString).SelectToken("messages").ToString();
                        JArray ChatMessages = JArray.Parse(messages);
                        if (ChatMessages.Count() == 0)
                        {
                            return;
                        }
                        var pathData = Settings.FolderDestination + Settings.SiteName + @"\\Data\\" + chatuserid + "\\";
                        var pathData2 = pathData + chatuserid + "_M.json";
                        DirectoryInfo di2 = Directory.CreateDirectory(pathData);
                        System.IO.File.WriteAllText(pathData2, messages);

                    }
                }
            }
            catch
            {

            }

        }
        public void TypingEvent(string Status)
        {
            try
            {
                if (CheckForInternetConnection() == false)
                {
                    return;
                }
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();
                    values["user_id"] = IDUser;
                    values["recipient_id"] = chatuserid;
                    values["s"] = session_ID;
                    if (Status == "Typping")
                    {
                        var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=register_typing", values);
                        var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                        var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                        string ApiStatus = dictChatusersList["api_status"].ToString();

                        if (ApiStatus == "200")
                        {
                            return;
                        }
                    }
                    else if (Status == "removing")
                    {
                        var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=remove_typing", values);
                        var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                        var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                        string ApiStatus = dictChatusersList["api_status"].ToString();

                        if (ApiStatus == "200")
                        {
                            return;
                        }

                    }

                }

            }
            catch
            {

            }

        }


        #endregion

        #region Classes 
        public class UploadFile
        {
            public UploadFile()
            {
                ContentType = "application/octet-stream";
            }
            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public Stream Stream { get; set; }
        }
        public class TodoItem
        {
            public string user_id { get; set; }
            public string username { get; set; }
            public string name { get; set; }
            public BitmapImage profile_picture { get; set; }
            public BitmapImage verified { get; set; }
            public BitmapImage lastseen { get; set; }
            public string SeenMessageOrNo { get; set; }
            public string url { get; set; }
            public string text { get; set; }
            public string lastseenunixtimetext { get; set; }
            public string LastMessage_date_time { get; set; }
        }
        public class PostItem
        {
            public string user_id { get; set; }
            public string post_id { get; set; }
            public string name { get; set; }
            public BitmapImage icon { get; set; }
            public string time_text { get; set; }
            public string url { get; set; }
            public string Orginaltext { get; set; }
            public string postYoutube { get; set; }
            public string postDailymotion { get; set; }
            public string postFacebook { get; set; }
            public string postSoundCloud { get; set; }
            public string postVimeo { get; set; }
            public string postVine { get; set; }


        }
        public class FriendsItem
        {
            public string user_id { get; set; }
            public string username { get; set; }
            public string name { get; set; }
            public BitmapImage avatar { get; set; }
            public string url { get; set; }
        }
        public class MessageItem
        {
            public string Content { get; set; }
            public string WidthWrap { get; set; }
            public string Visibilty { get; set; }
            public string Type { get; set; }
            public string Position { get; set; }
            public string CreatedAt { get; set; }
            public string messageID { get; set; }
            public string MediaName { get; set; }
            public string MediaSize { get; set; }
            public string DownloadFileUrl { get; set; }
            public BitmapImage UserImage { get; set; }
            public BitmapImage ImageMedia { get; set; }

        }
        public class PopUpModelBinder
        {
            public PopUpModelBinder()
            {
                this.Messagefrom = Profilename;
                this.Messagecontent = ContentMsg;
                this.UserImage = ProfilePicSourse.ToString();
            }
            public string UserImage { get; set; }
            public string Messagefrom { get; set; }
            public string Messagecontent { get; set; }
        }
        #endregion

        #region Online\Offline ChatUser ComboBox
        private static string ListType = "all";

        public void OnOffAllChat(string ChatListType)
        {
            try
            {
                if (CheckForInternetConnection() == false)
                {
                    return;
                }
                using (var client = new WebClient())
                {
                    var values = new NameValueCollection();

                    values["user_id"] = IDUser;
                    values["user_profile_id"] = IDUser;
                    values["s"] = session_ID;
                    values["list_type"] = ChatListType;

                    var ChatusersListresponse = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_users_list", values);
                    var ChatusersListresponseString = Encoding.Default.GetString(ChatusersListresponse);
                    var dictChatusersList = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(ChatusersListresponseString);
                    string ApiStatus = dictChatusersList["api_status"].ToString();

                    if (ApiStatus == "200")
                    {
                        string ThemeUrl = dictChatusersList["theme_url"].ToString();
                        var s = new System.Web.Script.Serialization.JavaScriptSerializer();
                        var gg = dictChatusersList["users"];
                        var users = JObject.Parse(ChatusersListresponseString).SelectToken("users").ToString();
                        Object obj = s.DeserializeObject(users);

                        var jss = new JavaScriptSerializer();
                        JArray Chatusers = JArray.Parse(users);

                        items.Clear();

                        foreach (var ChatUser in Chatusers)
                        {
                            JObject ChatlistUserdata = JObject.FromObject(ChatUser);
                            var ChatUser_User_ID = ChatlistUserdata["user_id"].ToString();
                            var ChatUser_avatar = ChatlistUserdata["profile_picture"].ToString();
                            var ChatUser_name = ChatlistUserdata["name"].ToString();
                            var ChatUser_lastseen = ChatlistUserdata["lastseen"].ToString();
                            var ChatUser_lastseen_Time_Text = ChatlistUserdata["lastseen_time_text"].ToString();
                            var ChatUser_verified = ChatlistUserdata["verified"].ToString();
                            string lastItemOfSplit = ChatUser_avatar.Split('/').Last();
                            var path = Settings.FolderDestination + Settings.SiteName + @"\ImageCash\" + ChatUser_User_ID + @"\";
                            var path2 = path + lastItemOfSplit;
                            if (CashSystem(path2) == "0")
                            {
                                DirectoryInfo di = Directory.CreateDirectory(path);
                                string[] files = System.IO.Directory.GetFiles(path);
                                client.DownloadFile(ChatUser_avatar, path2);
                            }

                            JObject ChatlistuserLastMessage = JObject.FromObject(ChatlistUserdata["last_message"]);
                            var listuserLastMessage_Text = ChatlistuserLastMessage["text"].ToString();
                            var listuserLastMessage_date_time = ChatlistuserLastMessage["date_time"].ToString();

                            StringWriter myWriter = new StringWriter();
                            HttpUtility.HtmlDecode(listuserLastMessage_Text, myWriter);

                            listuserLastMessage_Text = myWriter.ToString();

                            var bi = new BitmapImage();

                            using (var stream = new FileStream(path2, FileMode.Open, FileAccess.Read))
                            {
                                bi.BeginInit();
                                bi.DecodePixelWidth = 250; bi.CacheOption = BitmapCacheOption.OnLoad; bi.StreamSource = stream;
                                bi.EndInit();
                                stream.Close();
                            }

                            bi.Freeze();
                            var ChatUser_verified_bitmap = new BitmapImage();
                            var IconPathCheked = Settings.FolderDestination + Settings.SiteName + @"\Icons\verified.png";
                            using (var stream33 = new FileStream(IconPathCheked, FileMode.Open, FileAccess.Read))
                            {
                                if (ChatUser_verified == "1")
                                {
                                    ChatUser_verified_bitmap.BeginInit();
                                    ChatUser_verified_bitmap.DecodePixelWidth = 15;
                                    ChatUser_verified_bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                    ChatUser_verified_bitmap.StreamSource = stream33;
                                    ChatUser_verified_bitmap.EndInit();
                                    stream33.Close();
                                }
                            }
                            ChatUser_verified_bitmap.Freeze();

                            var ChatUser_bitmap = new BitmapImage();
                            var IconOffline = Settings.FolderDestination + Settings.SiteName + @"\Icons\offline.png";
                            var IconOnline = Settings.FolderDestination + Settings.SiteName + @"\Icons\online.png";
                            if (ChatUser_lastseen == "off")
                            {
                                using (var stream = new FileStream(IconOffline, FileMode.Open, FileAccess.Read))
                                {

                                    ChatUser_bitmap.BeginInit();
                                    ChatUser_bitmap.DecodePixelWidth = 25;
                                    ChatUser_bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                    ChatUser_bitmap.StreamSource = stream;
                                    ChatUser_bitmap.EndInit();
                                    stream.Close();
                                }
                            }
                            else
                            {
                                using (var stream = new FileStream(IconOnline, FileMode.Open, FileAccess.Read))
                                {
                                    ChatUser_bitmap.BeginInit();
                                    ChatUser_bitmap.DecodePixelWidth = 25;
                                    ChatUser_bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                    ChatUser_bitmap.StreamSource = stream;
                                    ChatUser_bitmap.EndInit();
                                    stream.Close();
                                }
                            }
                            ChatUser_bitmap.Freeze();

                            items.Add(new TodoItem()
                            {
                                username = ChatUser_name,
                                lastseen = ChatUser_bitmap,
                                profile_picture = bi,
                                text = listuserLastMessage_Text,
                                lastseenunixtimetext = ChatUser_lastseen_Time_Text,
                                LastMessage_date_time = listuserLastMessage_date_time,
                                verified = ChatUser_verified_bitmap,
                                user_id = ChatUser_User_ID,
                            });


                        }

                    }
                }
            }
            catch { }
        }
        public void OnOffAllChatCompleted()
        {
            lbTodoList.ItemsSource = items;
            lbTodoList.Items.Refresh();
        }
        private void OnOffAllChatStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedStatus = (OnOffAllChatStatus.SelectedIndex.ToString());
            //TimerINT = 1000;
            if (selectedStatus == "0")
            {
                ListType = "all";
            }
            else if (selectedStatus == "1")
            {
                ListType = "online";
            }
            else if (selectedStatus == "2")
            {
                ListType = "offline";
            }
            if (CheckForInternetConnection() == false)
            {
                return;
            }
            var bw = new BackgroundWorker();
            bw.DoWork += (o, args) => OnOffAllChat(ListType);
            bw.RunWorkerCompleted += (o, args) => OnOffAllChatCompleted();
            bw.RunWorkerAsync();

        }
        #endregion

        #region Close /Minimize / Maximize /Lougout
        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
            this.Close();
        }
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }
        private void Lougout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    using (var client = new WebClient())
                    {
                        var values = new NameValueCollection();
                        values["user_id"] = IDUser;
                        values["s"] = session_ID;
                        var response = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=logout", values);
                        var responseString = Encoding.Default.GetString(response);
                    }
                    string[] lines3 = { "0" }; System.IO.File.WriteAllLines(Settings.KeepMe, lines3);
                    string[] lines2 = { "" }; System.IO.File.WriteAllLines(Settings.SessionID, lines2);
                    string[] lines = { "" }; System.IO.File.WriteAllLines(Settings.UserID, lines);
                    Settings.Show_SplashScreen = false;
                    System.Windows.Forms.Application.Restart();
                    System.Windows.Application.Current.Shutdown();
                }));
            }
            catch { }
        }
        #endregion

        private void cbdg1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                       () =>
                       {
                           double d = e.VerticalOffset;
                           if (d <= 5)
                           {
                               if (ChatMessgaeslist.Items.Count == 0)
                               {
                                   LoadmoreMessages.Visibility = System.Windows.Visibility.Hidden;
                               }
                               else
                               {
                                   LoadmoreMessages.Visibility = System.Windows.Visibility.Visible;
                               }

                           }
                           else
                           {
                               LoadmoreMessages.Visibility = System.Windows.Visibility.Hidden;
                           }
                       }));
            }
            catch
            { }
        }

        private void LoadmoreMessages_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckForInternetConnection() == false)
                {
                    return;
                }
                if (LoadmoreMessages.Content.ToString() == "Load More Messages")
                {
                    ChatMessgaeslist.SelectedIndex = 0;
                    if (ChatMessgaeslist.SelectedItem != null)
                        FirstMessageid = (ChatMessgaeslist.SelectedItem as MessageItem).messageID;
                    if (LoadMoreMessageWorker.WorkerSupportsCancellation == true)
                    {
                        LoadMoreMessageWorker.CancelAsync();
                    }
                    LoadMoreMessageWorker = new BackgroundWorker();
                    LoadMoreMessageWorker.WorkerReportsProgress = true;
                    LoadMoreMessageWorker.WorkerSupportsCancellation = true;
                    LoadMoreMessageWorker.DoWork += LoadMoreMessageWorker_DoWork;
                    LoadMoreMessageWorker.ProgressChanged += LoadMoreMessageWorker_ProgressChanged;
                    LoadMoreMessageWorker.RunWorkerCompleted += LoadMoreMessageWorker_RunWorkerCompleted;
                    LoadMoreMessageWorker.RunWorkerAsync();
                    LoadmoreMessages.Content = "Loading...        ";
                }
            }
            catch { }

        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {


        }

        #region Emoji Icon Events Click/Mouse
        private void SmileButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            imogiPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void imogiPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            imogiPanel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void imogiPanel_MouseMove(object sender, MouseEventArgs e)
        {

            imogiPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void imogiPanel_MouseEnter(object sender, MouseEventArgs e)
        {

        }
        #endregion

        #region Emoji Icon Click

        private void NormalSmile_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" :)");
            MessageBoxText.Focus();

        }

        private void LaughingEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" (<");
            MessageBoxText.Focus();

        }

        private void HappyFaceEmoji_Click(object sender, RoutedEventArgs e)
        {
        
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" **)");
            MessageBoxText.Focus();
        }

        private void CrazyEmoji_Click(object sender, RoutedEventArgs e)
        {

            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" :p");
            MessageBoxText.Focus();
           

        }

        private void ChesekyEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" :_p");
            MessageBoxText.Focus();
           

        }

        private void CoolEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" B)");
            MessageBoxText.Focus();
           
        }

        private void CringeEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" :D");
            MessageBoxText.Focus();
          

        }

        private void CossolEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" /_)");
            MessageBoxText.Focus();
           

        }

        private void AngelEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" 0)");
            MessageBoxText.Focus();
          
        }

        private void CryingEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" :_(");
            MessageBoxText.Focus();

        }

        private void SadfaceEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun("  :(");
            MessageBoxText.Focus();
         

        }

        private void KissingEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun("  :*");
            MessageBoxText.Focus();
         

        }

        private void HeartEmoji_Click(object sender, RoutedEventArgs e)
        {

            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun("  <3");
            MessageBoxText.Focus();
          

        }

        private void BreakingHeartEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" </3");
            MessageBoxText.Focus();
           

        }

        private void HeartEyesEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" *_*");
            MessageBoxText.Focus();
            

        }

        private void StarEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" <5");
            MessageBoxText.Focus();
           

        }

        private void SurprisedEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" :o");
            MessageBoxText.Focus();
            

        }

        private void ScreamEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" :0");
            MessageBoxText.Focus();
         

        }

        private void PainedFaceEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" o(");
            MessageBoxText.Focus();
           

        }

        private void DissatisfiedEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" -_(");
            MessageBoxText.Focus();
        

        }

        private void AngryEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" x(");
            MessageBoxText.Focus();
           

        }

        private void AngryFace_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun("  X(");
            MessageBoxText.Focus();
         

        }

        private void FaceWithStraightMouthEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" -_-");
            MessageBoxText.Focus();
           

        }

        private void PuzzledEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun(" :-/");
            MessageBoxText.Focus();
           

        }

        private void StraightFacedEmoji_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun("  :|");
            MessageBoxText.Focus();
           
        }

        private void HeavyExclamation_Click(object sender, RoutedEventArgs e)
        {
            string textRange = new TextRange(MessageBoxText.Document.ContentStart, MessageBoxText.Document.ContentEnd).Text;
            if (textRange.Contains("Write your Message"))
            {
                MessageBoxText.Document.Blocks.Clear();
            }

            MessageBoxText.CaretPosition = MessageBoxText.CaretPosition.GetPositionAtOffset(0, LogicalDirection.Forward);
            MessageBoxText.CaretPosition.InsertTextInRun("  !_");
            MessageBoxText.Focus();
        

        }
        #endregion

        #region Settings Menu
        private void PrivacyMenu_Click(object sender, RoutedEventArgs e)
        {
            PrivacyWindow PrivacyWindow = new PrivacyWindow();
            PrivacyWindow.Show();
        }
        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            About About = new About();
            About.Show();
        }
        private void UpdateMenu_Click(object sender, RoutedEventArgs e)
        {
            // this code will be ready on Next Update
            UpdateWindow UpdateWindow = new UpdateWindow();
            UpdateWindow.Show();
        }
        #endregion

        private void ChatMessgaeslist_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(ChatMessgaeslist) > 0)
            {
                Border border = (Border)VisualTreeHelper.GetChild(ChatMessgaeslist, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
            }
        }

        private void PlaceholdersListBox_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = ItemsControl.ContainerFromElement(sender as ListBox, e.OriginalSource as DependencyObject) as ListBoxItem;
                if (item != null)
                {
                    try
                    {
                        var Type = (item.Content as MessageItem).Type.ToString();
                        if (Type == "right_image" || Type == "left_image")
                        {
                            var OpenPath = (item.Content as MessageItem).DownloadFileUrl.ToString();
                            Process.Start(OpenPath);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

     
    }

}
