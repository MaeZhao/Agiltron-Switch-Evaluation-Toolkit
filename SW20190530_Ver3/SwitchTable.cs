using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SW20190530_Ver3
{
    class SwitchGrid
    {
        Window call;
        int totalSteps, output, input;
        string Ttitle;
        ArrayList stepArray;
        DataTable dt;

        public SwitchGrid(Window caller)
        {
            call = caller;
            //TODO used for user defined tests
        }

        public SwitchGrid(int tSteps, string type, string tableTitle, ArrayList stepA)
        { 
            totalSteps = tSteps;
            output = System.Convert.ToInt32(type.Substring(type.IndexOf("X")).Replace("X", String.Empty));
            input = System.Convert.ToInt32(type.Substring(2, type.IndexOf("X") - 1).Replace("X", String.Empty));
            Ttitle = tableTitle;
            stepArray = stepA;

        }
        /// <summary>
        /// Loads a switch grid:
        /// <para>outP is the # of output channels </para>
        /// inP is the # of input channels 
        /// <para/> rowSteps  is the # of steps 
        /// <para/>currOut is the current output channel, set to 0 when initiating a new switch grid
        /// </summary>
        public Grid Load_Grid(int outP, int inP, int rowSteps, int currOut)
        {
            Grid switchGrid = new Grid();

            switchGrid.HorizontalAlignment = HorizontalAlignment.Left;
            switchGrid.VerticalAlignment = VerticalAlignment.Top;
            switchGrid.ShowGridLines = true;
            switchGrid.Background = Brushes.White;
            //When current output is not 0
            if (currOut != 0)
            {
                RowDefinition outLable = new RowDefinition();
                switchGrid.RowDefinitions.Add(outLable);
                outLable.Height = new GridLength(0, GridUnitType.Auto);
                //Outputs for each Channel
                for (int i = 1; i <= inP; i++)
                {
                    ColumnDefinition sPath = new ColumnDefinition();
                    //sPath.Width = new GridLength(40, GridUnitType.Auto);
                    
                        TextBlock outNum = new TextBlock();
                        outNum.FontSize = 12;
                        outNum.Text = "Out " + i;
                        Grid.SetRow(outNum, 0);
                        Grid.SetColumn(outNum, i - 1);
                        switchGrid.Children.Add(outNum);
                    
                    switchGrid.ColumnDefinitions.Add(sPath);
                        
                    //Generates each row/step
                    for (int n = 1; n <= rowSteps; n++)
                    {
                        RowDefinition step = new RowDefinition();
                        step.Height = new GridLength(40, GridUnitType.Auto);
                        switchGrid.RowDefinitions.Add(step);

                        //ON OFF button generation
                        Button onOff = new Button();
                        onOff.FontSize = 14;
                        onOff.VerticalAlignment = VerticalAlignment.Center;
                        onOff.HorizontalAlignment = HorizontalAlignment.Center;
                        onOff.Content = false;
                        //ON OFF button Click + Logic
                        onOff.Click += (sender, b) =>
                        {
                            int r = Grid.GetRow(onOff);
                            //ON OFF button Logic
                            //for(int c = 1; c<=inP; c++)
                            //{
                            //    if (c != Grid.GetColumn(onOff))
                            //    {
                            //        SetButton(switchGrid, r, c, false);
                            //    }
                            //}
                            onOff.Content = !(bool)onOff.Content;
                        };

                        Grid.SetRow(onOff, n);
                        Grid.SetColumn(onOff, i - 1);

                        switchGrid.Children.Add(onOff);

                    }
                }
            }
            else
            {
                //General grouping lables for multiple inputs
                for (int chanNum = 1; chanNum <= outP; chanNum++) 
                {
                    RowDefinition channelLable = new RowDefinition();
                    channelLable.Height = new GridLength(40, GridUnitType.Auto);
                    RowDefinition switchOp = new RowDefinition();
                    switchOp.Height = new GridLength(40, GridUnitType.Auto);

                    switchGrid.RowDefinitions.Add(channelLable);
                    switchGrid.RowDefinitions.Add(switchOp);

                    Grid oSwitch = Load_Grid(1, inP, rowSteps, chanNum);
                    Grid.SetColumn(oSwitch, chanNum - 1);
                    Grid.SetRow(oSwitch, 1);
                    switchGrid.Children.Add(oSwitch);

                    ColumnDefinition lable = new ColumnDefinition();
                    //lable.Width = new GridLength(40, GridUnitType.Auto);

                    switchGrid.ColumnDefinitions.Add(lable);

                    TextBlock group = new TextBlock();
                    group.Text = "Channel " + chanNum;
                    group.FontSize = 15;
                    group.FontWeight = FontWeights.Bold;
                    Grid.SetColumn(group, chanNum - 1);
                    switchGrid.Children.Add(group);
                }
            }
            return switchGrid;
        }



        //Set button does not work
        private static void SetButton(Grid grid, int row, int column, bool val)
        {
            foreach (Button c in grid.Children)
            {
                if (Grid.GetRow(c) == row
                      &&
                   Grid.GetColumn(c) == column)
                {
                    c.Content = val;
                    break;
                }
            }
        }



        //Load default data grid DataGrid
        private void Load_Table_Headers()
        {
            
            for (int sw = 1; 1 < output; output++) //for each switch, sw value should not be used to represent a switch
            {
                

            }
        } 

        //ONLY when loading a Table
        public DataTable Create_Switch_Table()
        {

            DataColumn lable = new DataColumn();
            lable.ColumnName = "Step(s)";
            lable.DataType = Type.GetType("System.Int32");
            lable.AutoIncrement = true;
            dt.Columns.Add(lable);

            //Columns
            for(int i = 0; i<output; i++)
            {
                DataColumn inOut = new DataColumn();
                inOut.DataType = Type.GetType("System.Boolean");
                inOut.ColumnName = input + "-" + output;
                inOut.AutoIncrement = true;
                dt.Columns.Add(inOut);
            }

            //Rows
            DataRow steps;
            for (int i=1; i <=totalSteps; i++)
            {
                steps = dt.NewRow();
                steps["Step(s)"] = i;

            }

            return dt;
        }

        public void Add_Step()
        {
            DataRow steps;
            steps = dt.NewRow();
            steps["Step(s)"] = 1;
        }
    }
}
