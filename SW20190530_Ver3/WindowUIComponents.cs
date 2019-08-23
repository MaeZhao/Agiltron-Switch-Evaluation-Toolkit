using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static SW20190530_Ver3.WindowUIComponentsResources;

namespace SW20190530_Ver3
{
    /// <summary>
    /// Initializations shared by all custom Windows used in this app
    /// </summary>
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

        #region REGION: Default pixel offset in window maximization/minimization
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

    }

}

