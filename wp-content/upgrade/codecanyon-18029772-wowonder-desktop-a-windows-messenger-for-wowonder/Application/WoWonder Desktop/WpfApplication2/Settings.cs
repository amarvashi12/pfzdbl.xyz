using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication2
{
     public static class Settings
    {

        // Main Settings >>>>> 
       
           public static string WebsiteUrl = "https://demo.wowonder.com/";
           public static string Version = "1.1";
           public static string API_ID = "144235f5702cb70fa6c3f48842738e35";
           public static string API_KEY = "b37bb1cd53d0bc21c13c1644e98a58ca";

        // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


          //Settings for Perfomance and Server connection
           public static bool Show_SplashScreen = true;
           public static bool Show_SettingsMenue = false;
           public static bool Show_Style_Settings_From_Website = true; 
           public static int Update_ContactList_INT = 5000;   //>>>>  5 seconds
           public static int Update_Message_Receiver_INT = 3000; // >>>>  3 seconds
           public static string MessageSound = "New-message.mp3"; // (Message Sound) Add first to the WowonderLight Solution Explorer and then specify the name here 


           //Auto-Impl Properties from Website Settings
           //  >>>>>>> You can modify the code by removing the { get; set; } and writing Example: (= "#444") the color wich you want
           //>>>>>>>>> public static string Header_Button_Shadow = "#444";

           public static string AppName = SiteName;
           public static string SiteName { get; set; }
           public static string Themes { get; set; }
           public static string Header_Background { get; set; }
           public static string Header_Hover_Border { get; set; }
           public static string Header_Color { get; set; }
           public static string Body_Background { get; set; }
           public static string Btn_Color { get; set; }
           public static string Btn_Background_Color { get; set; }
           public static string Btn_Hover_Color { get; set; }
           public static string Btn_Hover_Background_Color { get; set; }
           public static string Btn_Disabled { get; set; }
           public static string setting_header_color { get; set; }
           public static string setting_header_background { get; set; }
           public static string setting_active_sidebar_color { get; set; }
           public static string setting_active_sidebar_background { get; set; }
           public static string setting_sidebar_background { get; set; }
           public static string setting_sidebar_color { get; set; }
           public static string Header_Search_Color { get; set; }
           public static string Header_Button_Shadow { get; set; }
           public static string Chat_Outgoing_Background { get; set; }
           public static string Logo_Url { get; set; }
           public static string allowedExtenstion { get; set; }
           public static string Updateavailble{ get; set; }


        //=============== Message Popup Window Style ==========================

        public static string PopUpBackroundColor = "White";
        public static string PopUpTextFromcolor = "#444";
        public static string PopUpMsgTextcolor = "#444";

        // Saved Information Path 
        public static string FolderDestination = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\";
        public static string SessionID = FolderDestination + "\\" + AppName + @"\Session.txt";
        public static string UserID = FolderDestination + "\\" + AppName + @"\UserId.txt";
        public static string KeepMe = FolderDestination + "\\" + AppName + @"\keepme.txt";

    }
}
