   M S B A M L   `   `   `  ����   WowonderLight$    WpfApplication2.MsgPopupWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     �Name$  	PopUpform�� � Title$ Notification Popup��$	��300�� � SizeToContent$ Height=� � WindowStyle$
 None=� � AllowsTransparency . 
������ .���  J� RenderTransformOrigin$	 0,1&�$��84�� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     
BlurRadius$ 15Q�   Color$
 #000��	   	Direction$		 -90Q�
   RenderingBias$
 Performance=�   ShadowDepth$ 2Q�   Opacity$	 0.2Q�.+��� -    ��Name$ 	Backround��$��1q�
�������
������� �� CornerRadius$ 2��._��� $��20,19,-1,13q�$��50��.+��� -   $
��mask��
�������$ 3��$��48��$��	0,4,234,0q��� $��48��$��0,-47,234,0q� J� OpacityMask !�  !� Visual ��  �� ElementName$
 mask��.+��� -    ��Name$ profileimage��$��Pictures\d-avatar.jpg��$��UniformToFill=�$
��Left=�$��45��$��45���� $��50��
���  ���� $��50��
��������� $��50��
����  ��� -    ��Name$ MsgFrom��$
��Left=�$��
51,-49,0,0q�$��Center=�$
��Wrap=�$��48��$	��228���� .��� $"�New Message from Jony Verdas���� -   $ 
msgContent��$	��220��$	��220��$
��Wrap=�$
��Left=�$��
52,-31,0,0q�$��Center=�$"�� Hi mate i am waiting you !!��$��30�� � Triggers :�  :� RoutedEvent$ FrameworkElement.Loaded��.[��� .���� .��U�  �� TargetProperty$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleY)�.\���  O� KeyTime$ 0:0:0�� O� Value$ 0Q��� $ 0:0:0.5��$ 1Q�z���  �� ScaleY$ 1Q�