   M S B A M L   `   `   `  ����J  FDiagramDesigner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null1clr-namespace:DiagramDesignerDiagramDesigner  5   	   C&clr-namespace:DiagramDesigner.ControlsDiagramDesigner.Controls  5   	       DiagramDesigner.Window1   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   +sclr-namespace:DiagramDesigner     5   	   4c&clr-namespace:DiagramDesigner.Controls     5   	     � WindowStartupLocation$  CenterScreen=�5   	    � Title$ Diagram Designer��5   	   $	��850��5   	   $
��1000��6    � 	Resources 5
         DesignerCanvasContextMenu.  (System.Windows.Input.ApplicationCommands   Paste m� Command m� Icon$   DiagramDesigner.DesignerCanvas   	SelectAll��%�   &	        �� 5   
   .@�5      m� $��Paste��6   # Z 6&    5      �� 5      $ ��Resources/Images/Paste.png��6   $��16��6@   5      m� 5      $��
Select All��6   # Z 6+   5      5      .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   ��   	MyToolbar#��[ 6   � 5   
   $��1��6   $��0,10,0,0q�6    � ColumnDefinitions 5      �� 5      $	��265 �6#   �� 5       .+�5#      �� $��0��6   $��0,0,5,0q�6)   .+�5$      9� $��
Flow Chart��6     FlowChartStencils#��[ 6/   	 9� 
IsExpanded	 . 6\   9� 5%      $��Shapes��6     ShapeStencils#��[ 6+   	 . 6T   5&      �� 5(      |�.  $��2��6-   
�������$��Stretch=�5)      $��Right=�61   �� 5+      $��Diagram��6   $��1��6(   $��3,0,0,0q�68   .��5,      �� $
��Auto=�6   $
��Auto=�5-      .��5.        -   5        
  Name$
 
MyDesigner��5.   8   |�.   WindowBackgroundBrush#��[ 5/      $��10q�50       � ContextMenu# [  51       � FocusVisualStyle 50   )   Z� 51      52      53      54      55      56      