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

namespace SW20190530_Ver3
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class OffStartUpWindow : Window
    {

        Program_Ini call;
        public OffStartUpWindow(Program_Ini c )
        {
           InitializeComponent();
            call = c;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Start(false);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Start(true);
        }

        private void Start(bool def)
        {
            if(def == true)
            {
                ///TODO
            }
            else
            {
                call.IsEnabled = true;
            }

            this.Close();
        }

        
    }
}
