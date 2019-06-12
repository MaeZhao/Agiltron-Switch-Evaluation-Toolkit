using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SW20190530_Ver3
{
    /// <summary>
    /// Represents each step used in OpticalSwitchControlSequence 
    /// </summary>
    class SwitchStep
    {
        double Time;
        int Index;
        
        public SwitchStep(double time, int index)
        {
            Time = time;
            Index = index;
        }

        public double Time1 { get => Time; set => Time = value; }
        public int Index1 { get => Index; set => Index = value; }
    }
}
