   M S B A M L   `   `   `  ����B  >ChatApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:ChatAppChatApp  5   	       ChatApp.View.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:ChatApp     5   	     � Title$  
MainWindow��5   	   $	��450��6   $	��800��6)   .��5	      � .+�5
      �� $��Center=�6   $��0,71,0,0q�6)    �� TextWrapping$
 Wrap=�6;   $	��Top=�6�   $	��120��6�   ��6O   �� *Name+ �� UpdateSourceTrigger$ PropertyChanged=�6�   �� 5      $��Center=�6   $��	0,118,0,0q�6)   $
 Wrap=�6<   $	��Top=�6�   $	��120��6�   ��6P   �� *Port+$ PropertyChanged=�6�   �� 5      $��Center=�6   $��	0,163,0,0q�6)   $
 Wrap=�6<   $	��Top=�6�   $	��120��6�   ��6P   �� *	IPAddress+$ PropertyChanged=�6�   �� 5      $
��Left=�6   $��
341,55,0,0q�6)   $
��Wrap=�6=   $	��Top=�6Q   �� 5       � Language$ sv-se�6	   $
�Name��6   5      �� 5      $
��Left=�6   $��
340,97,0,0q�6)   $
��Wrap=�6=   $	��Top=�6Q   �� 5      $ sv-se�6	   $�Port number��6   5      �� 5      $
��Left=�6   $��341,142,0,0q�6)   $
��Wrap=�6>   $	��Top=�6R   �� 5      $ sv-se�6	   $�
IP address��6   5      �� 5      $��Start Server��6   $��Center=�6"   $��	0,217,0,0q�6?   $	��Top=�6R   ��6j   ��  �� Path$ StartServer��� 5      $��Start Client��6   $��Center=�6"   $��	0,254,0,0q�6?   $	��Top=�6R   ��6j   �� $ StartClient�5      5      