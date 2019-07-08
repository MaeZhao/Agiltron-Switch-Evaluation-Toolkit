using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
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
        //UI fields
        private Boolean offline;
        private Notifier notifier; //From ToastNotifications v2 nuget pkg
        private ToastNotifications.Core.MessageOptions messageOptions;  //From ToastNotifications v2 nuget pkg
        //switchGrid specs
        private int numOut, numChannel;
        //running/pausing button controls
        private bool running;
        private bool pause;
        private bool flashing;
        private int runningRow;


        /// <summary>
        /// Initializes a new instance of the <see cref="OpticalSwitchControlSequence"/> class.
        /// </summary>
        /// <param name="input">The input.</param>
        public OpticalSwitchControlSequence(MainWin input)
        {
            InitializeComponent();
            //Initializes general notifier settings
            Application.Current.MainWindow = this;
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
            messageOptions = new ToastNotifications.Core.MessageOptions
            {
                ShowCloseButton = true, // set the option to show or hide notification close button
                FreezeOnMouseEnter = true, // set the option to prevent notification dissapear automatically if user move cursor on it
                NotificationClickAction = n => // set the callback for notification click event
                {
                    n.Close(); // call Close method to remove notification
                },
            };

            // offline display
            offline = input.offlineButton.IsChecked.Value;
            input.Close();
            this.Show();
            if (offline)
            {
                notifier.ShowInformation("Running in Offline Mode", messageOptions);
            }

            //Initializes/finds switchGrid specs 
            SwitchBoardFieldIni(input);
            //Initializes switch test run specs
            SwitchRunPauseStopFieldIni();

            //TODO load presets
            //TODO do soemthing with the port
            this.MaxWidth = SystemParameters.WorkArea.Width;
            this.MaxHeight = SystemParameters.WorkArea.Height;
            DiagramINI();
        }

        #region REGION: Methods used for every Window (only slightly variated) TODO: Turn these methods into an abstract class
        #region REGION: Adjusts default pixel offset in window maximization/minimization
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        // Make sure RECT is actually OUR defined struct, not the windows rect.
        public static RECT GetWindowRectangle(Window window)
        {
            RECT rect;
            GetWindowRect((new WindowInteropHelper(window)).Handle, out rect);
            return rect;
        }

        private void Win_StateChanged(object sender, EventArgs e)
        {
            RECT rect = GetWindowRectangle(win);
            if (win.WindowState == WindowState.Maximized)
            {
                Window_Loaded(sender, new RoutedEventArgs()); // Inefficient: resets the WindowsState.Normal settings

                rect.Left = (int)SystemParameters.WorkArea.Left;
                rect.Top = (int)SystemParameters.WorkArea.Top;
                rect.Right = (int)(rect.Left - this.MaxWidth);
                rect.Bottom = (int)(rect.Top - this.MaxHeight);
                WindowState = WindowState.Normal;
            }

        }

        /// <summary>
        /// Handles the Loaded event of the Initial Window State control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = SystemParameters.WorkArea.Left;
            this.Top = SystemParameters.WorkArea.Top;
            this.Height = SystemParameters.WorkArea.Height;
            this.Width = SystemParameters.WorkArea.Width;

        }
        #endregion

        /// <summary>
        /// Handles the Exit event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown(99);
        /// <summary>
        /// Handles the Exit event of the Button_Min control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Min_Exit(object sender, RoutedEventArgs e) => SystemCommands.MinimizeWindow(this);
        /// <summary>
        /// Handles the MouseDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RECT rect = GetWindowRectangle(win);
            if (this.Height == SystemParameters.WorkArea.Height && this.Width == SystemParameters.WorkArea.Width && WindowState != WindowState.Maximized)
            {
                //WindowState = WindowState.Normal;
                this.Height = 600;
                this.Width = 1000;

                this.Top = GetMousePositionY();
                this.Left = (int)(GetMousePositionX() - 400);
            }

            this.DragMove();
        }
        #region REGION: Finds Mouse cursor positions
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        /// <summary>
        /// Gets the mouse position x.
        /// </summary>
        /// <returns></returns>
        public static int GetMousePositionX()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return w32Mouse.X;
        }
        /// <summary>
        /// Gets the mouse position y.
        /// </summary>
        /// <returns></returns>
        public static int GetMousePositionY()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return w32Mouse.Y;
        }
        #endregion
        #endregion
    }


    partial class OpticalSwitchControlSequence
    {
        #region REGION: Switch Grid Components (before running)
        /// <summary>
        /// Initializes Switch board fields.
        /// </summary>
        /// <param name="input">The input.</param>
        private void SwitchBoardFieldIni(MainWin input)
        {
            String type = input.type.Text;
            type = type.Replace(" ", String.Empty);
            type = type.Substring(0, type.IndexOf('('));

            numOut = System.Convert.ToInt32(type.Substring(type.IndexOf("X")).Replace("X", String.Empty));
            numChannel = System.Convert.ToInt32(type.Substring(2, type.IndexOf("X") - 1).Replace("X", String.Empty));

            //Title Components:
            Grid Title = new Grid();
            GridLength Stiff = new GridLength(2, GridUnitType.Auto);
            RowDefinition TitleHeight = new RowDefinition
            {
                Height = new GridLength(2, GridUnitType.Star)
            };
            Title.RowDefinitions.Add(TitleHeight);
            ColumnDefinition TitleWidth = new ColumnDefinition
            {
                Width = new GridLength(2, GridUnitType.Star)
            };
            Title.ColumnDefinitions.Add(TitleWidth);
            //Title
            TextBlock TestRunTitle = new TextBlock
            {
                Text = "" + numChannel + " - " + numOut + " Switch Control",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                FontWeight = FontWeights.Light,
                FontSize = 25,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(TestRunTitle, 1);
            Grid.SetColumnSpan(TestRunTitle, 2);
            Title.Children.Add(TestRunTitle);
            Grid.SetRow(Title, 1);

            Grid.SetColumnSpan(Title, 3);
            Main.Children.Add(Title);

            //initializes switchGrid:
            Load_Grid(100, new int[] { });

            runningRow = 2;
        }

        /// <summary>Loads ON OFF button grid.</summary>
        /// <param name="steps">The number of steps.</param>
        /// <param name="runtime">An array of runtime times (in seconds).</param>
        public void Load_Grid(int steps, int[] runtime)
        {
            GridLength Adjustable = new GridLength(2, GridUnitType.Star);
            GridLength Stiff = new GridLength(2, GridUnitType.Auto);

            ColumnDefinition stepNum = new ColumnDefinition
            {
                Width = Stiff
            };
            ColumnDefinition runTime = new ColumnDefinition
            {
                Width = Stiff
            };
            switchGrid.ColumnDefinitions.Add(stepNum);
            switchGrid.ColumnDefinitions.Add(runTime);

            RowDefinition chanNum = new RowDefinition
            {
                Height = Adjustable
            };
            RowDefinition outNum = new RowDefinition
            {
                Height = Adjustable
            };
            switchGrid.RowDefinitions.Add(chanNum);
            switchGrid.RowDefinitions.Add(outNum);

            //Run title
            TextBox runT = new TextBox
            {
                BorderBrush = System.Windows.Media.Brushes.Black,
                IsReadOnly = true,
                FontSize = 14,
                Text = "Run Times (s)"
            };
            Grid.SetColumn(runT, 1);
            Grid.SetRow(runT, 0);
            Grid.SetRowSpan(runT, 2);
            switchGrid.Children.Add(runT);

            //Step Title
            TextBox stepT = new TextBox
            {
                BorderBrush = System.Windows.Media.Brushes.Black,
                IsReadOnly = true,
                FontSize = 14,
                Text = "Step(s)"
            };
            Grid.SetColumn(stepT, 0);
            Grid.SetRow(stepT, 0);
            Grid.SetRowSpan(stepT, 2);
            switchGrid.Children.Add(stepT);

            //Generating Output Lables
            int currOutSt = 1;
            int currChanSt = 2;
            for (int c = 0; c < numChannel; c++)
            {
                for (int o = 0; o < numOut; o++)
                {
                    currOutSt++;
                    ColumnDefinition cOut = new ColumnDefinition
                    {
                        Width = Adjustable
                    };
                    switchGrid.ColumnDefinitions.Add(cOut);

                    TextBox outLable = new TextBox
                    {
                        BorderBrush = System.Windows.Media.Brushes.Black,
                        IsReadOnly = true,
                        FontSize = 14,
                        Text = (c + 1) + "-" + (o + 1)
                    };
                    Grid.SetColumn(outLable, currOutSt);
                    Grid.SetRow(outLable, 1);
                    switchGrid.Children.Add(outLable);
                }
                TextBox chanLable = new TextBox
                {
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    IsReadOnly = true,
                    FontSize = 14,
                    Text = "Channel " + (c + 1)
                };
                Grid.SetColumn(chanLable, currChanSt);
                Grid.SetRow(chanLable, 0);
                Grid.SetColumnSpan(chanLable, numOut);
                switchGrid.Children.Add(chanLable);
                currChanSt += numOut;
            }
            AddStepsButtonRT_UI(steps, numOut * numChannel, runtime);
        }


        /// <summary>
        /// Adds Steps, or Rows of buttons to the ON OFF table :
        /// initializes RunTime cell and ON OFF buttons user interface     
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="col">The col.</param>
        /// <param name="runTime">The run times.</param>
        public void AddStepsButtonRT_UI(int rows, int col, int[] runTime)
        {
            GridLength WxH = new GridLength(2, GridUnitType.Auto);

            //Default time if runTime array is not initialized properly:
            if (runTime.Length != rows)
            {
                if (runTime.Length == 0)
                {
                    notifier.ShowWarning("Run Times not initialized", messageOptions);
                }
                else
                {
                    notifier.ShowError("Unable to read Run Times", messageOptions);
                }

                runTime = Enumerable.Repeat<int>(100, rows).ToArray<int>();
            }

            int currRows = switchGrid.RowDefinitions.Count;
            for (int i = 1; i <= rows; i++)
            {
                RowDefinition nRow = new RowDefinition
                {
                    Height = WxH
                };
                switchGrid.RowDefinitions.Add(nRow);

                InlineUIContainer hcontrol = new InlineUIContainer();
                Rectangle highlight = new Rectangle
                {
                    Fill = System.Windows.Media.Brushes.Transparent,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    IsEnabled = false
                };
                Grid.SetRow(highlight, currRows);
                Grid.SetColumnSpan(highlight, 2); // Might change to 2
                switchGrid.Children.Add(highlight);

                //Highlights/unhighlights row
                highlight.IsEnabledChanged += (sender, e) =>
                {
                    if (runningRow == Grid.GetRow(highlight))
                    {
                        highlight.Fill = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3BA00"));
                    }
                    else
                    {
                        highlight.Fill = System.Windows.Media.Brushes.Transparent;
                    }
                };

                TextBox stepLable = new TextBox
                {
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    IsReadOnly = true,
                    FontSize = 14,
                    Text = "" + (currRows - 1),
                    Background = System.Windows.Media.Brushes.Transparent,
                };
                Grid.SetColumn(stepLable, 0);
                Grid.SetRow(stepLable, currRows);
                switchGrid.Children.Add(stepLable);


                TextBox times = new TextBox
                {
                    FontSize = 14,
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    Text = "" + runTime.GetValue(i - 1),
                    Background = System.Windows.Media.Brushes.Transparent,
                };
                Grid.SetColumn(times, 1);
                Grid.SetRow(times, currRows);
                switchGrid.Children.Add(times);
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
                        BorderThickness = new Thickness(1),
                        Focusable = false,
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

                    // highlights /unhighlights row of buttons
                    onOff.FocusableChanged += (sender, e) =>
                    {
                        if (runningRow == Grid.GetRow(onOff))
                        {
                            if ((string)onOff.Content != "ON")
                            {
                                onOff.Background = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#E3BA00"));
                            }
                        }
                        else
                        {
                            if ((string)onOff.Content != "ON")
                            {
                                onOff.Background = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#223C69"));
                            }
                        }
                    };

                    Grid.SetRow(onOff, currRows);
                    Grid.SetColumn(onOff, b + 2);
                    switchGrid.Children.Add(onOff);
                }
                currRows++;
            }
        }


        /// <summary>
        /// Handles the UI event of the RunTimeCell value is not an integer.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
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
        #endregion
    }



    partial class OpticalSwitchControlSequence
    {
        #region REGION: Switch Test Running Components
        /// <summary>
        /// Initializes fields used run pause stop the program.
        /// </summary>
        /// <param name="input">The input.</param>
        private void SwitchRunPauseStopFieldIni()
        {
            runningRow = 2;
            running = false;
            pause = false;
            flashing = false;
            ProgressBar_ValueChanged(new object(), new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));


        }

        /// <summary>
        /// Handles the ValueChanged event of the ProgressBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Double}"/> instance containing the event data.</param>
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (running == false)
            {
                progressBar.Value = 0;
            }
            else if (running == true && pause == false)
            {
                progressBar.Value = e.NewValue;
                progressBar.Foreground = Brushes.Green;
            }
            else
            {
                progressBar.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Handles the Run event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Run(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("runningRow val : " + runningRow);
            //TODO
            if (runningRow == 2)
            {
                running = true;
                FlashingIni(sender, e);
                return;
            }
            else
            {
                notifier.ShowError("Test is already running", messageOptions);
            }
        }
        private async void FlashingIni(object sender, RoutedEventArgs e)
        {
            int rowTotal = switchGrid.RowDefinitions.Count;

            double progress = (100.0 * (double)(runningRow - 1) / (double)(rowTotal - 2));
            // Test is running:
            if (running == true && pause == false)
            {
                if (runningRow == 2)
                {
                    progressBar.Value = 0;
                    ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value + progress));
                }
                int rowLeft = rowTotal - 1;
                while (runningRow <= rowLeft && running == true)
                {
                    //Console.WriteLine("running row: " + runningRow + " | ruwLeft: " + rowLeft);
                    BarFlash();
                    ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value + progress));

                    // Keeps RunningRow from incrementing when stopped
                    if (running == true)
                    {
                        runningRow++;
                        await Task.Delay(1000);
                    }

                    //Test is paused:
                    await Task.Run(() =>
                    {
                        //if (pause) Console.WriteLine("paused line: " + runningRow);
                        while (pause == true && running == true) ;
                    });

                    BarUnFlash();
                    ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
                }
            }
            //Test is finished:
            if (runningRow == rowTotal)
            {
                Button_Click_Stop(sender, e);
                notifier.ShowSuccess("Test Completed", messageOptions);
            }

        }

        /// <summary>
        /// Handles the Pause event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Pause(object sender, RoutedEventArgs e)
        {
            pause = !pause;
            if (pause == true)
                notifier.ShowInformation("Test Paused", messageOptions);
        }

        /// <summary>
        /// Handles the Stop event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("flashing : " + flashing);
            //Console.WriteLine("stopped line : " + runningRow);
            if (flashing == true)
            {
                BarUnFlash();
            }
            SwitchRunPauseStopFieldIni();
            ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
            notifier.ShowWarning("Test Stopped", messageOptions);
        }

        /// <summary>
        /// Highlights the row
        /// </summary>
        private void BarFlash()
        {
            flashing = true;
            foreach (UIElement child in switchGrid.Children)
            {
                if (Grid.GetRow(child) == runningRow)
                {
                    if (child.GetType() == new Rectangle().GetType())
                    {
                        child.IsEnabled = !child.IsEnabled;
                    }
                    else if (child.GetType() == new Button().GetType())
                    {
                        child.Focusable = !child.Focusable;
                    }
                }
            }

        }
        /// <summary>
        /// Removes the highlight on the previously highlighted row 
        /// </summary>
        private void BarUnFlash()
        {
            //Console.WriteLine("unflashed row : " + (runningRow - 1));
            flashing = false;
            foreach (UIElement child in switchGrid.Children)
            {
                if (Grid.GetRow(child) == runningRow - 1)
                {
                    if (child.GetType() == new Rectangle().GetType())
                    {
                        child.IsEnabled = !child.IsEnabled;
                    }
                    else if (child.GetType() == new Button().GetType())
                    {
                        child.Focusable = !child.Focusable;
                    }
                }
            }
        }
        #endregion

    }

    partial class OpticalSwitchControlSequence
    {
        #region REGION IS INCOMPLETE: Contains code related to loading and reading existing test cases
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
            //String PTH = File.ReadAllText("Logic_PTH/" + type + ".PTH");

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

            //ArrayList stepAr = Read_Step_Array(test, totalSteps);

            //IO.DataContext = new SwitchGrid(totalSteps, type, title, stepAr);

        }

        //Only follows specific step test file format
        //private ArrayList Read_Step_Array(StringReader test, int totalSteps)
        //{
        //    ArrayList stepAr = new ArrayList();
        //    int i = 0;
        //    for (i = 0; test.Peek() != -1; i++)
        //    {
        //        SwitchStep step = new SwitchStep(System.Convert.ToDouble(Find_Value(test, '=')), System.Convert.ToInt32(Find_Value(test, '=')));
        //        Console.WriteLine(i + " time: " + step.Time1);
        //        Console.WriteLine(i + " index: " + step.Index1);
        //        stepAr.Add(step);
        //    }
        //    if (i != totalSteps)
        //    {
        //        MessageBox.Show("Please fix TotalSteps in Test file");
        //    }
        //    return stepAr;
        //}

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
    }

    partial class OpticalSwitchControlSequence
    {
        private void DiagramINI()
        {
        }
    }

}
