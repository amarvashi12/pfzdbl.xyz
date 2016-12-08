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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
           

            
        }


        //Remove the comment from the code if you want to add fade on your splashscreen
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            //Closing -= Window_Closing;
            //e.Cancel = true;
            //var anim = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
            //anim.Completed += (s, _) => this.Close();
            //this.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
