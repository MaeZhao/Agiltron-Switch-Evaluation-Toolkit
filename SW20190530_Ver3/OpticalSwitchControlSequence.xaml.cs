using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Messages.Core;
using ToastNotifications.Position;

namespace SW20190530_Ver3
{
    /// <summary>
    /// Interaction logic for OpticalSwitchControlSequence.xaml
    /// </summary>
    public partial class OpticalSwitchControlSequence : Window
    {
        private ComboBox port;
        private String type;
        private Boolean offline;
        private Notifier notifier; //From ToastNotifications v2 nuget pkg:
        private bool running;
        private bool pause;

        public OpticalSwitchControlSequence(MainWin input)
        {
            InitializeComponent();
            running = false;
            pause = false;
            Application.Current.MainWindow = this;
            //Initializes general notifier settings
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 10,
                    offsetY: 10);
                cfg.DisplayOptions.TopMost = true;
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));
                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            type = input.type.Text;
            type = type.Replace(" ", String.Empty);
            type = type.Substring(0, type.IndexOf('('));

            int numOut = System.Convert.ToInt32(type.Substring(type.IndexOf("X")).Replace("X", String.Empty));
            int numChannel = System.Convert.ToInt32(type.Substring(2, type.IndexOf("X") - 1).Replace("X", String.Empty));

            TextBlock TableTitle = new TextBlock
            {
                Text = "" + numChannel + " - " + numOut + " Switch Control Table",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontSize = 20,
                Foreground = Brushes.White,
                //FontWeight = FontWeights.Medium
            };
            Grid.SetRow(TableTitle, 1);
            Main.Children.Add(TableTitle);

            Grid grid = Load_Grid(numOut, numChannel, 6, new int[] { });
            Grid.SetRow(grid, 2);
            Grid.SetColumnSpan(grid, 2);
            grid.Margin = new Thickness(10, 10, 10, 10);
            Main.Children.Add(grid);

            #region improperly initialized fields (TODO)
            port = input.port;
            offline = input.offlineButton.IsChecked.Value;
            input.Close();
            this.Show();
            // Display offline mode notification
            if (offline)
            {
                var messageOptions = new ToastNotifications.Core.MessageOptions
                {
                    ShowCloseButton = true, // set the option to show or hide notification close button
                    FreezeOnMouseEnter = true, // set the option to prevent notification dissapear automatically if user move cursor on it
                    NotificationClickAction = n => // set the callback for notification click event
                    {
                        n.Close(); // call Close method to remove notification
                    },
                };
                notifier.ShowInformation("Running in Offline Mode", messageOptions);


            }
            #endregion
            //TODO load presets
            //TODO do soemthing with the port
            ProgressBar_ValueChanged(new object(), new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
        }

        // Loads ON OFF Table Grid
        public Grid Load_Grid(int outP, int channelNum, int steps, int[] runtime)
        {

            Grid switchGrid = new Grid();
            switchGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            switchGrid.VerticalAlignment = VerticalAlignment.Top;
            switchGrid.Background = Brushes.White;

            GridLength WxH = new GridLength(2, GridUnitType.Star);

            ColumnDefinition stepNum = new ColumnDefinition();
            stepNum.Width = WxH;
            ColumnDefinition runTime = new ColumnDefinition();
            runTime.Width = WxH;
            switchGrid.ColumnDefinitions.Add(stepNum);
            switchGrid.ColumnDefinitions.Add(runTime);

            RowDefinition chanNum = new RowDefinition();
            chanNum.Height = WxH;
            RowDefinition outNum = new RowDefinition();
            outNum.Height = WxH;
            switchGrid.RowDefinitions.Add(chanNum);
            switchGrid.RowDefinitions.Add(outNum);


            //Run title
            TextBox runT = new TextBox();
            runT.BorderBrush = System.Windows.Media.Brushes.Black;
            //runT.BorderThickness = new Thickness(2);
            runT.IsReadOnly = true;
            runT.FontSize = 14;
            runT.Text = "Run Times (s)";
            Grid.SetColumn(runT, 1);
            Grid.SetRow(runT, 0);
            Grid.SetRowSpan(runT, 2);
            switchGrid.Children.Add(runT);

            //Step Title
            TextBox stepT = new TextBox();
            stepT.BorderBrush = System.Windows.Media.Brushes.Black;
            //stepT.BorderThickness = new Thickness(2);
            stepT.IsReadOnly = true;
            stepT.FontSize = 14;
            stepT.Text = "Step(s)";
            Grid.SetColumn(stepT, 0);
            Grid.SetRow(stepT, 0);
            Grid.SetRowSpan(stepT, 2);
            switchGrid.Children.Add(stepT);

            //Generating Output Lables
            int currOutSt = 1;
            int currChanSt = 2;
            for (int c = 0; c < channelNum; c++)
            {
                for (int o = 0; o < outP; o++)
                {
                    currOutSt++;
                    ColumnDefinition cOut = new ColumnDefinition();
                    cOut.Width = WxH;
                    switchGrid.ColumnDefinitions.Add(cOut);

                    TextBox outLable = new TextBox();
                    outLable.BorderBrush = System.Windows.Media.Brushes.Black;
                    //outLable.BorderThickness = new Thickness(2);
                    outLable.IsReadOnly = true;
                    outLable.FontSize = 14;
                    outLable.Text = (c + 1) + "-" + (o + 1);
                    Grid.SetColumn(outLable, currOutSt);
                    Grid.SetRow(outLable, 1);
                    switchGrid.Children.Add(outLable);
                }
                TextBox chanLable = new TextBox();
                chanLable.BorderBrush = System.Windows.Media.Brushes.Black;
                // chanLable.BorderThickness = new Thickness(2);
                chanLable.IsReadOnly = true;
                chanLable.FontSize = 14;
                chanLable.Text = "Channel " + (c + 1);
                Grid.SetColumn(chanLable, currChanSt);
                Grid.SetRow(chanLable, 0);
                Grid.SetColumnSpan(chanLable, outP);
                switchGrid.Children.Add(chanLable);
                currChanSt += outP;
            }

            AddStepsButtonRT_UI(switchGrid, steps, outP * channelNum, runtime);

            return switchGrid;
        }

        //Adds Steps, or Rows of buttons to the ON OFF table
        //initializes RunTime cell and ON OFF buttons user interface
        public void AddStepsButtonRT_UI(Grid gSteps, int rows, int col, int[] runTime)
        {
            GridLength WxH = new GridLength(2, GridUnitType.Auto);


            //Default time if runTime is not specified:
            if (runTime.Length != rows)
            {
                if (runTime.Length == 0)
                {
                    Console.WriteLine("RunTime array is empty");
                }
                else
                    Console.WriteLine("RunTime array is incorrect");

                runTime = Enumerable.Repeat<int>(100, rows).ToArray<int>();
            }

            int currRows = gSteps.RowDefinitions.Count;
            for (int i = 1; i <= rows; i++)
            {
                RowDefinition nRow = new RowDefinition();
                nRow.Height = WxH;
                gSteps.RowDefinitions.Add(nRow);

                TextBox stepLable = new TextBox();
                stepLable.BorderBrush = System.Windows.Media.Brushes.Black;
                //stepLable.BorderThickness = new Thickness(2);
                stepLable.IsReadOnly = true;
                stepLable.FontSize = 14;
                stepLable.Text = "" + (currRows - 1);
                Grid.SetColumn(stepLable, 0);
                Grid.SetRow(stepLable, currRows);
                gSteps.Children.Add(stepLable);

                TextBox times = new TextBox();
                times.FontSize = 14;
                times.BorderBrush = System.Windows.Media.Brushes.Black;
                //times.BorderThickness = new Thickness(1);
                times.Text = "" + runTime.GetValue(i - 1);
                Grid.SetColumn(times, 1);
                Grid.SetRow(times, currRows);
                gSteps.Children.Add(times);
                times.LostFocus += RunTimeCell_UI; //initializes runtime UI
                                                   //Adds the ON OFF buttons
                for (int b = 0; b < col; b++)
                {
                    //ON OFF button generation
                    Button onOff = new Button
                    {
                        FontSize = 17,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Content = "OFF",
                        BorderBrush = new SolidColorBrush(Colors.Black),
                        BorderThickness = new Thickness(1)
                    };
                    //ON OFF button Click UI + Logic
                    //TODO button logic
                    onOff.Click += (sender, e) =>
                    {
                        if ((string)onOff.Content == "OFF")
                        {
                            onOff.Content = "ON";
                            onOff.Background = Brushes.Green;
                        }
                        else
                        {
                            onOff.Content = "OFF";
                            onOff.Background = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#223C69"));
                        }

                    };
                    Grid.SetRow(onOff, currRows);
                    Grid.SetColumn(onOff, b + 2);
                    gSteps.Children.Add(onOff);
                }
                currRows++;
            }
        }





        //UI interaction when Run Time value is not an integer:
        private void RunTimeCell_UI(object sender, RoutedEventArgs e)
        {
            var times = sender as TextBox;

            times.Text = times.Text.Replace(" ", "");

            if (!Regex.IsMatch(times.Text, @"^\d+$"))
            {
                times.IsInactiveSelectionHighlightEnabled = true;
                times.SelectionBrush = Brushes.Red;
                times.SelectAll();
                notifier.ShowError("Run Time can only be an integer");
            }
            else
            {
                times.IsInactiveSelectionHighlightEnabled = false;
                times.SelectionBrush = SystemColors.HighlightBrush;
            }
        }

        //Exit Button
        private void Button_Click_Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown(99);
        //Min Button
        private void Button_Min_Exit(object sender, RoutedEventArgs e) => SystemCommands.MinimizeWindow(this);
        //Draggable
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) => this.DragMove();



        #region FOLLOWING IS INCOMPLETE: Contains code related to loading and reading existing test cases

#line hidden
        //Read test components from file
        private void File_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Load_Saved_Test(File.ReadAllText(openFileDialog.FileName));
            }
        }
        //Load path to switch logic
        //TODO: move the private fields to just the arguements, get rid of used fields
        private void Load_PTH()
        {
            String PTH = File.ReadAllText("Logic_PTH/" + type + ".PTH");

        }
        //Load saved test file NOT done VERY buggy
        private void Load_Saved_Test(String config)
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
#line default
        #endregion

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //TODO Have the progress update live
            if (running == false)
            {
                progressBar.Value = 100;
                progressBar.Foreground = Brushes.Red;
            }
            else if (running == true && pause == false)
            {
                progressBar.Value = 10;
                progressBar.Foreground = Brushes.Green;
            }
            else
            {
                progressBar.Foreground = Brushes.Red;
            }
        }

        private void Button_Click_Run(object sender, RoutedEventArgs e)
        {
            //TODO
            running = true;
            ProgressBar_ValueChanged(sender, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
        }
        private void Button_Click_Pause(object sender, RoutedEventArgs e)
        {
            //TODO
            pause = !pause;
            ProgressBar_ValueChanged(sender, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
        }
        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            //TODO
            running = false;
            ProgressBar_ValueChanged(sender, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
        }
    }


}
