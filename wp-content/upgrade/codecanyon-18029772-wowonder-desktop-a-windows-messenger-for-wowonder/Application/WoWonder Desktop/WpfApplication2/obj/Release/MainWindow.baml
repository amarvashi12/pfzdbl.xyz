   M S B A M L   `   `   `  ����   WowonderLight     WpfApplication2.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     �Name$  	formLogin�� � Title$ Wowonder��$	��399��$	��540�����#444 � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ NoResize=� � WindowStyle$
 None=� � AllowsTransparency .  � 	Resources    FGColor �� Color  FGHoverColor��%6   &	        &	      �� $ #A04545���� $	 Red��.��� $��0q�.+���  �� CornerRadius$ 2��	 J� RenderTransformOrigin$	 	0.5,0.542&�
 J� Effect
 3  -System.Windows.Media.Effects.DropShadowEffect     
BlurRadius$ 5Q�   Color$
 #888��   Opacity$	 0.5Q�   ShadowDepth$	 0.5Q�._�� 
   $ 15Q�$
 #000��   	Direction$	 -90Q�   RenderingBias$ Performance=�$ 2Q�$	 0.1Q�.+��� ���	#FFFFFDFD$ 2��$��158,153,154,213q�$	 	0.5,0.542&�._��� -    ��Name$ Rigesterpanel_Copy2��$
��Left=�$��38��$	��Top=�$	��228��
�������$��158,205,0,0q�$	 0.503,0.533&��� $ 2��$	 	0.5,0.542&�._��� -   $ 	panelmove��$
��Left=�$��30��$	��Top=�$	��540�����#555�� -    ��Name$ ApplicationName��$
��Wrap=�$��WoWonder��$��29,7,444,373q����	#FFFBFBFB�� -   $ Rigesterpanel��$
��Left=�$��45��$	��Top=�$	��540�����#555$��	0,354,0,0q��� -    ��Name$ Minimize��  minimize#��[ $
��Left=�$��	494,6,0,0q�$	��Top=�$��18��$��18�����	#FFF3EAEA$	 
1.366,0.72&� � ToolTip$
 Exit����Z�  � ContextMenu �� ���� $ 	#FF707070�� �� Opacity$
 0.01Q��� -   $ Exit_Button��  exit#��[ $��	515,5,5,0q�$	��Top=�$��20��$��20�����	#FFF3EAEA$	 
1.366,0.72&�$
 Exit����Z�  �� ���� $ 	#FF707070��$
 0.01Q��� ���	#FFFFFDFD$ 2��$��158,199,154,167q�$	 	0.5,0.542&�._��� -   $ Rigesterpanel_Copy��$
��Left=�$��38��$	��Top=�$	��228��
�������$��158,205,0,0q�$	 0.503,0.533&��� -	    ��Name$ Logowebsite��$
��Left=�$��46��$��
158,56,0,0q�$	��Top=�$	��228��$��logo.png���� -
    ��Name$ Password��$
��Left=�$��30��$��187,200,0,0q� �� TextWrapping$
 Wrap=�$��Password��$	��Top=�$	��194��
����������	#FF9B9B9B$��0q�$��0,5,5,0q��� $
��Left=�$��13��$��166,208,0,0q�$	��Top=�$��13��$��Icons\Unlock-25.png��$	 
0.42,0.524&�$
��Fill=��� -   $ Username��$
��Left=�$��30��$��0,5,5,0q�$��187,155,0,0q�$
 Wrap=�$��Username��$	��Top=�$	��194��
����������	#FF9B9B9B$��0q��� $
��Left=�$��13��$��167,162,0,0q�$	��Top=�$��13��$��Icons\username.png��$	 
0.42,0.524&�$
��Fill=��� -   $ Loginbutton��$��Login��$��14(�$��158,273,154,0q�$��34��
����������	#FFF3EAEA$	 
1.366,0.72&�$��Center=�$	��Top=�$	��228������  �� 
TargetType" ��.R���  ��Name$ bdr_main��$ 2��#���    �� Padding$ 6q�._��� $��Center=�$��Content�� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ���   �� 
TargetName$  bdr_main��! �� Value#! �  " �� Property" � ��P�  � �� . . ��� $  bdr_main��#! �  " � ���� -   # ��Name$# 	Keepmechk��
�������$��Keep me sign in��$
��Left=�$��158,245,0,0q�$	��Top=�$ J� OpacityMask
$ ������� -   $ RegisterButton��$��Register new account��$
��Left=�$��200,360,0,0q�$	��Top=�$	��148��
������ 
����������	#FF575757$��30��$��14(����� " ��.R��� $ bdr_main��$ 2��
������ $ 6q�% � Cursor$
% Hand��._��� $��Center=�$��Content�� P�  �@J�IsMouseOver . . ��� $  bdr_main��" � ��
! ���� P�  � �� . . ��� $  bdr_main��" � ��
! ���� �� -   $ 
errorpanel�����#555$ 2��$y�Hidden=�$��158,113,154,259q�$	 	0.5,0.542&�._��� $��	0,-1,0,-4q�.+��� -   $ ErrorPic��$
��Left=�$��17��$��5q�$	��Top=�$��17��$��Attention-25.png��$	 
0.42,0.524&�$
��Fill=��� -   $ 
ErrorLable��$
��Left=�$��25,-21,-15,50q�$
��Wrap=�$��Error!!��$	��Top=����	#FFF9F9F9�� -   $
 icon��$
��Left=�$��17��$��7,6,0,0q�$	��Top=�$��17��$��icon.png��$	 0.529,0.706&�$
��Fill=�