using System;

namespace ChatApp.Model
{
    public class ChatMessage
    {
        public string MessageType { get; set; } = "chatMessage";
        public string ChatMessageText { get; set; }
        public string NameOfSender { get; set; }

        public DateTime TimeSent { get; set; } 
    }

}