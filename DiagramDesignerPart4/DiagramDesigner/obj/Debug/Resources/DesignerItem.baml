   M S B A M L   `   `   `  ����O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  5      J FDiagramDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DiagramDesignerDiagramDesigner 5      C&clr-namespace:DiagramDesigner.ControlsDiagramDesigner.Controls 5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   DesignerItemContextMenu.   (System.Windows.Input.ApplicationCommands
     Cut m� Command m� Icon    Copy    Paste    Delete$  DiagramDesigner.DesignerCanvas   Group   Ungroup   BringForward	   BringToFront
   SendBackward   
SendToBack  DiagramDesigner.Connector �� 
TargetType �� Property �� Value �� 
TargetType  ConnectorDecoratorTemplate  Name   Orientation  ResizeDecoratorTemplate J� Opacity J� SnapsToDevicePixels*  $DiagramDesigner.Controls.ResizeThumb � Cursor(  "DiagramDesigner.Controls.DragThumb"  DiagramDesigner.DesignerItem � ContextMenu � DataContext �� RelativeSource  Name ��Name ��Name �� Triggers �� Value �� Binding �� Path �� 
TargetName  ^� 
Conditions! �� Binding" �� Value# �� Empty$ P� Property% P� Value�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    :x,http://schemas.microsoft.com/winfx/2006/xaml     5      .sys&clr-namespace:System;assembly=mscorlib  5      #sclr-namespace:DiagramDesigner  5      ,c&clr-namespace:DiagramDesigner.Controls  5      %v  &	        '  u    &	     &	 �    '  p    '      7   7  7  �� 5      .@�5   
   m� $	��Cut��6   # Z  6     5	      �� 5
      $��Images/Cut.png��6   $��16��60   5      m� 5   
   $
��Copy��6   # Z 6!    5      �� 5      $��Images/Copy.png��6   $��16��61   5      m� 5   
   $��Paste��6   # Z 6"    5      �� 5      $��Images/Paste.png��6   $��16��62   5      m� 5   
   $��Delete��6   # Z 6#    5      �� 5      $��Images/Delete.png��6   $��16��63   5      �� 5   
   m� 5   
   $��Grouping��6   .@�5      m� $��Group��6   # Z 6&    5      �� 5       $��Images/Group.png��6   $��16��66   5"      m� 5#      $��Ungroup��6   # Z 6(    5$      �� 5%      $��Images/Ungroup.png��6   $��16��68   5'      5(      m� 5)   
   $��Order��6   .@�5*      m� $��Bring Forward��6   # Z 6.    5+      �� 5,      $��Images/BringForward.png��6   $��16��6=   5.      m� 5/      $��Bring To Front��6   # Z	 6/    50      �� 51      $��Images/BringToFront.png��6   $��16��6=   53      m� 54      $��Send Backward��6   # Z
 6.    55      �� 56      $��Images/SendBackward.png��6   $��16��6=   58      m� 59      $��Send To Back��6   # Z 6-    5:      �� 5;      $��Images/SendToBack.png��6   $��16��6;   5=      5>      5?      �� 5B      "  6   .�5C   
   ��  � ��$ 8��6"   �� 5D   
    � ��$ 8��6#   �� 5E   
    �@�Cursor$ Cross��6#   �� 5F   
    �@J�SnapsToDevicePixels . 60   �� 5G   
    � �� 5H      �� 5I      "  6"   .R�5J      � .+�5L      �� 
������ $��-2q�67   �� 5M      
�������$��1��64   ���	#AA0000806H   5N      5O      5Q      5R      �� 5U      " ��69   .R�5V   
   � $��-5q�6   .+�5W        $
 Left��6   $
 Left=�6(   $��Center=�6;   $
��Left=�6V     5X      $	 Top��6   $	 Top=�6'   $	��Top=�69   $��Center=�6Q     5Y      $ Right��6   $ Right=�6)   $��Center=�6=   $��Right=�6X     5Z      $ Bottom��6   $ Bottom=�6*   $��Bottom=�6?   $��Center=�6Z   5[      5\      �� 5_      " ��66   .R�5`   
   � $	 0.7Q�6    . .+�5a        $��3��6   $ SizeNS��6'   $��0 -4 0 0q�67   $	��Top=�5b      $��Stretch=�6.     5c      $��3��6   $ SizeWE��6&   $��-4 0 0 0q�66   $��Stretch=�5d      $
��Left=�62     5e      $��3��6   $ SizeWE��6&   $��0 0 -4 0q�66   $��Stretch=�5f      $��Right=�62     5g      $��3��6   $ SizeNS��6'   $��0 0 0 -4q�67   $��Bottom=�5h      $��Stretch=�61     5i      $��7��6   $��7��6&   $ SizeNWSE��61   $��	-6 -6 0 0q�6C   $	��Top=�5j      $
��Left=�6.     5k      $��7��6   $��7��6&   $ SizeNESW��61   $��	0 -6 -6 0q�6C   $	��Top=�5l      $��Right=�6.     5m      $��7��6   $��7��6&   $ SizeNESW��61   $��	-6 0 0 -6q�6C   $��Bottom=�5n      $
��Left=�61     5o      $��7��6   $��7��6&   $ SizeNWSE��61   $��	0 0 -6 -6q�6C   $��Bottom=�5p      $��Right=�61   5q      5r      �� 5u      "  6   .�5v   
   ��  � �� 5w      �� 5x      "  6"   .R�5y      �� 
������ 6    5z      5|      5}      �� 5�      "  6   .�5�   
   ��  � ��$ 10��6%   �� 5�   
    � ��$ 10��6&   �� 5�   
    �@J�SnapsToDevicePixels . 60   �� 5�   
    � �� 5�      �� 5�      "  6"   .R�5�      � 8    6   ��  �� *TemplatedParent+.+�5�        $ PART_DragThumb��6&   $ SizeAll��5�   &   �� 5�      $ PART_ResizeDecorator��6"   $y�	Collapsed=�5�   "   8�� 5�   "   �� 5�      $ PART_ContentPresenter��6+   $��Stretch=�5�   +   $��Stretch=�5�   +   #��z��5�   +   #��z��5�   +   �� 5�      $ PART_ConnectorDecorator��6"   $y�Hidden=�5�   "   8�� 5�   "   5�       5�      �� 5�      $
 True��6&    63   ��  �� *Self+$ IsGroup�.a�5�      �� $ PART_DragThumb��6%    � y�$ 	Collapsed=�6W   5�      ^� 5�        5�      �� 5�   "   ! 69   ��  �� *Self+$ 
IsSelected�" True6,   69   �� 5�   "   #" Z# 6,   ! 6N   ��  �� *Self+$ ParentID�.0�5�      �� $ PART_ResizeDecorator��6%    � y�$ Visible=�6]   5�      P� 5�      $ �@J�IsMouseOver% . . �5�      �� $ PART_ConnectorDecorator��6%    � y�$ Visible=�6`   5�      �� 5�      $
 True��6&    63   ��  �� *Self+$ IsDragConnectionOver�.a�5�      �� $ PART_ConnectorDecorator��6%    � y�$ Visible=�6`   5�      �� 5�      $
 True��6&    63   ��  �� *Self+$ IsGroup�.a�5�      �� $ PART_ConnectorDecorator��6%    � y�$ Hidden=�6`   5�      5�      5�      5�      5�      