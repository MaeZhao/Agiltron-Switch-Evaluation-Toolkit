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
    /// Interaction logic for TruthTale.xaml
    /// </summary>
    public partial class TruthTable : WindowUIComponents
    {
        private Notifier notifier; //From ToastNotifications v2 nuget pkg
        private ToastNotifications.Core.MessageOptions messageOptions;  //From ToastNotifications v2 nuget pkg
                                                                        //switchGrid specs
        private int inChannelNum, outSwitchNum, steps;

        public TruthTable()
        {
            InitializeComponent();
            //Switch Title:
            Viewbox TestRunTitle = new Viewbox
            {
                //Text = "" + inChannelNum + " - " + outSwitchNum + " SWITCH CONTROL",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Stretch = Stretch.Uniform,
                StretchDirection = StretchDirection.Both,
                //FontWeight = FontWeights.ExtraBold,
                //TextWrapping = TextWrapping.Wrap,
                //Foreground = Brushes.White,
                //TextDecorations = TextDecorations.Underline,
                MaxHeight = 30
            };
            TextBlock Title = new TextBlock
            {
                Text = "" + inChannelNum + " - " + outSwitchNum + " TRUTH TABLE",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.White,
            };

            TestRunTitle.Child = Title;

            Grid.SetRow(TestRunTitle, 1);
            Grid.SetColumn(TestRunTitle, 1);
            WindowBar.Children.Add(TestRunTitle);
            steps = 1;
            LoadGrid(new int[] { });
        }


        private void NewClick(object sender, RoutedEventArgs e)
        {
            //todo
        }
        private void AddStepClick(object sender, RoutedEventArgs e)
        {
            steps++;
            AddPaths(1, outSwitchNum * inChannelNum, new int[] { });
        }



        public void AddPaths(int rows, int col, int[] runTime)
        {
            GridLength WxH = new GridLength(2, GridUnitType.Auto);

            int currRows = truthTable.RowDefinitions.Count;
            for (int r = 1; r <= rows; r++)
            {
                RowDefinition nRow = new RowDefinition
                {
                    Height = WxH
                };
                truthTable.RowDefinitions.Add(nRow);

                TextBlock stepLable = new TextBlock
                {
                    Text = "" + (currRows - 1),
                    Style = Application.Current.Resources["TextBlockLables"] as Style,
                };

                Grid.SetColumn(stepLable, 0);
                Grid.SetRow(stepLable, currRows);
                truthTable.Children.Add(stepLable);


                for (int c = 0; c < col; c++)
                {
                    //Truth Table button generation
                    Button onOff = new Button
                    {
                        Style = Application.Current.Resources["OFFButton"] as Style,
                    };
                    //ON OFF button Click UI + Logic
                    onOff.Click += (sender, e) =>
                    {
                        if ((string)onOff.Content == "OFF")
                        {
                            onOff.Background = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3A402"));
                            onOff.Content = "ON";
                        }
                        else
                        {
                            onOff.Background = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#3A5894"));
                            onOff.Content = "OFF";
                        }

                    };
                    Grid.SetRow(onOff, currRows);
                    Grid.SetColumn(onOff, c + 1);
                    truthTable.Children.Add(onOff);
                }
                currRows++;
            }
            truthTable.UpdateLayout();
        }


        public void LoadGrid(int[] runtime)
        {
            GridLength Adjustable = new GridLength(2, GridUnitType.Star);
            GridLength Stiff = new GridLength(2, GridUnitType.Auto);

            ColumnDefinition stepNum = new ColumnDefinition
            {
                Width = Stiff
            };

            truthTable.ColumnDefinitions.Add(stepNum);

            RowDefinition chanNum = new RowDefinition
            {
                Height = Adjustable
            };
            RowDefinition outNum = new RowDefinition
            {
                Height = Adjustable
            };
            truthTable.RowDefinitions.Add(chanNum);
            truthTable.RowDefinitions.Add(outNum);


            //Step Title
            TextBlock stepT = new TextBlock
            {
                //BorderBrush = System.Windows.Media.Brushes.Black,
                Text = "Step(s)",
                Padding = new Thickness(20),
                Style = Application.Current.Resources["TextBlockLables"] as Style,
            };
            Grid.SetColumn(stepT, 0);
            Grid.SetRow(stepT, 0);
            Grid.SetRowSpan(stepT, 1);
            truthTable.Children.Add(stepT);

            //Generating Output Lables
            int currOutSt = 1;
            int currChanSt = 1;
            for (int c = 0; c < inChannelNum; c++)
            {
                for (int o = 0; o < outSwitchNum; o++)
                {
                    currOutSt++;
                    ColumnDefinition cOut = new ColumnDefinition
                    {
                        Width = Adjustable
                    };
                    truthTable.ColumnDefinitions.Add(cOut);

                    TextBlock outLable = new TextBlock
                    {
                        Style = Application.Current.Resources["TextBlockLables"] as Style,
                        Text = (c + 1) + "-" + (o + 1),
                    };
                    Grid.SetColumn(outLable, currOutSt);
                    Grid.SetRow(outLable, 1);
                    truthTable.Children.Add(outLable);
                }
                TextBlock chanLable = new TextBlock
                {
                    Style = Application.Current.Resources["TextBlockLables"] as Style,
                    Text = "Channel " + (c + 1)
                };
                Grid.SetColumn(chanLable, currChanSt);
                Grid.SetRow(chanLable, 0);
                Grid.SetColumnSpan(chanLable, outSwitchNum);
                truthTable.Children.Add(chanLable);
                currChanSt += outSwitchNum;
            }
            AddPaths(steps, outSwitchNum * inChannelNum, runtime);
        }

        /* TODO:
         * Find faster way to load io and unload io taking better advantage of the dictionary*/


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

