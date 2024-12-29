using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ChatApp.ViewModel.Command;
using ChatApp.Model;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Media;
using System.IO;
using System.Linq;

namespace ChatApp.ViewModel
{
    internal class ServerViewModel : INotifyPropertyChanged
    {
        private NetworkManager _networkManager { get; set; }
        private string _connectionRequestMessage;
        private TcpClient _currentClient;
        private string _serverMessage;
        public ObservableCollection<ChatMessage> Messages => _networkManager.Messages;

        public event PropertyChangedEventHandler PropertyChanged;
        public string ServerName { get; set; }

        public ObservableCollection<Conversation> MessageHistory
        {
            get
            {
                // Filter MessageHistory based on SearchQuery using LINQ
                var filteredHistory = string.IsNullOrEmpty(SearchQuery) ? _networkManager.ServerMessageHistory :
                    new ObservableCollection<Conversation>(_networkManager.ServerMessageHistory.Where(conversation => conversation.ChattingWith.Contains(SearchQuery)));

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

        private Conversation _selectedConversation;
        public Conversation SelectedConversation
        {
            get => _selectedConversation;
            set
            {
                if (_selectedConversation != value)
                {
                    _selectedConversation = value;
                    OnPropertyChanged(nameof(SelectedConversation));
                }
            }
        }

        private string _clientName;
        public string ClientName
        {
            get => _clientName;
            set
            {
                if (_clientName != value)
                {
                    _clientName = value;
                    OnPropertyChanged(nameof(ClientName));
                }
            }
        }

        public string ServerMessage
        {
            get => _serverMessage;
            set
            {
                if (_serverMessage != value)
                {
                    _serverMessage = value;
                    OnPropertyChanged(nameof(ServerMessage));
                    (_sendMessageCommand as SendMessageCommand)?.RaiseCanExecuteChanged();
                }
            }
        }


        public string ConnectionRequestMessage
        {
            get => _connectionRequestMessage;
            set
            {
                if (_connectionRequestMessage != value)
                {
                    _connectionRequestMessage = value;
                    OnPropertyChanged(nameof(ConnectionRequestMessage));
                }
            }
        }

        public TcpClient CurrentClient
        {
            get => _currentClient;
            set
            {
                if (_currentClient != value)
                {
                    _currentClient = value;
                    OnPropertyChanged(nameof(CurrentClient));
                }
            }
        }

        private bool _showButtons;
        public bool ShowButtons
        {
            get => _showButtons;
            set
            {
                _showButtons = value;
                OnPropertyChanged(nameof(ShowButtons));
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

        public ServerViewModel(NetworkManager networkManager, string servername)
        {
            _networkManager = networkManager;
            ServerName = servername;

            _networkManager.PropertyChanged += myModel_PropertyChanged;

            _networkManager.ConnectionRequestReceived += (s, e) =>
            {
                ShowHistoryBool = false;
                ConnectionRequestMessage = $"Connection request from {e.Name}";
                ShowButtons = true;
            };
            _networkManager.ClientConnected += (s, clientName) =>
            {
                ClientName = clientName;
                ShowMessages = true;
                OnPropertyChanged(nameof(ConnectionStatus));
            };
            _networkManager.ConnectionClosed += (s, _) =>
            {
                ShowHistoryBool = false;
                ConnectionRequestMessage = $"{ClientName} has disconnected.";
                OnPropertyChanged(nameof(ConnectionStatus));
                _currentClient = null;
                ClientName = null;
            };
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
        }

        private void myModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_networkManager.CurrentClient))
            {
                // React to changes in the CurrentClient
                if (_networkManager.CurrentClient != null)
                {
                    _currentClient = _networkManager.CurrentClient;
                }
            }
        }

        public string ConnectionStatus
        {
            get => _networkManager.IsServerConnected && _networkManager.IsClientConnected && !string.IsNullOrEmpty(ClientName)
                ? $"Connected to {ClientName}"
                : "";
        }

        public async Task AcceptConnectionAsync()
        {
            if (_networkManager != null)
            {
                await _networkManager.AcceptConnection();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ConnectionRequestMessage = string.Empty;
                    ShowButtons = false;
                });
            }
        }

        public async Task DeclineConnectionAsync()
        {
            if (_networkManager != null)
            {
                await _networkManager.DeclineConnection();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ConnectionRequestMessage = string.Empty;
                    ShowButtons = false;
                });
            }
        }

        private void NetworkManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_networkManager.IsClientConnected))
            {
                OnPropertyChanged(nameof(ConnectionStatus));
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private ICommand acceptConnection;
        public ICommand AcceptConnectionCommand
        {

            get
            {
                if (acceptConnection == null)
                    acceptConnection = new AcceptCommand(this);
                return acceptConnection;
            }
            set
            {
                acceptConnection = value;
            }
        }

        private ICommand declineConnection;
        public ICommand DeclineConnectionCommand
        {
            get
            {
                if (declineConnection == null)
                    declineConnection = new DeclineCommand(this);
                return declineConnection;
            }
            set
            {
                declineConnection = value;
            }
        }

        private ICommand _sendMessageCommand;
        public ICommand SendMessageCommand
        {
            get
            {
                if (_sendMessageCommand == null)
                {
                    _sendMessageCommand = new SendMessageCommand(async () =>
                    {
                        if (_networkManager.IsClientConnected && !string.IsNullOrWhiteSpace(ServerMessage))
                        {
                            await _networkManager.SendMessage(ServerName, ServerMessage);
                            ServerMessage = string.Empty; // Clear the message input
                        }
                    },
                    () => !string.IsNullOrWhiteSpace(ServerMessage));
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
                        if (_networkManager.IsClientConnected)
                        {
                            await _networkManager.SendBuzz();
                        }
                    });
                }
                return _buzzCommand;
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

        public void ShowConversationHistory(Conversation selectedConversation)
        {
            if (selectedConversation.ChattingWith == ClientName)
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

        public void HandleWindowClosed()
        {
            // Notify that the server is shutting down
            if (_networkManager != null)
            {
                Task.Run(() => _networkManager.SendConnectionCloseMessage(ServerName));
            }
        }

    }
}
