   M S B A M L   `   `   `  ����J  FDiagramDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DiagramDesignerDiagramDesigner  5      C&clr-namespace:DiagramDesigner.ControlsDiagramDesigner.Controls  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   ConnectionContextMenu.   (System.Windows.Input.ApplicationCommands     Delete m� Command m� Icon$   DiagramDesigner.DesignerCanvas   BringForward   BringToFront   SendBackward   
SendToBack    DiagramDesigner.Connection �� 
TargetType �� 	Resources  Key	 �� Property
 �� Value T� X T� Y �� 
TargetType �� RadiusX �� RadiusY  SolidBorderBrush  ArrowSymbolBaseStyle �� BasedOn � ContextMenu � DataContext �� RelativeSource �� StrokeStartLineCap �� StrokeEndLineCap �� StrokeLineJoin J� SnapsToDevicePixels �� StrokeDashArray  ArrowGridStyle �� Left
 �� Top � LayoutTransform �� Angle J� BitmapEffect F� Color F� 	Direction  F� ShadowDepth! F� Softness" F� Opacity# �� Triggers$ �� Value% �� Binding& �� Path' �� 
TargetName 	 Arrow  Diamond�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      #sclr-namespace:DiagramDesigner  5      ,c&clr-namespace:DiagramDesigner.Controls  5      %�  &	        '  �    7  7  7  7   7  7  7  7  7  7  7  �� 5      .@�5   
   m� $��Delete��6   # Z  6#    5      �� 5	      $��Images/Delete.png��6   $��16��63   5      �� 5   
   m� 5   
   $��Order��6   .@�5      m� $��Bring Forward��6   # Z 6.    5      �� 5      $��Images/BringForward.png��6   $��16��6=   5      m� 5      $��Bring To Front��6   # Z 6/    5      �� 5      $��Images/BringToFront.png��6   $��16��6=   5      m� 5      $��Send Backward��6   # Z 6.    5      �� 5      $��Images/SendBackward.png��6   $��16��6=   5      m� 5      $��Send To Back��6   # Z 6-    5      �� 5      $��Images/SendToBack.png��6   $��16��6;   5!      5"      5#      �� 5%      "  6    5&   
   ���� 5(      ConnectionAdornerThumbStyle " n�68   .�5)      �� 	 � ��$
 12��6*   �� 5*      	 � ��$
 12��6+   �� 5+      	 �@J�SnapsToDevicePixels
 . 68   �� 5,      	 � z�
 5-      T� 5.      $ -6Q�6-   $ -6Q�64   50      �� 51      	 � ��
 52      �� 53      " n�6*   .R�54      �� ���	#AADCDCDC
������$��1��6M   $ 0��6a   $ 0��6m   55      57      58      �� 5:      ArrowGridStyle " �6+   .�5;      �� 	 � ��$
 10��6*   �� 5<      	 � ��$
 10��6+   �� 5=      	 � z�
 5>      T� 5?      $ -5Q�6-   $ -5Q�64   5A      5B      �� 5D      ArrowSymbolBaseStyle " F�61   .�5E      �� 	 � ��8
   6)   �� 5F      	 �@��Stretch$

 Fill=�6,   5G      �� 5I      	Arrow " F�6"   8  .�5J      �� 	 � ��"
 ��        � z  	= �     z 	6)   5K      �� 5M      Diamond " F�6$   8  .�5N      �� 	 � ��+
 ������    �    �����@KL     �    @KL 	6)   5O      5P      .�5Q   
   �� 	 �@J�SnapsToDevicePixels
 . 60   �� 5R   
   	 � ��
 5S      �� 5T      "  6"   .R�5U      �� 8   6   ��  �� *TemplatedParent+.+�5W      F� $��PART_ConnectionPath��6   $��2��5X      8�� $ Round=�5Z      $ Round=�5[      $ Round=�5\       .  5]      �� *StrokeDashArray+��5_      �� *PathGeometry+5`      � 5a      8��  5b      �� *AnchorPositionSource.X+ 5c      �� *AnchorPositionSource.Y+.+�5d      F� $��PART_SourceAnchorPath��6#    5e      �� 5f   "    62   �� *AnchorAngleSource+5h      � 5i      8��  5j      �� *AnchorPositionSink.X+ 5k      �� *AnchorPositionSink.Y+.+�5l      F� $��PART_SinkAnchorPath��6#    5m      �� 5n   "    62   �� *AnchorAngleSink+5p       5|      F� 5}      $
 Gray��65   $	 315Q�6B   $  10Q�6R   $! 0Q�6c   $	" 0.1Q�6p   5      # 5�      �� 5�      $$ Arrow��6&   % 64   ��  �� *Self+$& SourceArrowSymbol�.a�5�      �� $' PART_SourceAnchorPath��6%   8
  	 � ��6Y   5�      �� 5�      $$ Diamond��6&   % 66   ��  �� *Self+$& SourceArrowSymbol�.a�5�      �� $' PART_SourceAnchorPath��6%   8
  	 � ��6Y   5�      �� 5�      $$ Arrow��6&   % 64   ��  �� *Self+$& SinkArrowSymbol�.a�5�      �� $' PART_SinkAnchorPath��6%   8
 	 	 � ��6W   5�      �� 5�      $$ Diamond��6&   % 66   ��  �� *Self+$& SinkArrowSymbol�.a�5�      �� $' PART_SinkAnchorPath��6%   8
 
 	 � ��6W   5�      5�      5�      5�      5�      