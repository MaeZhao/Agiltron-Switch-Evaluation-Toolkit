using System;

using System.Linq;
using System.Runtime.InteropServices;

using System.Windows;

using System.Windows.Input;
using System.Windows.Interop;

namespace SW20190530_Ver3
{
    public abstract class WindowUIComponents : Window
    {
        public WindowUIComponents()
        {
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
            Window wind = (Window)sender;
            RECT rect = GetWindowRectangle(wind);
            if (wind.WindowState == WindowState.Maximized)
            {
                Window_Loaded(sender, new RoutedEventArgs()); // Inefficient: resets the WindowsState.Normal settings

                rect.Left = (int)SystemParameters.WorkArea.Left;
                rect.Top = (int)SystemParameters.WorkArea.Top;
                rect.Right = (int)(rect.Left - wind.MaxWidth);
                rect.Bottom = (int)(rect.Top - wind.MaxHeight);
                wind.WindowState = WindowState.Normal;
            }

        }

        /// <summary>
        /// Handles the Loaded event of the Initial Window State control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        public abstract void Window_Loaded(object sender, RoutedEventArgs e);
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
        public abstract void Window_MouseDown(object sender, MouseButtonEventArgs e);
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

