   M S B A M L   `   `   `  ����  WowonderLight$    WpfApplication2.MsgPopupWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  	PopUpform��5   	    � Title$ Notification Popup��5      $	��300��6)    � SizeToContent$ Height=�65    � WindowStyle$
 None=�5       � AllowsTransparency . 
������ .��5      �  J� RenderTransformOrigin$	 0,1&�6   $��84��6'    J� Effect 5   
   3  -System.Windows.Media.Effects.DropShadowEffect  5         
BlurRadius$ 15Q�6      Color$
 #000��6/   	   	Direction$		 -90Q�6<   
   RenderingBias$
 Performance=�6L      ShadowDepth$ 2Q�6h      Opacity$	 0.2Q�6x   .+�5   
   �� -   5         ��Name$ 	Backround��5      $��1q�6$   
�������
������� �� CornerRadius$ 2��6_   ._�5      �� $��20,19,-1,13q�6   $��50��6.   .+�5      �� -   5        $
��mask��5      
�������$ 3��68   $��48��6I   $��	0,4,234,0q�6U   �� 5      $��48��6   $��0,-47,234,0q�6)    J� OpacityMask 5      !� 5       !� Visual 6&   ��  �� ElementName$
 mask��.+�5      �� -   5         ��Name$ profileimage��5      $��Pictures\d-avatar.jpg��63   $��UniformToFill=�6R   $
��Left=�6j   $��45��6�   $��45��6�   �� 5      $��50��6    
���  ��6,   �� 5      $��50��6    
�������6,   �� 5      $��50��6    
����  �6,   5      �� 5      -   5         ��Name$ MsgFrom��5      $
��Left=�6.   $��
51,-49,0,0q�6I   $��Center=�6]   $
��Wrap=�6x   $��48��6�   $	��228��6�   �� 6�   .�6�   �� $"�New Message from Jony Verdas��6�   6�   6�   �� 5      -   5        $ 
msgContent��5      $	��220��60   $	��220��6<   $
��Wrap=�6K   $
��Left=�6_   $��
52,-31,0,0q�6z   $��Center=�6�   $"�� Hi mate i am waiting you !!��6�   $��30��6�   6�   5      5       � Triggers 5   
   :� 5        :� RoutedEvent$ FrameworkElement.Loaded��6   .[�5!      �� .��5"      �� .��5#      U�  �� TargetProperty$9 3(UIElement.RenderTransform).(ScaleTransform.ScaleY)�68   .\�5$      ��  O� KeyTime$ 0:0:0��63    O� Value$ 0Q�6C   �� 5%      $ 0:0:0.5��63   $ 1Q�6E   5&      5+      5,      5-      z�50   
   �� 51       �� ScaleY$ 1Q�6   54      56      