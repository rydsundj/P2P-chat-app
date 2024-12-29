using System;
using System.Windows;
using System.Windows.Controls;
using ChatApp.Model;
using ChatApp.ViewModel;

namespace ChatApp.View
{
    public partial class Server : Window
    {
        internal Server(NetworkManager networkManager, string serverName)
        {
            InitializeComponent();
            this.DataContext = new ServerViewModel(networkManager, serverName);
            this.Closed += OnWindowClosed;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            // Notify the NetworkManager about the closure
            ((ServerViewModel)this.DataContext)?.HandleWindowClosed();
        }

        private void OnConversationSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedConversation = (Conversation)((ListBox)sender).SelectedItem;

            if (selectedConversation != null)
            {
                ((ServerViewModel)this.DataContext)?.ShowConversationHistory(selectedConversation);
            }
        }
    }
}
