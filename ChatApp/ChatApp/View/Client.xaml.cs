using ChatApp.Model;
using ChatApp.ViewModel;
using System;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace ChatApp.View
{
    public partial class Client : Window
    {
        internal Client(NetworkManager networkManager, string clientName, string serverName)
        {
            InitializeComponent();
            this.DataContext = new ClientViewModel(networkManager, clientName, serverName);
            this.Closed += OnWindowClosed;

        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            ((ClientViewModel)this.DataContext)?.HandleWindowClosed();
        }

        private void OnConversationSelected(object sender, SelectionChangedEventArgs e)
        {
            var selectedConversation = (Conversation)((ListBox)sender).SelectedItem;

            if (selectedConversation != null)
            {
                ((ClientViewModel)this.DataContext)?.ShowConversationHistory(selectedConversation);
            }
        }

    }
}
