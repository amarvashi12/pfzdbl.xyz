   M S B A M L   `   `   `  ����H  DWowonderLight, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null1clr-namespace:WpfApplication2WpfApplication2  5   	   "    WpfApplication2.SplashScreen   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:WpfApplication2     5   	   J FWpfAnimatedGif, Version=1.4.14.0, Culture=neutral, PublicKeyToken=null,gif"http://wpfanimatedgif.codeplex.com  5   	   -   5          � Title$  Wowonder��5	      $	��250��6   $	��470��6%   
������  � AllowsTransparency .  � WindowStartupLocation$ CenterScreen=�6f    � 
ResizeMode$ NoResize=�6�    � WindowStyle$
 None=�6�    � 	Resources 5         
Storyboard g� 	BeginTime g� Duration �� 
TargetName	 �� TargetProperty
 W� From	 W� To g� 
SpeedRatio g� RepeatBehavior��%'  &	        �� 5   
   $ 00:00:00.000X�6(   $ 00:00:10.000D�6A   .��5      W� $ Logowebsite��6   $9	 3(UIElement.RenderTransform).(RotateTransform.Angle)�5      $
 0X�5      $	 360X�6'   $ 00:00:01.000X�60   $ 00:00:50.000D�6I   $ 40Q�6b   $ 4��6r   5      5      .��5      � $��10q�6    J� Effect 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5         
BlurRadius$ 15Q�6      Color$
 #000��6/      	Direction$	 -90Q�6<      RenderingBias$ Performance=�6L      ShadowDepth$ 2Q�6h      Opacity$	 0.2Q�6x   .+�5   
   ��  �� CornerRadius$ 3��6    �� Padding$ 0q�6"   ���#000���#444$��0q�6T   ._�5      �� -   5         ��Name$ Logowebsite��5      $��Center=�6)   $��Center=�6I   $��60��6d   $��icon.png��6o   $
��Fill=�6�   $��60��6�   $��192,73,198,97q�6�   z�5      �� 5       �� CenterX$ 1Q�6'    �� Angle$ 0Q�63   5      5      �� 5   
   $�� Loading Data . . ��6   $��40��6-   $��Segoe WP Light)�6:   &   System.Windows.Media.TextOptions   TextFormattingMode$ Display=�6V      TextRenderingMode$ 	ClearType=�6   
�������$��Bottom=�6�   $��Center=�6�   $��151,0,140,25q�6�   $��20(�6  $	��159��6  5!      5#      