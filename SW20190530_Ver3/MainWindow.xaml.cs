using Microsoft.Win32;
using System;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
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
using ComboBox = System.Windows.Controls.ComboBox;


namespace SW20190530_Ver3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWin : Window
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWin"/> class.
        /// </summary>
        public MainWin()
        {
            InitializeComponent();
            this.MaxWidth = SystemParameters.WorkArea.Width;
            this.MaxHeight = SystemParameters.WorkArea.Height;
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
            RECT rect = GetWindowRectangle(this);
            if (this.WindowState == WindowState.Maximized)
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
            this.Height = 580;
            this.Width = 750;

        }
        #endregion

        /// <summary>
        /// Handles the Exit event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Exit(object sender, RoutedEventArgs e) => System.Windows.Application.Current.Shutdown(99);
        /// <summary>
        /// Handles the Exit event of the Button_Min control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Min_Exit(object sender, RoutedEventArgs e) => SystemCommands.MinimizeWindow(this);

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                this.Top = GetMousePositionY();
                this.Left = (int)(GetMousePositionX() - this.Width / 2);
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


    public partial class MainWin
    {
        /// <summary>
        /// Handles the Continue event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Continue(object sender, RoutedEventArgs e)
        {
            this.IsEnabled = false;
            OpticalSwitchControlSequence op = new OpticalSwitchControlSequence(this);

            op.Show();
        }

        /// <summary>
        /// Handles the Changed event of the Group control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Group_Changed(object sender, RoutedEventArgs e)
        {
            if (type == null)
            {
                type = new ComboBox();
            }
            type.Items.Clear();
            if (group.SelectedIndex == 0)
            {

                Add_Items(type, new[]{ "CL 1X2 (N)", "CL 1X4 (N)", "CL 1X8 (N)", "LB 1X2 (N)",
                    "LB 1X4 (N)","LB 1X8 (N)", "CL 1X16 (Y)", "CL 1X2PM (N)", "CL 1X4PM (N)",
                    "CL 1X8MN (N)", "CL 1X8PM (N)", "LB 1X16 (N)", "LB 1X17 (N)", "1X12 (N)" });

                type.SelectedIndex = 6;


            }
            else if (group.SelectedIndex == 1)
            {
                Add_Items(type, new[]{"CL 2X1 (Y)", "CL 8X1 (Y)", "CL 8X1MN (Y)",
                    "LB 2X1 (Y)", "LB 4X1 (Y)", "LB 8X1 (Y)" });

                type.SelectedIndex = 0;
            }
            else if (group.SelectedIndex == 2)
            {

                Add_Items(type, new[]{"CL 4X4 (Y)", "LB 4X4 (Y)", "DD 4BIT (Y)",
                    "DD 5BIT (Y)", "DD 6BIT (Y)", "CL 2X2 (Y)","LB 2X2 (Y)"});

                type.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Adds String array elements into Combobox        
        /// </summary>
        /// <param name="c">The c.</param>
        /// <param name="l">The l.</param>
        private void Add_Items(ComboBox c, string[] l)
        {
            for (int i = 0; i < l.GetLength(0); i++)
            {
                c.Items.Add(l[i]);
            }
        }
    }
}

