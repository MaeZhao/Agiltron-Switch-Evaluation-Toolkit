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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Exit Button
        private void Button_Click_Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown(99);
        //Min Button
        private void Button_Min_Exit(object sender, RoutedEventArgs e) => SystemCommands.MinimizeWindow(this);
        //Draggable
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        //Continue Button
        private void Button_Click_Continue(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;

            if (offlineButton.IsChecked.Value)
            {
                OffStartUpWindow offStart = new OffStartUpWindow(this);
                offStart.Show();
            }
            else
            {
                ///TODO
                OpticalSwitchControlSequence op = new OpticalSwitchControlSequence(this);
                op.Show();
                this.Close();
            }
        }
        //Change in group selection
        private void Group_Changed(object sender, RoutedEventArgs e)
        {
            if(group.SelectedIndex == 0)
            {
                return;
            }
            else if (group.SelectedIndex == 1)
            {
                type.Items.Clear();
                Add_Items(type, new[]{"CL 2X1 (Y)", "CL 8X1 (Y)", " CL 8X1MN (Y)",
                    "LB 2X1 (Y)", "LB 4X1 (Y)", "LB 8X1 (Y)" });
                
                type.SelectedIndex= 0;
            }
            else if(group.SelectedIndex == 2)
            {
                type.Items.Clear();
                Add_Items(type, new[]{"CL 4X4 (Y)", "LB 4X4 (Y)", "DD 4BIT (Y)",
                    "DD 5BIT (Y)", "DD 6BIT (Y)", "CL 2X2 (Y)","LB 2X2 (Y)"});
                
                type.SelectedIndex = 0;
            }
        }

        // probably should make a form
        private void Add_Items(ComboBox c, string[] l)
        {
            for (int i = 0; i < l.GetLength(0); i++)
            {
                c.Items.Add(l[i]);
            }
        }
    }

}
    
