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
    /// Interaction logic for NoSwitchPopUp.xaml
    /// </summary>
    public partial class NoSwitchPopUp : WindowUIComponents
    {
        public NoSwitchPopUp()
        {
            InitializeComponent();
        }

        public void Button_Click_OK(object sender, RoutedEventArgs e)=>SystemCommands.CloseWindow(this);
    }
}
