using DiagramDesigner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using ToastNotifications.Messages;


/* Dynamically loads Switch ON/OFF Grid Components (before running)
 */
namespace SW20190530_Ver3
{
    partial class OpticalSwitchControlSequence
    {

        /// <summary>
        /// Initializes Switch Board Grid components fields.
        /// Calls Switch Board Grid controls.
        /// </summary>
        /// <param name="input">The input.</param>
        private void SwitchBoardFieldIni(MainWin input)
        {
            String type = input.type.Text;
            type = type.Replace(" ", String.Empty);
            type = type.Substring(0, type.IndexOf('('));

            outSwitchNum = System.Convert.ToInt32(type.Substring(type.IndexOf("X")).Replace("X", String.Empty));
            inChannelNum = System.Convert.ToInt32(type.Substring(2, type.IndexOf("X") - 1).Replace("X", String.Empty));

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
}
