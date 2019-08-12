//using DiagramDesigner;
//using Microsoft.Win32;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.Data;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Interop;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Shapes;
//using ToastNotifications;
//using ToastNotifications.Lifetime;
//using ToastNotifications.Messages;
//using ToastNotifications.Messages.Core;
//using ToastNotifications.Position;
//using Path = System.Windows.Shapes.Path;
//namespace SW20190530_Ver3
//{
//    /* Loads Switch Diagram components:
//     * input/output node formation + arrow generation/degeneration
//     */
//    partial class OpticalSwitchControlSequence
//    {
//        /// <summary>
//        /// Structure used to map out the positions in our input/output diagram
//        /// </summary>
//        public struct Radius
//        {
//            private double space;
//            private double xOffset;
//            private double yOffset;
//            private double diameter;
//            private double[] centerXY;
//            private double height;

//            public double Space { get => space; }
//            public double XOffset { get => xOffset; }
//            public double YOffset { get => yOffset; }
//            public double Diameter { get => diameter; }
//            public double[] CenterXY { get => centerXY; }
//            public double Height { get => height; }
//            /// <summary>
//            /// Initializes a new instance of the <see cref="Radius"/> struct.
//            /// </summary>
//            /// <param name="outNum">The out number.</param>
//            /// <param name="h">The h.</param>
//            /// <param name="offX">The off x.</param>
//            /// <param name="offY">The off y.</param>
//            /// <param name="s">The s.</param>
//            public Radius(int outNum, double h, double offX, double offY, double s)
//            {
//                height = h;
//                space = s;
//                xOffset = offX;
//                yOffset = offY + h;
//                diameter = (height + space) * outNum;
//                centerXY = new double[2];
//                centerXY[0] = xOffset;
//                centerXY[1] = yOffset + diameter + height / 2;
//            }
//            /// <summary>
//            /// Initializes a new instance of the <see cref="Radius"/> struct.
//            /// </summary>
//            /// <param name="outNum">The out number.</param>
//            /// <param name="h">The h.</param>
//            public Radius(int outNum, double h)
//            {
//                height = h;
//                space = 20;
//                diameter = (height + space) * outNum * 1.5;
//                yOffset = -(height + space) * (outNum) * 0.25;
//                xOffset = 30;
//                centerXY = new double[2];
//                centerXY[0] = xOffset;
//                centerXY[1] = diameter / 2 + 10 + height / 2.0 + yOffset;
//            }
//        }

//        /// <summary>
//        /// Places index and nodes onto diagram canvas
//        /// </summary>
//        private void SwitchDiagramCircleIni()
//        {
//            if (inChannelNum == 1)
//            {
//                double height = 45;
//                double width = 90;
//                double totalH = switchDiagram.ActualHeight;
//                double totalW = diagramBounds.ActualWidth;
//                Radius radius = new Radius(outSwitchNum, height);
//                //Places input Node
//                PlaceNewNode(true, height, width, 1, "#62C1AF", radius.CenterXY);
//                //Finds outputnode indexes and places output Nodes
//                Dictionary<int, double[]> outNodeIndex = OutputNodeIndexGenerator(radius);
//                for (int i = 1; i <= outNodeIndex.Count; i++)
//                {
//                    PlaceNewNode(false, height, width, i, "#F3A414", outNodeIndex[i]);
//                }
//            }
//            else
//            {
//                SwitchDiagramIni();
//            }
//            switchDiagram.UpdateLayout();
//            /*TODO: make this part of the style/template?
//             */
//            RemoveConnectors("Left", inp);
//            RemoveConnectors("Right", oup);
//        }

//        /// <summary>
//        /// Generates output node indexes.
//        /// </summary>
//        /// <param name="rad">The RAD.</param>
//        /// <returns></returns>
//        private Dictionary<int, double[]> OutputNodeIndexGenerator(Radius rad)
//        {
//            Dictionary<int, double[]> outNodeIndex = new Dictionary<int, double[]>();
//            double radLength = rad.Diameter / 2.0;
//            double NodeHeight = rad.Height;
//            double NodeSpace = rad.Space;

//            //Add first node
//            double yNode = 0.0;
//            //outNodeIndex.Add(1, new double[] { rad.CenterXY[0], yNode });
//            for (int NodeID = 1; NodeID <= outSwitchNum; NodeID++)
//            {
//                if (NodeID == 1)
//                {
//                    yNode = rad.CenterXY[1] - (NodeHeight + NodeSpace) * outSwitchNum / 2.0;
//                }
//                else
//                {
//                    yNode += NodeHeight + NodeSpace;
//                }

//                if (yNode <= rad.CenterXY[1])
//                {
//                    double NodeAngle = Math.Acos((rad.CenterXY[1] - yNode) / radLength);
//                    outNodeIndex.Add(NodeID, new double[] { radLength * Math.Sin(NodeAngle) + rad.CenterXY[0], yNode });
//                }
//                else if (yNode > rad.CenterXY[1])
//                {
//                    double NodeAngle = Math.Asin((yNode - rad.CenterXY[1]) / radLength);
//                    outNodeIndex.Add(NodeID, new double[] { (radLength * Math.Cos(NodeAngle)) + rad.CenterXY[0], yNode });
//                }
//            }
//            return outNodeIndex;
//        }
//        /// <summary>
//        /// Places the new node on diagram canvas.
//        /// </summary>
//        /// <param name="input">if set to <c>true</c> [input].</param>
//        /// <param name="height">The height.</param>
//        /// <param name="width">The width.</param>
//        /// <param name="nodeID">The node identifier.</param>
//        /// <param name="color">The color.</param>
//        /// <param name="xyPosition">The xy position.</param>
//        private void PlaceNewNode(Boolean input, double height, double width, int nodeID, string color, double[] xyPosition)
//        {
//            double xPosition = xyPosition[0];
//            double yPosition = xyPosition[1];
//            string text = "";
//            DesignerItem Node = new DesignerItem
//            {
//                Height = height,
//                Width = width,
//                Visibility = Visibility.Visible,
//                OverridesDefaultStyle = false,
//            };

//            Grid NodeContent = new Grid
//            {
//                IsHitTestVisible = false,
//                Focusable = false,
//            };

//            Path NodeShape = new System.Windows.Shapes.Path
//            {
//                Style = Application.Current.Resources["Start"] as Style,
//                Fill = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
//                Visibility = Visibility.Visible,
//                Height = height,
//                Width = width,
//                Stroke = Brushes.Transparent,
//            };
//            if (input == true)
//            {
//                text = "input #" + nodeID;
//            }
//            else
//            {
//                text = "output #" + nodeID;
//            }

//            TextBlock name = new TextBlock
//            {
//                Text = text,
//                HorizontalAlignment = HorizontalAlignment.Center,
//                VerticalAlignment = VerticalAlignment.Center,
//                FontWeight = FontWeights.DemiBold,
//                FontSize = 12,
//                Foreground = Brushes.Black,
//                MaxHeight = height,
//                MaxWidth = width - 10,
//            };

//            NodeContent.Children.Add(NodeShape);
//            NodeContent.Children.Add(name);
//            Node.Content = NodeContent;

//            Canvas.SetTop(Node, yPosition);
//            Canvas.SetLeft(Node, xPosition);
//            if (input == true)
//            {
//                inp.Add(Node);
//            }
//            else
//            {
//                oup.Add(Node);
//            }
//            switchDiagram.Children.Add(Node);
//        }
//        /// <summary>
//        /// Initializeds fields and node formation in Switch Diagram
//        /// </summary>
//        private void SwitchDiagramIni()
//        {
//            //double space = switchDiagram.ActualHeight;
//            double totalW = diagramBounds.ActualWidth;
//            double height = 60;
//            double width = 80;
//            //double yPositionInp = (space / 2) - (numChannel / 2.0 * 50.0);
//            //double yPositionOutp = (space / 2) - (numOut / 2.0 * 50.0);
//            double yPositionInp = 40;
//            double yPositionOutp = 40;

//            for (int i = 1; i <= inChannelNum; i++)
//            {
//                DesignerItem Node = new DesignerItem
//                {
//                    Height = height,
//                    Width = width,
//                    Visibility = Visibility.Visible,
//                    OverridesDefaultStyle = false,
//                    Name = "in_" + i,
//                };

//                Grid NodeContent = new Grid
//                {
//                    IsHitTestVisible = false,
//                    Focusable = false,
//                };
//                Path NodeShape = new System.Windows.Shapes.Path
//                {
//                    Style = Application.Current.Resources["Card"] as Style,
//                    Fill = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#62C1AF")),
//                    Visibility = Visibility.Visible,
//                    Height = height,
//                    Width = width,
//                    Stroke = Brushes.Transparent,
//                };
//                TextBlock name = new TextBlock
//                {
//                    Text = "in #" + i,
//                    HorizontalAlignment = HorizontalAlignment.Center,
//                    VerticalAlignment = VerticalAlignment.Center,
//                    FontWeight = FontWeights.DemiBold,
//                    FontSize = 15,
//                    Foreground = Brushes.Black,
//                    MaxHeight = 50,
//                    MaxWidth = 80,

//                };
//                NodeContent.Children.Add(NodeShape);
//                NodeContent.Children.Add(name);
//                Node.Content = NodeContent;

//                Canvas.SetTop(Node, yPositionInp);
//                Canvas.SetLeft(Node, 5);
//                yPositionInp += 80;
//                inp.Add(Node);
//                switchDiagram.Children.Add(Node);
//            }

//            for (int i = 1; i <= outSwitchNum; i++)
//            {
//                DesignerItem Node = new DesignerItem
//                {
//                    Height = height,
//                    Width = width,
//                    Visibility = Visibility.Visible,
//                    OverridesDefaultStyle = false,
//                    Name = "out_" + i,
//                };

//                Grid NodeContent = new Grid
//                {
//                    IsHitTestVisible = false,
//                    Focusable = false,
//                };
//                Path NodeShape = new System.Windows.Shapes.Path
//                {
//                    Style = Application.Current.Resources["Card"] as Style,
//                    Fill = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#F3A414")),
//                    Visibility = Visibility.Visible,
//                    Height = height,
//                    Width = width,
//                    Stroke = Brushes.Transparent,
//                };
//                TextBlock name = new TextBlock
//                {
//                    Text = "out #" + i,
//                    HorizontalAlignment = HorizontalAlignment.Center,
//                    VerticalAlignment = VerticalAlignment.Center,
//                    FontWeight = FontWeights.DemiBold,
//                    FontSize = 15,
//                    Foreground = Brushes.Black,
//                    MaxHeight = 50,
//                    MaxWidth = 80,

//                };
//                NodeContent.Children.Add(NodeShape);
//                NodeContent.Children.Add(name);
//                Node.Content = NodeContent;

//                Canvas.SetTop(Node, yPositionOutp);
//                switchGrid.UpdateLayout();
//                Canvas.SetLeft(Node, totalW - 700);
//                yPositionOutp += 80;
//                oup.Add(Node);
//                switchDiagram.Children.Add(Node);
//            }
//            switchDiagram.UpdateLayout();
//            RemoveConnectors("Left", inp);
//            RemoveConnectors("Right", oup);
//        }

//        /// <summary>
//        /// Removes the specified connector square (via connectorKey) from all nodes in item list.
//        /// </summary>
//        /// <param name="connectorKey">The connector key.</param>
//        /// <param name="items">The items.</param>
//        private void RemoveConnectors(String connectorKey, List<DesignerItem> items)
//        {
//            foreach (DesignerItem i in items)
//            {
//                i.RemoveConnector(connectorKey);
//            }
//        }

//        /// <summary>
//        /// Draws Arrow connection between input and output nodes
//        /// </summary>
//        /// <param name="inNode">The input node.</param>
//        /// <param name="outNode">The output node.</param>
//        /// <returns>Connected arrow between input and output nodes</returns>
//        private Connection Draw_Arrow(DesignerItem inNode, DesignerItem outNode)
//        {
//            Connector pR = inNode.GetConnector("Right");
//            Connector cL = outNode.GetConnector("Left");
//            ConnectorAdorner sinkAdorner = new ConnectorAdorner(switchDiagram, cL);

//            Connection connectedArrow = new Connection(pR, cL);

//            switchDiagram.Children.Add(connectedArrow);
//            switchDiagram.UpdateLayout();
//            return connectedArrow;
//        }

//    }
//}
