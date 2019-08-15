using DiagramDesigner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace SW20190530_Ver3
{
    /// <summary>
    /// Interaction logic for OpticalSwitchControlSequence.xaml.
    /// Contains code for grid on/off table, MenuItem controls (incomplete) as well as loads diagram formation.
    /// </summary>
    /// <seealso cref="SW20190530_Ver3.WindowUIComponents" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class OpticalSwitchControlSequence : WindowUIComponents
    {
        #region All Fields
        //UI fields
        private Boolean offline;
        private Notifier notifier; //From ToastNotifications v2 nuget pkg
        private ToastNotifications.Core.MessageOptions messageOptions;  //From ToastNotifications v2 nuget pkg
        //switchGrid specs
        private int inChannelNum, outSwitchNum;
        //running/pausing button controls + diagram animations
        private bool running;
        private bool pause;
        private bool flashing;
        private int runningRow; //Current highlighted row
        private List<int[]> activeIOpairs = new List<int[]>(); //List of active inChannel and outSwitch pairs
        Dictionary<int, Connection> runningUIconnection = new Dictionary<int, Connection>(); //Key value-->(connectionID, Arrow)
        //switchDiagram Specs
        List<DesignerItem> inp = new List<DesignerItem>(); //List of input Nodes
        List<DesignerItem> oup = new List<DesignerItem>(); //List of output Nodes
        #endregion

        /// <summary>
        /// Initialization of OpticalSwitchControlSequence
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
                    offsetX: 0,
                    offsetY: WindowBar.ActualHeight);
                cfg.DisplayOptions.TopMost = true;
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(10),
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

            // Offline notification display
            offline = input.offlineButton.IsChecked.Value;
            input.Close();
            this.Show();
            if (offline)
            {
                notifier.ShowInformation("Running in Offline Mode", messageOptions);
            }

            #region Initializes/finds switchGrid specs [REGION A]
            String type = input.type.Text;
            type = System.Text.RegularExpressions.Regex.Replace(type, @"[^X0-9]", String.Empty);

            outSwitchNum = System.Convert.ToInt32(type.Substring(type.IndexOf("X")).Replace("X", String.Empty));
            inChannelNum = System.Convert.ToInt32(type.Substring(0, type.IndexOf("X")));

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
                Text = "" + inChannelNum + " - " + outSwitchNum + " SWITCH CONTROL",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.ExtraBold,
                FontSize = 30,
                Foreground = Brushes.White,
                //TextDecorations = TextDecorations.Underline,
                Margin = new Thickness(0, -20, 0, 30)
            };
            Grid.SetRow(TestRunTitle, 1);
            Grid.SetColumnSpan(TestRunTitle, 2);
            Title.Children.Add(TestRunTitle);
            Grid.SetRow(Title, 1);

            Grid.SetColumnSpan(Title, 3);
            Main.Children.Add(Title);

            //initializes switchGrid:
            Load_Grid(20, new int[] { });

            runningRow = 2;
            #endregion

            #region Initializes switch Diagram [REGION B inactive]
            //SwitchDiagramCircleIni();
            #endregion

            #region Initializes switch test run specs [REGION C]
            SwitchRunControlsIni();
            #endregion

            this.MaxWidth = GetWidth();
            this.MaxHeight = SystemParameters.WorkArea.Height;
        }

        private static double GetWidth()
        {
            return SystemParameters.WorkArea.Width;
        }
    }

    #region REGION A:Dynamically loads Switch ON/OFF Grid Components (before running)
    partial class OpticalSwitchControlSequence : WindowUIComponents
    {
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
            for (int c = 0; c < inChannelNum; c++)
            {
                for (int o = 0; o < outSwitchNum; o++)
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
                Grid.SetColumnSpan(chanLable, outSwitchNum);
                switchGrid.Children.Add(chanLable);
                currChanSt += outSwitchNum;
            }
            AddStepsButtonRT_UI(steps, outSwitchNum * inChannelNum, runtime);
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
            for (int r = 1; r <= rows; r++)
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
                Grid.SetColumnSpan(highlight, 2);
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
                    Text = "" + runTime.GetValue(r - 1),
                    Background = System.Windows.Media.Brushes.Transparent,
                };
                Grid.SetColumn(times, 1);
                Grid.SetRow(times, currRows);
                switchGrid.Children.Add(times);
                times.LostFocus += RunTimeCell_UI; //initializes runtime UI

                for (int c = 0; c < col; c++)
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
                    Grid.SetColumn(onOff, c + 2);
                    switchGrid.Children.Add(onOff);
                }
                currRows++;
            }
        }



        /* TODO:
         * Find faster way to load io and unload io taking better advantage of the dictionary*/
        /// <summary>
        /// Loads the arrow connection corresponding to ON buttons on runningRow into diagram window.
        /// </summary>
        private void LoadIOConnection()
        {
            int buttonRow = runningRow;

            foreach (UIElement child in switchGrid.Children)
            {
                if (child.GetType() == typeof(Button))
                {
                    Button c = child as Button;
                    if (Grid.GetRow(child) == buttonRow && (string)c.Content == "ON")
                    {
                        int bcol = Grid.GetColumn(c) - 2;
                        int inBnum = (int)((double)bcol / (double)(switchGrid.ColumnDefinitions.Count - 2)); // Active child channel number
                        int outBnum = bcol % outSwitchNum; // Active child switch output number
                        //Connection arrow = Draw_Arrow(inp[inBnum], oup[outBnum]);
                        int connectionID = int.Parse(inBnum.ToString() + outBnum.ToString()); //Unique identifier for each connection

                        //runningUIconnection.Add(connectionID, arrow);
                        //switchDiagram.UpdateLayout();
                    }
                }
            }
        }

        /// <summary>
        /// Unloads the arrow connection corresponding to ON buttons on previous runningRow into diagram window.
        /// </summary>
        private void UnloadIOConnection()
        {
            int buttonRow = runningRow - 1;
            if (flashing == false)
            {
                foreach (UIElement child in switchGrid.Children)
                {
                    if (child is Button)
                    {
                        Button c = child as Button;


                        if (Grid.GetRow(child) == buttonRow && (string)c.Content == "ON")
                        {
                            int bcol = Grid.GetColumn(c) - 2;
                            int inBnum = (int)((double)bcol / (double)(switchGrid.ColumnDefinitions.Count - 2)); // Active child channel number
                            int outBnum = bcol % outSwitchNum; // Active child switch output number

                            int connectionID = int.Parse(inBnum.ToString() + outBnum.ToString());
                            Connection activeArrow = runningUIconnection[connectionID];
                            activeArrow.Visibility = Visibility.Collapsed;
                            runningUIconnection.Remove(connectionID);
                        }
                    }
                }
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
    }
    #endregion

    #region REGION C: Switch Test Running Controls: initiates events that trigger button and arrow flashing/unflashing
    partial class OpticalSwitchControlSequence : WindowUIComponents
    {
        /// <summary>
        /// Initializes fields used for run/pause/stop/loop abilities during testing
        /// </summary>
        private void SwitchRunControlsIni()
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
            if (running == false)
            {
                SwitchRunControlsIni();
                running = true;
                Animate(sender, e);
                return;
            }
            else
            {
                notifier.ShowError("Test is already running", messageOptions);
            }
        }
        /// <summary>
        /// Iterates through flashing animation of the graph rows and diagram arrows
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void Animate(object sender, RoutedEventArgs e)
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
                    Flash();
                    ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value + progress));

                    // Keeps RunningRow from incrementing when stopped
                    if (running == true)
                    {
                        await Task.Delay(1000);
                        runningRow++;
                    }

                    //IF test is paused:
                    await Task.Run(() =>
                    {
                        //Waits until unpaused or stops running
                        while (pause == true && running == true)
                        {
                            ;
                        }

                    });
                    /* TODO:
                    * -Check if the following "if/else" statement is being used at all */
                    if (running == true)
                    {
                        UnFlash();
                        ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
                    }
                    else
                    {
                        break;
                    }
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
            if (pause == true && running == true)
            {
                notifier.ShowInformation("Test Paused", messageOptions);
            }
            else
            {
                notifier.ShowError("Test not Running", messageOptions);
            }
        }

        /// <summary>
        /// Handles the Stop event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            if (pause != true)
            {
                runningRow++;
            }

            UnFlash();
            SwitchRunControlsIni();
            running = false;
            notifier.ShowInformation("Test Stopped", messageOptions);
        }
        /// <summary>
        /// Flashes visual diagrams.
        /// </summary>
        private void Flash()
        {
            RowHighlight();
            LoadIOConnection();
        }
        /// <summary>
        /// UnFlashes visual diagrams.
        /// </summary>
        private void UnFlash()
        {
            RowUnhiglight();
            UnloadIOConnection();
        }

        /// <summary>
        /// Highlights the runningRow
        /// </summary>
        private void RowHighlight()
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
        private void RowUnhiglight()
        {
            //Console.WriteLine("unflashed row : " + (runningRow - 1));
            flashing = false;
            int r = runningRow - 1;
            foreach (UIElement child in switchGrid.Children)
            {
                if (Grid.GetRow(child) == r)
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
    }
    #endregion

    #region REGION B (inactive):Loads Switch Diagram components: input/output node formation + arrow generation/degeneration

    //    partial class OpticalSwitchControlSequence
    //    {
    //        /// <summary>
    //        /// Structure used to map out the positions in our input/output diagram
    //        /// </summary>
    //        public struct Radius
    //        {
    //            private double space;
    //            private double xOffset;
    //            private double yOffset;
    //            private double diameter;
    //            private double[] centerXY;
    //            private double height;

    //            public double Space { get => space; }
    //            public double XOffset { get => xOffset; }
    //            public double YOffset { get => yOffset; }
    //            public double Diameter { get => diameter; }
    //            public double[] CenterXY { get => centerXY; }
    //            public double Height { get => height; }
    //            /// <summary>
    //            /// Initializes a new instance of the <see cref="Radius"/> struct.
    //            /// </summary>
    //            /// <param name="outNum">The out number.</param>
    //            /// <param name="h">The h.</param>
    //            /// <param name="offX">The off x.</param>
    //            /// <param name="offY">The off y.</param>
    //            /// <param name="s">The s.</param>
    //            public Radius(int outNum, double h, double offX, double offY, double s)
    //            {
    //                height = h;
    //                space = s;
    //                xOffset = offX;
    //                yOffset = offY + h;
    //                diameter = (height + space) * outNum;
    //                centerXY = new double[2];
    //                centerXY[0] = xOffset;
    //                centerXY[1] = yOffset + diameter + height / 2;
    //            }
    //            /// <summary>
    //            /// Initializes a new instance of the <see cref="Radius"/> struct.
    //            /// </summary>
    //            /// <param name="outNum">The out number.</param>
    //            /// <param name="h">The h.</param>
    //            public Radius(int outNum, double h)
    //            {
    //                height = h;
    //                space = 20;
    //                diameter = (height + space) * outNum * 1.5;
    //                yOffset = -(height + space) * (outNum) * 0.25;
    //                xOffset = 30;
    //                centerXY = new double[2];
    //                centerXY[0] = xOffset;
    //                centerXY[1] = diameter / 2 + 10 + height / 2.0 + yOffset;
    //            }
    //        }

    //        /// <summary>
    //        /// Places index and nodes onto diagram canvas
    //        /// </summary>
    //        private void SwitchDiagramCircleIni()
    //        {
    //            if (inChannelNum == 1)
    //            {
    //                double height = 45;
    //                double width = 90;
    //                double totalH = switchDiagram.ActualHeight;
    //                double totalW = diagramBounds.ActualWidth;
    //                Radius radius = new Radius(outSwitchNum, height);
    //                //Places input Node
    //                PlaceNewNode(true, height, width, 1, "#62C1AF", radius.CenterXY);
    //                //Finds outputnode indexes and places output Nodes
    //                Dictionary<int, double[]> outNodeIndex = OutputNodeIndexGenerator(radius);
    //                for (int i = 1; i <= outNodeIndex.Count; i++)
    //                {
    //                    PlaceNewNode(false, height, width, i, "#F3A414", outNodeIndex[i]);
    //                }
    //            }
    //            else
    //            {
    //                SwitchDiagramIni();
    //            }
    //            switchDiagram.UpdateLayout();
    //            /*TODO: make this part of the style/template?
    //             */
    //            RemoveConnectors("Left", inp);
    //            RemoveConnectors("Right", oup);
    //        }

    //        /// <summary>
    //        /// Generates output node indexes.
    //        /// </summary>
    //        /// <param name="rad">The RAD.</param>
    //        /// <returns></returns>
    //        private Dictionary<int, double[]> OutputNodeIndexGenerator(Radius rad)
    //        {
    //            Dictionary<int, double[]> outNodeIndex = new Dictionary<int, double[]>();
    //            double radLength = rad.Diameter / 2.0;
    //            double NodeHeight = rad.Height;
    //            double NodeSpace = rad.Space;

    //            //Add first node
    //            double yNode = 0.0;
    //            //outNodeIndex.Add(1, new double[] { rad.CenterXY[0], yNode });
    //            for (int NodeID = 1; NodeID <= outSwitchNum; NodeID++)
    //            {
    //                if (NodeID == 1)
    //                {
    //                    yNode = rad.CenterXY[1] - (NodeHeight + NodeSpace) * outSwitchNum / 2.0;
    //                }
    //                else
    //                {
    //                    yNode += NodeHeight + NodeSpace;
    //                }

    //                if (yNode <= rad.CenterXY[1])
    //                {
    //                    double NodeAngle = Math.Acos((rad.CenterXY[1] - yNode) / radLength);
    //                    outNodeIndex.Add(NodeID, new double[] { radLength * Math.Sin(NodeAngle) + rad.CenterXY[0], yNode });
    //                }
    //                else if (yNode > rad.CenterXY[1])
    //                {
    //                    double NodeAngle = Math.Asin((yNode - rad.CenterXY[1]) / radLength);
    //                    outNodeIndex.Add(NodeID, new double[] { (radLength * Math.Cos(NodeAngle)) + rad.CenterXY[0], yNode });
    //                }
    //            }
    //            return outNodeIndex;
    //        }
    //        /// <summary>
    //        /// Places the new node on diagram canvas.
    //        /// </summary>
    //        /// <param name="input">if set to <c>true</c> [input].</param>
    //        /// <param name="height">The height.</param>
    //        /// <param name="width">The width.</param>
    //        /// <param name="nodeID">The node identifier.</param>
    //        /// <param name="color">The color.</param>
    //        /// <param name="xyPosition">The xy position.</param>
    //        private void PlaceNewNode(Boolean input, double height, double width, int nodeID, string color, double[] xyPosition)
    //        {
    //            double xPosition = xyPosition[0];
    //            double yPosition = xyPosition[1];
    //            string text = "";
    //            DesignerItem Node = new DesignerItem
    //            {
    //                Height = height,
    //                Width = width,
    //                Visibility = Visibility.Visible,
    //                OverridesDefaultStyle = false,
    //            };

    //            Grid NodeContent = new Grid
    //            {
    //                IsHitTestVisible = false,
    //                Focusable = false,
    //            };

    //            Path NodeShape = new System.Windows.Shapes.Path
    //            {
    //                Style = Application.Current.Resources["Start"] as Style,
    //                Fill = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
    //                Visibility = Visibility.Visible,
    //                Height = height,
    //                Width = width,
    //                Stroke = Brushes.Transparent,
    //            };
    //            if (input == true)
    //            {
    //                text = "input #" + nodeID;
    //            }
    //            else
    //            {
    //                text = "output #" + nodeID;
    //            }

    //            TextBlock name = new TextBlock
    //            {
    //                Text = text,
    //                HorizontalAlignment = HorizontalAlignment.Center,
    //                VerticalAlignment = VerticalAlignment.Center,
    //                FontWeight = FontWeights.DemiBold,
    //                FontSize = 12,
    //                Foreground = Brushes.Black,
    //                MaxHeight = height,
    //                MaxWidth = width - 10,
    //            };

    //            NodeContent.Children.Add(NodeShape);
    //            NodeContent.Children.Add(name);
    //            Node.Content = NodeContent;

    //            Canvas.SetTop(Node, yPosition);
    //            Canvas.SetLeft(Node, xPosition);
    //            if (input == true)
    //            {
    //                inp.Add(Node);
    //            }
    //            else
    //            {
    //                oup.Add(Node);
    //            }
    //            switchDiagram.Children.Add(Node);
    //        }
    //        /// <summary>
    //        /// Initializeds fields and node formation in Switch Diagram
    //        /// </summary>
    //        private void SwitchDiagramIni()
    //        {
    //            //double space = switchDiagram.ActualHeight;
    //            double totalW = diagramBounds.ActualWidth;
    //            double height = 60;
    //            double width = 80;
    //            //double yPositionInp = (space / 2) - (numChannel / 2.0 * 50.0);
    //            //double yPositionOutp = (space / 2) - (numOut / 2.0 * 50.0);
    //            double yPositionInp = 40;
    //            double yPositionOutp = 40;

    //            for (int i = 1; i <= inChannelNum; i++)
    //            {
    //                DesignerItem Node = new DesignerItem
    //                {
    //                    Height = height,
    //                    Width = width,
    //                    Visibility = Visibility.Visible,
    //                    OverridesDefaultStyle = false,
    //                    Name = "in_" + i,
    //                };

    //                Grid NodeContent = new Grid
    //                {
    //                    IsHitTestVisible = false,
    //                    Focusable = false,
    //                };
    //                Path NodeShape = new System.Windows.Shapes.Path
    //                {
    //                    Style = Application.Current.Resources["Card"] as Style,
    //                    Fill = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#62C1AF")),
    //                    Visibility = Visibility.Visible,
    //                    Height = height,
    //                    Width = width,
    //                    Stroke = Brushes.Transparent,
    //                };
    //                TextBlock name = new TextBlock
    //                {
    //                    Text = "in #" + i,
    //                    HorizontalAlignment = HorizontalAlignment.Center,
    //                    VerticalAlignment = VerticalAlignment.Center,
    //                    FontWeight = FontWeights.DemiBold,
    //                    FontSize = 15,
    //                    Foreground = Brushes.Black,
    //                    MaxHeight = 50,
    //                    MaxWidth = 80,

    //                };
    //                NodeContent.Children.Add(NodeShape);
    //                NodeContent.Children.Add(name);
    //                Node.Content = NodeContent;

    //                Canvas.SetTop(Node, yPositionInp);
    //                Canvas.SetLeft(Node, 5);
    //                yPositionInp += 80;
    //                inp.Add(Node);
    //                switchDiagram.Children.Add(Node);
    //            }

    //            for (int i = 1; i <= outSwitchNum; i++)
    //            {
    //                DesignerItem Node = new DesignerItem
    //                {
    //                    Height = height,
    //                    Width = width,
    //                    Visibility = Visibility.Visible,
    //                    OverridesDefaultStyle = false,
    //                    Name = "out_" + i,
    //                };

    //                Grid NodeContent = new Grid
    //                {
    //                    IsHitTestVisible = false,
    //                    Focusable = false,
    //                };
    //                Path NodeShape = new System.Windows.Shapes.Path
    //                {
    //                    Style = Application.Current.Resources["Card"] as Style,
    //                    Fill = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3A414")),
    //                    Visibility = Visibility.Visible,
    //                    Height = height,
    //                    Width = width,
    //                    Stroke = Brushes.Transparent,
    //                };
    //                TextBlock name = new TextBlock
    //                {
    //                    Text = "out #" + i,
    //                    HorizontalAlignment = HorizontalAlignment.Center,
    //                    VerticalAlignment = VerticalAlignment.Center,
    //                    FontWeight = FontWeights.DemiBold,
    //                    FontSize = 15,
    //                    Foreground = Brushes.Black,
    //                    MaxHeight = 50,
    //                    MaxWidth = 80,

    //                };
    //                NodeContent.Children.Add(NodeShape);
    //                NodeContent.Children.Add(name);
    //                Node.Content = NodeContent;

    //                Canvas.SetTop(Node, yPositionOutp);
    //                switchGrid.UpdateLayout();
    //                Canvas.SetLeft(Node, totalW - 700);
    //                yPositionOutp += 80;
    //                oup.Add(Node);
    //                switchDiagram.Children.Add(Node);
    //            }
    //            switchDiagram.UpdateLayout();
    //            RemoveConnectors("Left", inp);
    //            RemoveConnectors("Right", oup);
    //        }

    //        /// <summary>
    //        /// Removes the specified connector square (via connectorKey) from all nodes in item list.
    //        /// </summary>
    //        /// <param name="connectorKey">The connector key.</param>
    //        /// <param name="items">The items.</param>
    //        private void RemoveConnectors(String connectorKey, List<DesignerItem> items)
    //        {
    //            foreach (DesignerItem i in items)
    //            {
    //                i.RemoveConnector(connectorKey);
    //            }
    //        }

    //        /// <summary>
    //        /// Draws Arrow connection between input and output nodes
    //        /// </summary>
    //        /// <param name="inNode">The input node.</param>
    //        /// <param name="outNode">The output node.</param>
    //        /// <returns>Connected arrow between input and output nodes</returns>
    //        private Connection Draw_Arrow(DesignerItem inNode, DesignerItem outNode)
    //        {
    //            Connector pR = inNode.GetConnector("Right");
    //            Connector cL = outNode.GetConnector("Left");
    //            ConnectorAdorner sinkAdorner = new ConnectorAdorner(switchDiagram, cL);

    //            Connection connectedArrow = new Connection(pR, cL);

    //            switchDiagram.Children.Add(connectedArrow);
    //            switchDiagram.UpdateLayout();
    //            return connectedArrow;
    //        }
    //    }

    #endregion


}
