using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ToastNotifications.Messages;

/* Switch Test Running Controls:
 * initiates events that trigger button and arrow flashing/unflashing
 */
namespace SW20190530_Ver3
{
    partial class OpticalSwitchControlSequence
    {
        /// <summary>
        /// Initializes fields used for run/pause/stop/loop abilities during testing
        /// </summary>
        private void SwitchRunControlsIni()
        {
            runningRow = 2;
            running = false;
            pause = false;
            flashing = false;
            ProgressBar_ValueChanged(new object(), new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
        }

        /// <summary>
        /// Handles the ValueChanged event of the ProgressBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{System.Double}"/> instance containing the event data.</param>
        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (running == false)
            {
                progressBar.Value = 0;
            }
            else if (running == true && pause == false)
            {
                progressBar.Value = e.NewValue;
                progressBar.Foreground = Brushes.Green;
            }
            else
            {
                progressBar.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// Handles the Run event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Run(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("runningRow val : " + runningRow);
            //TODO
            if (running == false)
            {
                SwitchRunControlsIni();
                running = true;
                FlashingIni(sender, e);
                return;
            }
            else
            {
                notifier.ShowError("Test is already running", messageOptions);
            }
        }
        private async void FlashingIni(object sender, RoutedEventArgs e)
        {
            int rowTotal = switchGrid.RowDefinitions.Count;

            double progress = (100.0 * (double)(runningRow - 1) / (double)(rowTotal - 2));
            // Test is running:
            if (running == true && pause == false)
            {
                if (runningRow == 2)
                {
                    progressBar.Value = 0;
                    ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value + progress));
                }
                int rowLeft = rowTotal - 1;
                while (runningRow <= rowLeft && running == true)
                {
                    //Console.WriteLine("running row: " + runningRow + " | ruwLeft: " + rowLeft);
                    BarFlash();
                    LoadIOConnection(runningRow);
                    ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value + progress));

                    // Keeps RunningRow from incrementing when stopped
                    if (running == true)
                    {
                        await Task.Delay(1000);
                        runningRow++;
                    }
                    flashing = true;
                    //Test is paused:
                    await Task.Run(() =>
                    {
                        //if (pause) Console.WriteLine("paused line: " + runningRow);
                        while (pause == true && running == true)
                        {
                            ;
                        }
                    });

                    BarUnFlash();
                    UnloadIOConnection(runningRow - 1);
                    ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
                }
            }
            //Test is finished:
            if (runningRow == rowTotal)
            {
                Button_Click_Stop(sender, e);
                notifier.ShowSuccess("Test Completed", messageOptions);
            }

        }

        /// <summary>
        /// Handles the Pause event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Pause(object sender, RoutedEventArgs e)
        {
            pause = !pause;
            if (pause == true && running == true)
            {
                notifier.ShowInformation("Test Paused", messageOptions);
            }
        }

        /// <summary>
        /// Handles the Stop event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine("flashing : " + flashing);
            //Console.WriteLine("stopped line : " + runningRow);

            if (pause != true)
            {
                runningRow++;
            }

            BarUnFlash();


            running = false;
            //ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
            notifier.ShowWarning("Test Stopped", messageOptions);
        }

        /// <summary>
        /// Highlights the row
        /// </summary>
        private void BarFlash()
        {
            foreach (UIElement child in switchGrid.Children)
            {
                if (Grid.GetRow(child) == runningRow)
                {
                    if (child.GetType() == new Rectangle().GetType())
                    {
                        child.IsEnabled = !child.IsEnabled;
                    }
                    else if (child.GetType() == new Button().GetType())
                    {
                        child.Focusable = !child.Focusable;
                    }
                }
            }

        }
        /// <summary>
        /// Removes the highlight on the previously highlighted row 
        /// </summary>
        private void BarUnFlash()
        {
            //Console.WriteLine("unflashed row : " + (runningRow - 1));
            flashing = false;
            int r = runningRow - 1;
            foreach (UIElement child in switchGrid.Children)
            {
                if (Grid.GetRow(child) == r)
                {
                    if (child.GetType() == new Rectangle().GetType())
                    {
                        child.IsEnabled = !child.IsEnabled;
                    }
                    else if (child.GetType() == new Button().GetType())
                    {
                        child.Focusable = !child.Focusable;
                    }
                }
            }
        }
    }
}
