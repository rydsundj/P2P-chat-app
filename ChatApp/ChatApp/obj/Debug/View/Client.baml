   M S B A M L   `   `   `  ����B  >ChatApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:ChatApp.ViewChatApp.View  5   	       ChatApp.View.Client   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:ChatApp.View     5   	     � Title$  Client window��5   	   $	��400��6   $	��600��6,    � 	Resources 5
         	BoolToVis��%   &	        �� 5   
   6'   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $	��200 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��20(�5      $��Comic Sans MS)�5      $��10,10,10,321q�5      ��5      �� *SearchQuery+ �� UpdateSourceTrigger$ PropertyChanged=��� 5   
   -   5         �� DisplayMemberPath$ ChattingWith��5      $��10,45,10,10q�5      ��5      �� *MessageHistory+ �� SelectedItem 5      �� *SelectedConversation+ �� Mode$ TwoWay=�5       � 5#   
   $��1��6   $��10q�6    � RowDefinitions 5$      �� 5%      $
��Auto �6    �� 5&      $��* �6    �� 5'      $
��Auto �6    .+�5+      �� $��Center=�5,      $��16(�5-      
��� � �$��	0,10,0,10q�5/      ��5+      �� *ConnectionStatus+5/      �� 51      $��1��6
    �� 
IsReadOnly . "	 �� VerticalScrollBarVisibility$
	 Auto=�55   
   $
 �� HorizontalScrollBarVisibility$

 Auto=�56   
    �� TextWrapping$
 Wrap=�57   
   ��52   
   �� *ConversationHistory+y�53   
   �� *ShowHistoryBool+ �� 	Converter# [  57   
   �� 59      $��1��6   $
��Auto=�6(   y�5:   	   �� *ShowMessages+# [  .��5;      �� ��6   �� *
Messages+��5<      �� 5=      .R�5>      �� .+�5?   "   ��  �� Orientation$ 
Horizontal=�6-   .+�5@   &   �� $
��Wrap=�5A   0   $��5q�5B   0   $
��Bold"�5C   0   ��5@   0   �� *NameOfSender+5C   0   �� 5D   &   $��10(�5E   0   
�������$��5,0,0,0q�5G   0   $��Center=�5H   0   ��5D   0   �� *
TimeSent+ �� StringFormat$ {0:HH:mm:ss}��5H   0   5I   #   �� 5J   "   $
��Wrap=�5K   ,   $��5q�5L   ,   ��5J   ,   �� *ChatMessageText+5L   ,   5M      5N      5P      5Q      � 5V      $��2��6   $��0,10,0,0q�6     5W      �� 5X      $��* �6'   �� 5Y      $
��Auto �6'   �� 5Z      $
��Auto �6'   .+�5]      �� $��0��6   $��30��5_      $��0,0,10,0q�5`      ��5^      �� *ClientMessage+$ PropertyChanged=�5`      �� 5b      $��1��6   $
��Send��5c      $	��100��5e      $��0,0,10,0q�5f      ��5d      �� *SendMessageCommand+5f      �� 5h      $��2��6   $
��Buzz��5i      $	��100��5k      ��5j      �� *BuzzCommand+5k      5l      5m      5n      5o      