using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                SplashScreen Splash = new SplashScreen();
                if (Settings.Show_SplashScreen)
                {Splash.Show();}
                StyleReader();
              
                    if (File.Exists(Settings.KeepMe) && File.Exists(Settings.SessionID) && File.Exists(Settings.UserID))
                    {

                        string aa = File.ReadAllText(Settings.UserID);
                        string bb = File.ReadAllText(Settings.SessionID);
                        string dd = File.ReadAllText(Settings.KeepMe);
                        string TextID = aa.Replace("\r\n", "");
                        string sessionIDText = bb.Replace("\r\n", "");
                        string keepmeText = dd.Replace("\r\n", "");

                        if (keepmeText == "1" && sessionIDText != "" && TextID != "")
                        {
                            MainForm Mainform = new MainForm();
                            Thread.Sleep(1000);
                            Splash.Close();
                            Mainform.Show();
                        }
                        else
                        {
                            MainWindow Loginform = new MainWindow();
                            Thread.Sleep(1000);
                            Splash.Close();
                            Loginform.ShowDialog();
                        }

                    }
                    else
                    {
                        DirectoryInfo di = Directory.CreateDirectory(Settings.FolderDestination + "\\" + Settings.AppName);
                        string[] lines3 = { "0" }; System.IO.File.WriteAllLines(Settings.KeepMe, lines3);
                        string[] lines2 = { "" }; System.IO.File.WriteAllLines(Settings.SessionID, lines2);
                        string[] lines = { "" }; System.IO.File.WriteAllLines(Settings.UserID, lines);
                        MainWindow Loginform = new MainWindow();
                        Loginform.ShowDialog();
                    }


            }
            catch { }

        }

        private string StyleReader()
        {
            try
            {
                if (Settings.Show_Style_Settings_From_Website)
                {
                    using (var client = new WebClient())
                    {
                        var values = new NameValueCollection();
                        values["windows_app_version"] = Settings.Version;

                        ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                        var response = client.UploadValues(Settings.WebsiteUrl + "/app_api.php?type=get_settings", values);
                        var responseString = Encoding.Default.GetString(response);
                        var dict = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(responseString);
                        string ApiStatus = dict["api_status"].ToString();

                        if (ApiStatus == "200")
                        {
                            JObject settings = JObject.FromObject(dict["config"]);

                            Settings.SiteName = settings["siteName"].ToString();
                            Settings.Themes = settings["theme"].ToString();
                            Settings.Header_Background = settings["header_background"].ToString();
                            Settings.Header_Hover_Border = settings["header_hover_border"].ToString();
                            Settings.Header_Color = settings["header_color"].ToString();
                            Settings.Body_Background = settings["body_background"].ToString();
                            Settings.Btn_Color = settings["btn_color"].ToString();
                            Settings.Btn_Background_Color = settings["btn_background_color"].ToString();
                            Settings.Btn_Hover_Color = settings["btn_hover_color"].ToString();
                            Settings.Btn_Hover_Background_Color = settings["btn_hover_background_color"].ToString();
                            Settings.Btn_Disabled = settings["setting_sidebar_color"].ToString();
                            Settings.setting_header_color = settings["setting_header_color"].ToString();
                            Settings.setting_header_background = settings["setting_header_background"].ToString();
                            Settings.setting_active_sidebar_color = settings["setting_active_sidebar_color"].ToString();
                            Settings.setting_active_sidebar_background = settings["setting_active_sidebar_background"].ToString();
                            Settings.setting_sidebar_background = settings["setting_sidebar_background"].ToString();
                            Settings.setting_sidebar_color = settings["setting_sidebar_color"].ToString();
                            Settings.Header_Search_Color = settings["header_search_color"].ToString();
                            Settings.Header_Button_Shadow = settings["header_button_shadow"].ToString();
                            Settings.Chat_Outgoing_Background = settings["chat_outgoing_background"].ToString();
                            Settings.Logo_Url = settings["logo_url"].ToString();
                            Settings.allowedExtenstion = settings["allowedExtenstion"].ToString();
                            Settings.Updateavailble = settings["update_available"].ToString();

                            if (Settings.API_ID == settings["widnows_app_api_id"].ToString() && Settings.API_KEY == settings["widnows_app_api_key"].ToString())
                            { return "Correct"; }
                        }
                        else
                        { return "Error"; }

                    }
                }
                else
                {
                    return "Correct";
                }

                   
                
                return "Nothing";
            }
            catch(Exception e)
            {
                MessageBox.Show("No Respone from server ,the application cannot connect at this mean Time,please try again later" + e);
                return "Error";
            }
        }
    }
}
