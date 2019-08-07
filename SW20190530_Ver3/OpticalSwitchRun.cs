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
                Animate(sender, e);
                return;
            }
            else
            {
                notifier.ShowError("Test is already running", messageOptions);
            }
        }
        /// <summary>
        /// Iterates through flashing animation of the graph rows and diagram arrows
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void Animate(object sender, RoutedEventArgs e)
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
                    Flash();
                    ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value + progress));

                    // Keeps RunningRow from incrementing when stopped
                    if (running == true)
                    {
                        await Task.Delay(1000);
                        runningRow++;
                    }

                    //IF test is paused:
                    await Task.Run(() =>
                    {
                        //Waits until unpaused or stops running
                        while (pause == true && running == true)
                        {
                            ;
                        }

                    });
                    /* TODO:
                    * -Check if the following "if/else" statement is being used at all */
                    if (running == true)
                    {
                        UnFlash();
                        ProgressBar_ValueChanged(progressBar, new RoutedPropertyChangedEventArgs<double>(progressBar.Value, progressBar.Value));
                    }
                    else
                    {
                        break;
                    }
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
            else
            {
                notifier.ShowError("Test not Running", messageOptions);
            }
        }

        /// <summary>
        /// Handles the Stop event of the Button_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            if (pause != true)
            {
                runningRow++;
            }

            UnFlash();
            SwitchRunControlsIni();
            running = false;
            notifier.ShowInformation("Test Stopped", messageOptions);
        }
        /// <summary>
        /// Flashes visual diagrams.
        /// </summary>
        private void Flash()
        {
            RowHighlight();
            LoadIOConnection();
        }
        /// <summary>
        /// UnFlashes visual diagrams.
        /// </summary>
        private void UnFlash()
        {
            RowUnhiglight();
            UnloadIOConnection();
        }

        /// <summary>
        /// Highlights the runningRow
        /// </summary>
        private void RowHighlight()
        {
            flashing = true;
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
        private void RowUnhiglight()
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
