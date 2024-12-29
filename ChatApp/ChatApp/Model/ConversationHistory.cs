using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    public class ConversationHistory
    {
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();
    }

    public class User
    {
        public string UserName { get; set; }
        public ObservableCollection<Conversation> Conversations { get; set; } = new ObservableCollection<Conversation>();
    }

    public class Conversation
    {
        public string ChattingWith { get; set; }
        public DateTime LastMessageTimestamp { get; set; }
        public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();
    }
}
