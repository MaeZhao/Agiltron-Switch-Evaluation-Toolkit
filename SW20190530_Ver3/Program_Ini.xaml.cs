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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SW20190530_Ver3
{
    /// <summary>
    /// Interaction logic for Program_Ini.xaml
    /// </summary>
    public partial class Program_Ini : Page
    {

        public Program_Ini()
        {
            InitializeComponent();
        }

        //Exit Button
        private void Button_Click_Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown(99);

        //Continue Button
        private void Button_Click_Continue(object sender, RoutedEventArgs e)
        {
            
            if (offlineButton.IsChecked.Value)
            {
                /// TODO: find out how offline specifically changes any of the data
            }
            else
                Console.WriteLine(false);
        }

       
    }
}
