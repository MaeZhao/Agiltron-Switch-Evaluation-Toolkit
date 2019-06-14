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
using System.Collections;

namespace SW20190530_Ver3
{
    /// <summary>
    /// Interaction logic for OpticalSwitchControlSequence.xaml
    /// </summary>
    public partial class OpticalSwitchControlSequence : Window
    {
        ComboBox port;
        String type;
        Boolean offline;
        public OpticalSwitchControlSequence(MainWindow input)
        {
            InitializeComponent();
            //Presets passed from MainWindow
            port = input.port;
            type = input.type.Text;
            type = type.Replace(" ", String.Empty);
            type = type.Substring(0,type.IndexOf('('));
            Console.WriteLine("type :" + type);
            offline = input.offlineButton.IsChecked.Value;
            int o = System.Convert.ToInt32(type.Substring(type.IndexOf("X")).Replace("X", String.Empty));
            Console.WriteLine(o);
            
            int i = System.Convert.ToInt32(type.Substring(2, type.IndexOf("X") - 1).Replace("X", String.Empty));
            Console.WriteLine(i);
            SwitchGrid s = new SwitchGrid();
            Grid grid = s.Load_Grid(4, 2, 3);
            Grid.SetRow(grid, 1);
            Main.Children.Add(grid);
            
            
            //TODO different load presets when not online
            //TODO do soemthing with the port
        }

        //Load path to switch logic
        //TODO: move the private fields to just the arguements, get rid of used fields
        private void Load_PTH()
        {
            String PTH = File.ReadAllText("Logic_PTH/" + type +".PTH");
            
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






        //Read test components from file
        private void File_Open_Click (object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            { 
                Load_Saved_Test(File.ReadAllText(openFileDialog.FileName));
            }
        }

        //Load saved test file NOT done VERY buggy
        private void Load_Saved_Test (String config)
        {
            StringReader test = new StringReader(config);
            //Loading Test Components
            //TODO: remove console writelines
            int totalSteps = System.Convert.ToInt32(Find_Value(test, '='));
            Console.WriteLine("totalSteps: " + totalSteps);

            String title = "";
            if (test.Peek() == 'T')
            {
                title = Find_Value(test, '=');
            }
            Console.WriteLine("title: " + title);

            String boardConfig = Find_Value(test, '=');
            Console.WriteLine("boardConfig: " + boardConfig);

            ArrayList stepAr = Read_Step_Array(test, totalSteps);

            //IO.DataContext = new SwitchGrid(totalSteps, type, title, stepAr);

        }
        
        //Only follows specific step test file format
        private ArrayList Read_Step_Array(StringReader test, int totalSteps)
        {
            ArrayList stepAr = new ArrayList();
            int i = 0;
            for (i = 0; test.Peek() != -1; i++)
            {
                SwitchStep step = new SwitchStep(System.Convert.ToDouble(Find_Value(test, '=')), System.Convert.ToInt32(Find_Value(test, '=')));
                Console.WriteLine(i + " time: " + step.Time1);
                Console.WriteLine(i + " index: " + step.Index1);
                stepAr.Add(step);
            }
            if (i != totalSteps)
            {
                MessageBox.Show("Please fix TotalSteps in Test file");
            }
            return stepAr;
        }

        //Returns the rest of the line skipping everything before (and including) the stop
        private string Find_Value(StringReader test, char stop)
        {

            while (test.Peek() != stop && test.Peek() != -1)
            {
                test.Read();
            }
            if (test.Peek() == stop)
            {
                test.Read();
            }

            return test.ReadLine();
        }
       
    }
}