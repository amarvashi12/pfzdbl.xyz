using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication2
{
   public class MsgDataTamplate: DataTemplateSelector

    {
       public DataTemplate commingmsg { get; set; }
       public DataTemplate goingmsg { get; set; }
       public DataTemplate goingimage { get; set; }
       public DataTemplate commingimage { get; set; }
       public DataTemplate goingfile { get; set; }
       public DataTemplate commingfile { get; set; }
       public DataTemplate goingvideo { get; set; }
       public DataTemplate commingvideo { get; set; }
       public DataTemplate commingsound { get; set; }
       public DataTemplate goingsound { get; set; }
       public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
       {
           var msg = item as WpfApplication2.MainForm.MessageItem;
           if (msg.Type == "right_text")
           { 
               return goingmsg;
           }
           else if (msg.Type == "left_text")
           {
               return commingmsg; 
           }
           else if (msg.Type == "right_image")
           {
               return goingimage;
           }
           else if (msg.Type == "left_image")
           {
               return commingimage;
           }
           else if (msg.Type == "left_file")
           {
               return commingfile;
           }
           else if (msg.Type == "right_file")
           {
               return goingfile;
           }
           else if (msg.Type == "right_video")
           {
               return goingvideo; 
           }
           else if (msg.Type == "left_video")
           {
               return commingvideo;
           }
           else if (msg.Type == "right_audio") 
           {
               return goingsound; 
           } 
           else 
           {
               return commingsound;
           }
       }

    }
}
