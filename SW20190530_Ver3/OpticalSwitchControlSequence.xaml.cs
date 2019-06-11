using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.IO;
namespace SW20190530_Ver3
{
    /// <summary>
    /// Interaction logic for OpticalSwitchControlSequence.xaml
    /// </summary>
    public partial class OpticalSwitchControlSequence : Window
    {
        ComboBox port, group, type;
        Boolean offline;

        public OpticalSwitchControlSequence(MainWindow input)
        {
            InitializeComponent();
            //Presets passed from MainWindow
            port = input.port;
            group = input.group;
            type = input.type;
            offline = input.offlineButton.IsChecked.Value;

            //TODO different load presets depending on offline
                        
        }
        
        //Exit Button
        private void Button_Click_Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown(99);
        //Min Button
        private void Button_Min_Exit(object sender, RoutedEventArgs e) => SystemCommands.MinimizeWindow(this);
        //Draggable
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
                this.DragMove();
        }
        
        //Get's table test structure from file
        private void File_Open_Click (object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            { 
                Read_Saved_Test(File.ReadAllText(openFileDialog.FileName));
            }
        }

        private void Read_Saved_Test (String config)
        {
            StringReader test = new StringReader(config);
            test.ReadLine();
        
            int totalSteps = System.Convert.ToInt32(RestLine(test, '='));
            String boardConfig = RestLine(test, '=');

            Console.WriteLine(totalSteps);
            Console.WriteLine(boardConfig);
        }
        
        //Returns the rest of the line skipping char before the stop
        private string RestLine(StringReader test, char stop)
        {
            while(test.Read() != stop && test.Read() != -1)
            {
            }

            return test.ReadLine();            
        }
        
    }
}
