   M S B A M L   `   `   `  ����B  >ChatApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:ChatApp.ViewChatApp.View  5   	       ChatApp.View.Server   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:ChatApp.View     5   	     � Title$  Server window��5   	   $	��400��6   $	��600��6,    � 	Resources 5
         	BoolToVis��%   &	        �� 5   
   6'   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $	��200 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��20(�5      $��Comic Sans MS)�5      $��10,10,10,321q�5      ��5      �� *SearchQuery+ �� UpdateSourceTrigger$ PropertyChanged=��� 5   
   -   5         �� DisplayMemberPath$ ChattingWith��5      $��10,45,10,10q�5      ��5      �� *MessageHistory+ �� SelectedItem 5      �� *SelectedConversation+ �� Mode$ TwoWay=�5       � 5#   
   $��1��6   $��10q�6    � RowDefinitions 5$      �� 5%      $
��Auto �6    �� 5&      $��* �6    �� 5'      $
��Auto �6    .+�5+   
   �� $��16(�5,      $��Center=�6"   $	��Top=�5-      $��10q�6,   
���  ����5+      �� *ConnectionRequestMessage+5.      �� 51   
    �� Orientation$ 
Horizontal=�6   $��Center=�52      $	��Top=�53      $��
10,50,10,0q�54      y�55      �� *ShowButtons+	 �� 	Converter#	 [  .+�56      �� $��Accept��6   $��5q�6R   $	��100��6]   ��6&   �� *AcceptConnectionCommand+6]   �� 57      $��Decline��6   $��5q�6Y   $	��100��6d   ��6'   �� 
 �� Path$
 DeclineConnectionCommand�6d   58      �� 5:      $��Center=�5;      $��16(�5<      
��� � �$��	0,10,0,10q�5>      ��5:      �� *ConnectionStatus+5>      �� 5A      $��1��6
    �� 
IsReadOnly . " �� VerticalScrollBarVisibility$
 Auto=�5E   
   $ �� HorizontalScrollBarVisibility$
 Auto=�5F   
    �� TextWrapping$
 Wrap=�5G   
   ��5B   
   �� *ConversationHistory+y�5C   
   �� *ShowHistoryBool+#	 [  5G   
   �� 5I      $��1��6   $
��Auto=�6'   y�5J      �� *ShowMessages+#	 [  .��5K      �� ��6   �� *
Messages+��5L      �� 5M      .R�5N      �� .+�5O   "   �� $ 
Horizontal=�6-   .+�5P   &   �� $
��Wrap=�5Q   0   $��5q�5R   0   $
��Bold"�5S   0   ��5P   0   �� *NameOfSender+5S   0   �� 5T   &   $��10(�5U   0   
�������$��5,0,0,0q�5W   0   $��Center=�5X   0   ��5T   0   �� *
TimeSent+ �� StringFormat$ {0:HH:mm:ss}��5X   0   5Y   #   �� 5Z   "   $
��Wrap=�5[   ,   $��5q�5\   ,   ��5Z   ,   �� *ChatMessageText+5\   ,   5]      5^      5`      5a      � 5f      $��2��6   $��0,10,0,0q�6     5g      �� 5h      $��* �6'   �� 5i      $
��Auto �6'   �� 5j      $
��Auto �6'   .+�5m      �� $��0��6   $��30��5o      $��0,0,10,0q�5p      ��5n      �� *ServerMessage+$ PropertyChanged=�5p      �� 5r      $��1��6   $
��Send��5s      $	��100��5u      $��0,0,10,0q�5v      ��5t      �� *SendMessageCommand+5v      �� 5x      $��2��6   $
��Buzz��5y      $	��100��5{      ��5z      �� *BuzzCommand+5{      5|      5}      5~      5      