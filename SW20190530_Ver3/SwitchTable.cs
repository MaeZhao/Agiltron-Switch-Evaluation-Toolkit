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
        int totalSteps, output, input;
        string Ttitle;
        ArrayList stepArray;
        DataTable dt;

        public SwitchGrid()
        {
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
        //Load default data grid DataGrid TURN INTO CLASS
        public Grid Load_Grid(int outP, int inP, int rowSteps)
        {
            Grid switchGrid = new Grid();
            //switchGrid.Width = (double)GridUnitType.Star;
            //switchGrid.Height = (double)GridUnitType.Star;
            switchGrid.HorizontalAlignment = HorizontalAlignment.Left;
            switchGrid.VerticalAlignment = VerticalAlignment.Top;
            switchGrid.ShowGridLines = true;
            switchGrid.Background = Brushes.White;
            RowDefinition s = new RowDefinition();
            //step.Height = new GridLength(0, GridUnitType.Auto);
            switchGrid.RowDefinitions.Add(s);
            if (outP == 1)
            {
                for (int i = 1; i < inP; i++)
                {
                    ColumnDefinition sPath = new ColumnDefinition();
                    //sPath.Width = new GridLength(0, GridUnitType.Auto);
                    switchGrid.ColumnDefinitions.Add(sPath);
                }
                for (int i = 1; i < rowSteps; i++)
                {
                    RowDefinition step = new RowDefinition();
                    //step.Height = new GridLength(0, GridUnitType.Auto);
                    switchGrid.RowDefinitions.Add(step);
                }
            }
            else
            {
                //General grouping lables for multiple inputs
                for (int sw = 1; sw <= outP; sw++) //for each switch, sw value should not be used to represent a switch
                {
                    Grid oSwitch = Load_Grid(1, inP, rowSteps);
                    Grid.SetColumn(oSwitch, sw-1);
                    Grid.SetRow(oSwitch, 1);
                    switchGrid.Children.Add(oSwitch);

                    ColumnDefinition lable = new ColumnDefinition();
                    lable.Width = new GridLength(0, GridUnitType.Auto);
                    switchGrid.ColumnDefinitions.Add(lable);

                    TextBlock group = new TextBlock();
                    group.Text = "Output " + sw;
                    group.FontSize = 12;
                    group.FontWeight = FontWeights.Bold;
                    Grid.SetColumn(group, sw - 1);
                    switchGrid.Children.Add(group);
                }
            }
            return switchGrid;
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
