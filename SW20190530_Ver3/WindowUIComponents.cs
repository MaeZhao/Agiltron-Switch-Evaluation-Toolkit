/* Basic Windows functions shared by all windows-->Create custom ControlTemplate 
 */
using System;

using System.Linq;
using System.Runtime.InteropServices;

using System.Windows;

using System.Windows.Input;
using System.Windows.Interop;

namespace SW20190530_Ver3
{
    public partial class WindowUIComponents : Window
    {
        public WindowUIComponents()
        {
            this.Style = FindResource("DefaultWindowStyle") as Style;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.StateChanged += Win_StateChanged;
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

        public void Win_StateChanged(object sender, EventArgs e)
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
        public void Window_Loaded(object sender, RoutedEventArgs e)
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
        public void Button_Click_Exit(object sender, RoutedEventArgs e) => Application.Current.Shutdown(99);
        /// <summary>
        /// Handles the Exit event of the Button_Min control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public void Button_Min_Exit(object sender, RoutedEventArgs e) => SystemCommands.MinimizeWindow(this);
        /// <summary>
        /// Handles the MouseDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        public void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RECT rect = GetWindowRectangle(this);
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
}
