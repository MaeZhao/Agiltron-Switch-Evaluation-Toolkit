using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW20190530_Ver3
{
    class SwitchTable
    {
        int totalSteps, input, output;
        string Ttitle, boardConfig;
        ArrayList stepArray;

        public SwitchTable(int tSteps, int inp, int outp, string tableTitle, string bConfig, ArrayList stepA)
        { 
            totalSteps = tSteps;
            input = inp;
            output = outp;
            Ttitle = tableTitle;
            boardConfig = bConfig;
            stepArray = stepA;
        }

        public DataTable Create_Switch_Table()
        {
            DataTable dt = new DataTable(Ttitle);
            DataRow steps;
            //Rows
            steps = dt.NewRow();


            for(int i=0; i <totalSteps; i++)
            {
                
            }
            
            
        }
    }
}
