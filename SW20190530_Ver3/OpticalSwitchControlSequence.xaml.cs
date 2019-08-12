using DiagramDesigner;
using System;
using System.Collections.Generic;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

/* Core partial class for OpticalSwitchControlSequence:
 * Initializes all OpticalSwitchControlSequence window methods and controls.
 */

namespace SW20190530_Ver3
{
    /// <summary>
    /// Interaction logic for OpticalSwitchControlSequence.xaml.
    /// Contains code for grid on/off table, MenuItem controls (incomplete) as well as loads diagram formation.
    /// </summary>
    /// <seealso cref="SW20190530_Ver3.WindowUIComponents" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class OpticalSwitchControlSequence : WindowUIComponents
    {
        //UI fields
        private Boolean offline;
        private Notifier notifier; //From ToastNotifications v2 nuget pkg
        private ToastNotifications.Core.MessageOptions messageOptions;  //From ToastNotifications v2 nuget pkg
        //switchGrid specs
        private int inChannelNum, outSwitchNum;
        //running/pausing button controls + diagram animations
        private bool running;
        private bool pause;
        private bool flashing;
        private int runningRow; //Current highlighted row
        private List<int[]> activeIOpairs = new List<int[]>(); //List of active inChannel and outSwitch pairs
        Dictionary<int, Connection> runningUIconnection = new Dictionary<int, Connection>(); //Key value-->(connectionID, Arrow)
        //switchDiagram Specs
        List<DesignerItem> inp = new List<DesignerItem>(); //List of input Nodes
        List<DesignerItem> oup = new List<DesignerItem>(); //List of output Nodes


        #region REGION: Initialization of OpticalSwitchControlSequence
        /// <summary>
        /// Initializes a new instance of.
        /// </summary>
        /// <param name="input">The input.</param>
        public OpticalSwitchControlSequence(MainWin input)
        {
            InitializeComponent();
            //Initializes general notifier settings
            Application.Current.MainWindow = this;
            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 0,
                    offsetY: WindowBar.ActualHeight);
                cfg.DisplayOptions.TopMost = true;
                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(10),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));
                cfg.Dispatcher = Application.Current.Dispatcher;
            });
            messageOptions = new ToastNotifications.Core.MessageOptions
            {
                ShowCloseButton = true, // set the option to show or hide notification close button
                FreezeOnMouseEnter = true, // set the option to prevent notification dissapear automatically if user move cursor on it
                NotificationClickAction = n => // set the callback for notification click event
                {
                    n.Close(); // call Close method to remove notification
                },
            };

            // Offline notification display
            offline = input.offlineButton.IsChecked.Value;
            input.Close();
            this.Show();
            if (offline)
            {
                notifier.ShowInformation("Running in Offline Mode", messageOptions);
            }

            //Initializes/finds switchGrid specs 
            SwitchBoardFieldIni(input);
            //Initializes switch Diagram
            //SwitchDiagramCircleIni();
            //Initializes switch test run specs
            SwitchRunControlsIni();


            //TODO load presets
            //TODO do soemthing with the port
            this.MaxWidth = GetWidth();
            this.MaxHeight = SystemParameters.WorkArea.Height;
        }

        private static double GetWidth()
        {
            return SystemParameters.WorkArea.Width;
        }
        #endregion
    }

}
