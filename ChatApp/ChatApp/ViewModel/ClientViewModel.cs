using ChatApp.Model;
using ChatApp.ViewModel.Command;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Media;
using System.Linq;
using System.IO;


namespace ChatApp.ViewModel
{ 
    internal class ClientViewModel : INotifyPropertyChanged
    {
        private NetworkManager _networkManager;

        public ObservableCollection<ChatMessage> Messages => _networkManager.Messages;
        public ObservableCollection<Conversation> MessageHistory
        {
            get
            {
                // Filter MessageHistory based on SearchQuery using LINQ
                var filteredHistory = string.IsNullOrEmpty(SearchQuery) ? _networkManager.ClientMessageHistory :
                    new ObservableCollection<Conversation>(_networkManager.ClientMessageHistory.Where(conversation => conversation.ChattingWith.Contains(SearchQuery)));

                // Sort the filtered history by the latest message
                 var sortedHistory = filteredHistory
                    .OrderByDescending(conversation => conversation.LastMessageTimestamp)
                    .ToList();

                return new ObservableCollection<Conversation>(sortedHistory);
            }
        }

   

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                    OnPropertyChanged(nameof(MessageHistory));
                }
            }
        }


        private bool _showMessages = true;
        public bool ShowMessages
        {
            get => _showMessages;
            set
            {
                _showMessages = value;
                OnPropertyChanged(nameof(ShowMessages));
            }
        }


        private bool _showHistoryBool;
        public bool ShowHistoryBool
        {
            get => _showHistoryBool;
            set
            {
                _showHistoryBool = value;
                OnPropertyChanged(nameof(ShowHistoryBool));
            }
        }


        private string _conversationHistory;
        public string ConversationHistory
        {
            get => _conversationHistory;
            set
            {
                if (_conversationHistory != value)
                {
                    _conversationHistory = value;
                    OnPropertyChanged(nameof(ConversationHistory));
                }
            }
        }

        public string ConnectionStatus
        {
            get => _networkManager.IsClientConnected ? $"Connected to {ServerName}" : "Disconnected";
        }

        public string ClientName { get; }
        public string ServerName { get; }


        private string _clientMessage;
        public string ClientMessage
        {
            get => _clientMessage;
            set
            {
                if (_clientMessage != value)
                {
                    _clientMessage = value;
                    OnPropertyChanged(nameof(ClientMessage));
                    (_sendMessageCommand as SendMessageCommand)?.RaiseCanExecuteChanged();

                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                if (_sendMessageCommand == null)
                {
                    _sendMessageCommand = new SendMessageCommand(
                        async () =>
                        {
                            if (_networkManager.CurrentClient == null)
                            {
                                MessageBox.Show("No active connection to send messages.");
                                return;
                            }

                            if (_networkManager.IsServerConnected && !string.IsNullOrWhiteSpace(ClientMessage))
                            {
                                await _networkManager.SendMessage(ClientName, ClientMessage);
                                ClientMessage = string.Empty; // Clear message box after sending
                            }
                        },
                        () => !string.IsNullOrWhiteSpace(ClientMessage) && _networkManager.IsClientConnected
                    );
                }
                return _sendMessageCommand;
            }
        }

        private ICommand _buzzCommand;
        public ICommand BuzzCommand
        {
            get
            {
                if (_buzzCommand == null)
                {
                    _buzzCommand = new BuzzCommand(async () =>
                    {
                        if (_networkManager.IsServerConnected)
                        {
                            await _networkManager.SendBuzz();
                        }
                    });
                }
                return _buzzCommand;
            }
        }
        public ClientViewModel(NetworkManager networkManager, string clientName, string serverName)
        {
            _networkManager = networkManager;
            ClientName = clientName;
            ServerName = serverName;

            _networkManager.PropertyChanged += NetworkManager_PropertyChanged;

            _networkManager.BuzzReceived += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PlayCustomSound();
                    //System.Media.SystemSounds.Asterisk.Play();
                });
            };
            _networkManager.UpdateHistory += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnPropertyChanged(nameof(MessageHistory));
                });
            };
            _networkManager.ServerClosed += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Server closed. Client-window will close.");
                });
            };
        }


        private void NetworkManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_networkManager.IsClientConnected))
            {
                OnPropertyChanged(nameof(ConnectionStatus));
            }
            else if (e.PropertyName == nameof(_networkManager.CurrentClient))
            {
                OnPropertyChanged(nameof(Messages)); 
            }
        }

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void ShowConversationHistory(Conversation selectedConversation)
        {
            if (selectedConversation.ChattingWith == ServerName)
            {

                ShowMessages = true;
                ShowHistoryBool = false;
            }
            else
            {
                ShowMessages = false;
                ShowHistoryBool = true;
            }

            if (selectedConversation != null)
            {
                ConversationHistory = string.Join(
                    Environment.NewLine + Environment.NewLine,
                    selectedConversation.Messages.Select(m =>
                        $"{m.TimeSent:HH:mm:ss} {m.NameOfSender}:{Environment.NewLine}{m.ChatMessageText}"));
            }
        }

        private void PlayCustomSound()
        {
            try
            {
                var soundFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "capy.wav");

                using (var player = new SoundPlayer(soundFilePath))
                {
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error playing sound: {ex.Message}");
            }
        }
        public void HandleWindowClosed()
        {
            if (_networkManager != null)
            {
                Task.Run(() => _networkManager.SendConnectionCloseMessage(ClientName));
            }
        }
    }
}