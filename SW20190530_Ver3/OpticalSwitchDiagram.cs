using DiagramDesigner;
using Microsoft.Win32;
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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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
using Path = System.Windows.Shapes.Path;
namespace SW20190530_Ver3
{
    /* Loads Switch Diagram components:
     * input/output node formation + arrow generation/degeneration
     */
    partial class OpticalSwitchControlSequence
    {
        /// <summary>
        /// Initializeds feilds and node formation in Switch Diagram
        /// </summary>
        private void SwitchDiagramIni()
        {
            double space = switchDiagram.ActualHeight;
            double spaceW = diagramBounds.ActualWidth;
            double height = 60;
            double width = 80;
            //double yPositionInp = (space / 2) - (numChannel / 2.0 * 50.0);
            //double yPositionOutp = (space / 2) - (numOut / 2.0 * 50.0);
            double yPositionInp = 40;
            double yPositionOutp = 40;

            for (int i = 1; i <= inChannelNum; i++)
            {
                DesignerItem Node = new DesignerItem
                {
                    Height = height,
                    Width = width,
                    Visibility = Visibility.Visible,
                    OverridesDefaultStyle = false,
                    Name = "in_" + i,
                };

                Grid NodeContent = new Grid
                {
                    IsHitTestVisible = false,
                    Focusable = false,
                };
                Path NodeShape = new System.Windows.Shapes.Path
                {
                    Style = Application.Current.Resources["Card"] as Style,
                    Fill = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#62C1AF")),
                    Visibility = Visibility.Visible,
                    Height = height,
                    Width = width,
                    Stroke = Brushes.Transparent,
                };
                TextBlock name = new TextBlock
                {
                    Text = "in #" + i,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.DemiBold,
                    FontSize = 15,
                    Foreground = Brushes.Black,
                    MaxHeight = 50,
                    MaxWidth = 80,

                };
                NodeContent.Children.Add(NodeShape);
                NodeContent.Children.Add(name);
                Node.Content = NodeContent;

                Canvas.SetTop(Node, yPositionInp);
                Canvas.SetLeft(Node, 5);
                yPositionInp += 80;
                inp.Add(Node);
                switchDiagram.Children.Add(Node);
            }

            for (int i = 1; i <= outSwitchNum; i++)
            {
                DesignerItem Node = new DesignerItem
                {
                    Height = height,
                    Width = width,
                    Visibility = Visibility.Visible,
                    OverridesDefaultStyle = false,
                    Name = "out_" + i,
                };

                Grid NodeContent = new Grid
                {
                    IsHitTestVisible = false,
                    Focusable = false,
                };
                Path NodeShape = new System.Windows.Shapes.Path
                {
                    Style = Application.Current.Resources["Card"] as Style,
                    Fill = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3A414")),
                    Visibility = Visibility.Visible,
                    Height = height,
                    Width = width,
                    Stroke = Brushes.Transparent,
                };
                TextBlock name = new TextBlock
                {
                    Text = "out #" + i,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontWeight = FontWeights.DemiBold,
                    FontSize = 15,
                    Foreground = Brushes.Black,
                    MaxHeight = 50,
                    MaxWidth = 80,

                };
                NodeContent.Children.Add(NodeShape);
                NodeContent.Children.Add(name);
                Node.Content = NodeContent;

                Canvas.SetTop(Node, yPositionOutp);
                switchGrid.UpdateLayout();
                Canvas.SetLeft(Node, spaceW - 700);
                yPositionOutp += 80;
                oup.Add(Node);
                switchDiagram.Children.Add(Node);
            }
            switchDiagram.UpdateLayout();
            RemoveConnectors("Left", inp);
            RemoveConnectors("Right", oup);
        }

        /// <summary>
        /// Removes the specified connector square (via connectorKey) from all nodes in item list.
        /// </summary>
        /// <param name="connectorKey">The connector key.</param>
        /// <param name="items">The items.</param>
        private void RemoveConnectors(String connectorKey, List<DesignerItem> items)
        {
            foreach (DesignerItem i in items)
            {
                i.RemoveConnector(connectorKey);
            }
        }

        /// <summary>
        /// Draws Arrow connection between input and output nodes
        /// </summary>
        /// <param name="inNode">The input node.</param>
        /// <param name="outNode">The output node.</param>
        /// <returns>Connected arrow between input and output nodes</returns>
        private Connection Draw_Arrow(DesignerItem inNode, DesignerItem outNode)
        {
            Connector pR = inNode.GetConnector("Right");
            Connector cL = outNode.GetConnector("Left");
            ConnectorAdorner sinkAdorner = new ConnectorAdorner(switchDiagram, cL);

            Connection connectedArrow = new Connection(pR, cL);

            switchDiagram.Children.Add(connectedArrow);
            switchDiagram.UpdateLayout();
            return connectedArrow;
        }

    }
}
