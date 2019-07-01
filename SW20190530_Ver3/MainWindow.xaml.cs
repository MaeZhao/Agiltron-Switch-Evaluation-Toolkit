using System.Windows;
using System.Windows.Input;
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

        }

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
        /// <summary>
        /// Handles the MouseDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

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

