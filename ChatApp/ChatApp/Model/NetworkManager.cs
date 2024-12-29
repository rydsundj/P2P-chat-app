using ChatApp.View;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ChatApp;
using System.Threading;
using System.Xml.Linq;
using System.Collections.Generic;

namespace ChatApp.Model
{
    internal class NetworkManager : INotifyPropertyChanged
    {
        private string _message;
        private TcpListener _server;
        private TcpClient _currentClient;
        private bool _isServerConnected;
        private bool _isClientConnected;
        private Client _client;

        public string ServerName { get; private set; }
        public string ClientName { get; private set; }

        public ObservableCollection<ChatMessage> Messages { get; } = new ObservableCollection<ChatMessage>();
        public ObservableCollection<Conversation> ServerMessageHistory { get; set; } = new ObservableCollection<Conversation>();
        public ObservableCollection<Conversation> ClientMessageHistory { get; set; } = new ObservableCollection<Conversation>();

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public bool IsServerConnected
        {
            get => _isServerConnected;
            set
            {
                _isServerConnected = value;
                OnPropertyChanged(nameof(IsServerConnected));
            }
        }

        public bool IsClientConnected
        {
            get => _isClientConnected;
            set
            {
                _isClientConnected = value;
                OnPropertyChanged(nameof(IsClientConnected));
            }
        }

        public TcpClient CurrentClient
        {
            get => _currentClient;
            private set
            {
                _currentClient = value;
                OnPropertyChanged(nameof(CurrentClient));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ChatMessage> MessageReceived;
        public event EventHandler<ConnectionRequest> ConnectionRequestReceived;
        public event EventHandler ConnectionClosed;
        public event EventHandler ServerClosed;
        public event EventHandler BuzzReceived;
        public event EventHandler UpdateHistory;
        public event EventHandler RequestDenied;
        public event EventHandler NoListener;

        private const string HistoryFilePath = "../../conversation_history.json";
        private ObservableCollection<User> _conversationHistories;

        public ObservableCollection<User> ConversationHistories
        {
            get => _conversationHistories;
            private set
            {
                _conversationHistories = value;
                OnPropertyChanged(nameof(ConversationHistories));
            }
        }

        private void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public NetworkManager()
        {
            Task.Run(async () => await LoadConversationHistory());
        }

        private async Task LoadConversationHistory()
        {
            try
            {
                if (File.Exists(HistoryFilePath))
                {
                    using (var reader = new StreamReader(HistoryFilePath))
                    {
                        var json = await reader.ReadToEndAsync();
                        var users = JsonConvert.DeserializeObject<ObservableCollection<User>>(json);
                        ConversationHistories = users ?? new ObservableCollection<User>();
                    }
                }
                else
                {
                    ConversationHistories = new ObservableCollection<User>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading conversation history: {ex.Message}");
            }
        }

        private async Task SaveConversationHistory()
        {
            // If file is used in another process, wait 100 milliseconds and retry
            const int MaxRetries = 3;
            const int Delay = 100; // Milliseconds
            int retries = 0;

            while (true)
            {
                try
                {
                    using (var fileStream = new FileStream(HistoryFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var writer = new StreamWriter(fileStream))
                    {
                        var json = JsonConvert.SerializeObject(ConversationHistories, Formatting.Indented);
                        await writer.WriteAsync(json);
                    }
                    break;
                }
                catch (IOException) when (retries < MaxRetries)
                {
                    retries++;
                    await Task.Delay(Delay); // Wait asynchronously before retrying
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error saving conversation history: {ex.Message}");
                    break;
                }
            }
        }

        public void AddMessageToHistory(string userName, string chattingWith, ChatMessage message)
        {
            var user = ConversationHistories.FirstOrDefault(u => u.UserName == userName);
            if (user == null)
            {
                user = new User { UserName = userName };
                ConversationHistories.Add(user);
            }

            var conversation = user.Conversations.FirstOrDefault(c => c.ChattingWith == chattingWith);
            if (conversation == null)
            {
                conversation = new Conversation { ChattingWith = chattingWith };
                user.Conversations.Add(conversation);
            }

            conversation.LastMessageTimestamp = DateTime.Now;
            conversation.Messages.Add(message);

            Task.Run(async () => await SaveConversationHistory());
        }

        public void LoadMessagesForConversation(string userName, string chattingWith)
        {
            var user = ConversationHistories.FirstOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                // Find the specific conversation with the other user
                var conversation = user.Conversations.FirstOrDefault(c => c.ChattingWith == chattingWith);
                if (conversation != null)
                {
                    // Replace the Messages collection with the conversation's messages
                    Messages.Clear();
                    foreach (var message in conversation.Messages)
                    {
                        Messages.Add(message);
                    }
                    return;
                }
            }
        }

        public async Task SetConvHistory(string name)
        {
            var user = await Task.Run(() => ConversationHistories.FirstOrDefault(u => u.UserName == name)).ConfigureAwait(false);

            if (user == null)
            {
                user = new User
                {
                    UserName = name,
                    Conversations = new ObservableCollection<Conversation>()
                };

                await Task.Run(() => ConversationHistories.Add(user)).ConfigureAwait(false);
            }

            if (name == ServerName)
            {
                ServerMessageHistory = user.Conversations;
            }
            else if (name == ClientName)
            {
                ClientMessageHistory = user.Conversations;
            }
        }

        public async Task StartServer(string ipAddress, int port, string name)
        {
            ServerName = name;
            await Task.Delay(100);
            await SetConvHistory(name);

            await Task.Run(() =>
            {
                try
                {
                    var ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);

                    _server = new TcpListener(ipEndPoint);
                    //_server.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _server.Start();

                    Task.Run(() => ListenForClients()); // Handle clients asynchronously

                    IsServerConnected = true;

                    // Open the Server window on the main UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Server ser = new Server(this, ServerName);
                        ser.Show();
                    });

                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine($"Error starting server: {ex.Message}");
                    });
                }
            });
        }

        private async Task ListenForClients()
        {
            try
            {
                while (_server != null) 
                {
                    try
                    {
                        if (IsClientConnected)
                        {
                            Console.WriteLine("A client is already connected. New connections are ignored.");
                            await Task.Delay(100); // Small delay..
                            continue;
                        }

                        TcpClient client = await _server.AcceptTcpClientAsync();
                        if (client.Connected)
                        {
                            CurrentClient = client;
                            IsServerConnected = true; // Indicate that the server is connected to a client
                            IsClientConnected = true;
                            HandleClientConnection(client); 
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Console.WriteLine($"Error listening for clients: {ex.Message}");
                });
            }
        }

        public async Task StartClient(string ipAddress, int port, string name)
        {
            try
            {
                TcpClient client = new TcpClient();

                ClientName = name;

                // Asynchronously connect to the server
                await client.ConnectAsync(ipAddress, port);
                if (client.Connected)
                {
                    CurrentClient = client;

                    var request = new ConnectionRequest
                    {
                        MessageType = "connectRequest",
                        Name = name,
                        IPAddress = ipAddress,
                        Port = port
                    };


                    string jsonRequest = JsonConvert.SerializeObject(request) + "\n";
                    NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(jsonRequest);

                    // Write data to the server
                    await stream.WriteAsync(data, 0, data.Length);

                    await Task.Run(() => ListenForServerMessages(client));
                }
                else
                {
                    IsClientConnected = false;
                }
            }
            catch (Exception ex)
            {
                IsClientConnected = false;
                NoListener?.Invoke(this, EventArgs.Empty);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Console.WriteLine($"Error connecting to server: {ex.Message}");
                });
            }
        }

        private void ListenForServerMessages(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    while (true)
                    {
                        string jsonMessage = reader.ReadLine();
                        if (jsonMessage == null) break;

                        ParseServerMessage(jsonMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Console.WriteLine($"Error receiving messages from server: {ex.Message}");
                });
            }
        }

        private void ParseServerMessage(string jsonMessage)
        {
            try
            {
                if (jsonMessage == "Buzz")
                {
                    BuzzReceived?.Invoke(this, EventArgs.Empty);
                    return;
                }
                
                var connectionAccept = JsonConvert.DeserializeObject<ConnectionAccept>(jsonMessage);
                var connectionRequest = JsonConvert.DeserializeObject<ConnectionRequest>(jsonMessage);

                if (connectionAccept?.MessageType == "connectAccept")
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IsClientConnected = true;
                        IsServerConnected = true;
                        ServerName = connectionAccept.Name;

                        Task.Run(async () => await Task.Delay(100));
                        Task.Run(async () => await SetConvHistory(ClientName));

                        // Open the Client window
                        _client = new Client(this, ClientName, ServerName);
                        _client.Show();

                        LoadMessagesForConversation(ClientName, ServerName);
                    });
                }
                else if (connectionAccept?.MessageType == "connectDenied")
                {
                    RequestDenied?.Invoke(this, EventArgs.Empty);
                    IsClientConnected = false;
                    return;
                }
                else if (connectionRequest?.MessageType == "connectionClose")
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ServerClosed?.Invoke(this, EventArgs.Empty);
                        IsServerConnected = false;
                        IsClientConnected = false;
                        CurrentClient = null;
                        _server?.Stop();
                        _server = null;
                        IsServerConnected = false;
                        _client.Close();
                    });
                }
                else
                {
                    var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(jsonMessage);
                    if (chatMessage != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Messages.Add(chatMessage);
                            AddMessageToHistory(ClientName, ServerName, chatMessage);
                            UpdateHistory?.Invoke(this, EventArgs.Empty);
                        });
                        MessageReceived?.Invoke(this, chatMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Console.WriteLine($"Error parsing server message: {ex.Message}");
                });
            }
        }

        private void HandleClientConnection(TcpClient client)
        {
            Task.Run(async () =>
            {
                try
                {

                    NetworkStream stream = client.GetStream();
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        while (true)
                        {
                            string jsonMessage = await reader.ReadLineAsync();
                            if (string.IsNullOrEmpty(jsonMessage)) break;

                            ParseClientMessage(jsonMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Console.WriteLine($"Error handling client connection: {ex.Message}");
                    });
                }
            });
        }

        private void ParseClientMessage(string jsonMessage)
        {
            try
            {

                if (jsonMessage == "Buzz")
                {
                    BuzzReceived?.Invoke(this, EventArgs.Empty);
                    return;
                }

                var connectionRequest = JsonConvert.DeserializeObject<ConnectionRequest>(jsonMessage);
                if (connectionRequest?.MessageType == "connectRequest")
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ClientName = connectionRequest.Name;
                        ConnectionRequestReceived?.Invoke(this, connectionRequest);
                    });
                }
                else if (connectionRequest?.MessageType == "connectionClose")
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IsClientConnected = false;
                        CurrentClient?.Close();
                        CurrentClient = null;
                        Messages.Clear();
                        ConnectionClosed?.Invoke(this, EventArgs.Empty);
                    });
                }
                else
                {
                    var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(jsonMessage);
                    if (chatMessage != null)
                    {

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Messages.Add(chatMessage);
                            MessageReceived?.Invoke(this, chatMessage);
                            AddMessageToHistory(ClientName, ServerName, chatMessage);
                            AddMessageToHistory(ServerName, ClientName, chatMessage);
                            UpdateHistory?.Invoke(this, EventArgs.Empty);
                        });
                    }
                    else
                    {
                        Console.WriteLine("Received message is not a valid ChatMessage.");
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Console.WriteLine($"Error parsing client message: {ex.Message}");
                });
            }
        }

        public event EventHandler<string> ClientConnected;

        public async Task AcceptConnection()
        {
            if (_currentClient != null)
            {
                var response = new ConnectionAccept
                {
                    MessageType = "connectAccept",
                    Name = ServerName
                };

                string jsonResponse = JsonConvert.SerializeObject(response) + "\n";
                NetworkStream stream = CurrentClient.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(jsonResponse);

                await stream.WriteAsync(data, 0, data.Length);

                IsClientConnected = true;
                ClientConnected?.Invoke(this, ClientName);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    LoadMessagesForConversation(ServerName, ClientName);
                });
            }
        }

        public async Task DeclineConnection()
        {
            if (_currentClient != null)
            {
                var response = new ConnectionAccept
                {
                    MessageType = "connectDenied",
                    Name = ServerName
                };

                string jsonResponse = JsonConvert.SerializeObject(response) + "\n";
                NetworkStream stream = _currentClient.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(jsonResponse);

                await stream.WriteAsync(data, 0, data.Length);
                _currentClient.Close();
                _currentClient = null;
                IsClientConnected = false;
            }
        }

        public async Task SendMessage(string senderName, string messageContent)
        {
            try
            {
                if (!IsClientConnected && !IsServerConnected)
                    throw new InvalidOperationException("No active connection to send messages.");

                var chatMessage = new ChatMessage
                {
                    ChatMessageText = messageContent,
                    NameOfSender = senderName,
                    TimeSent = DateTime.Now
                };

                Messages.Add(chatMessage);

                if (IsServerConnected && !IsClientConnected)
                {
                    MessageReceived?.Invoke(this, chatMessage);
                }

                string jsonMessage = JsonConvert.SerializeObject(chatMessage) + "\n";
                byte[] data = Encoding.UTF8.GetBytes(jsonMessage);

                NetworkStream stream = null;

                if (IsServerConnected && IsClientConnected && CurrentClient != null)
                {
                    stream = CurrentClient.GetStream();
                }

                if (stream != null && stream.CanWrite)
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
                else
                {
                    throw new InvalidOperationException("Stream is null, disposed, or not writable.");
                }

                string recipient;
                if (senderName == ServerName)
                {
                    recipient = ClientName;
                }
                else
                {
                    recipient = ServerName;
                }
                AddMessageToHistory(senderName, recipient, chatMessage);
                AddMessageToHistory(recipient, senderName, chatMessage);
                UpdateHistory?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }


        public async Task SendBuzz()
        {
            try
            {
                if (!IsClientConnected && !IsServerConnected)
                    throw new InvalidOperationException("No active connection to send a buzz.");

                string buzzMessage = "Buzz\n";
                byte[] data = Encoding.UTF8.GetBytes(buzzMessage);

                NetworkStream stream = null;

                if (IsServerConnected && IsClientConnected && CurrentClient != null)
                {
                    stream = CurrentClient.GetStream();
                }

                if (stream != null && stream.CanWrite)
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
                else
                {
                    throw new InvalidOperationException("Stream is null, disposed, or not writable.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending buzz: {ex.Message}");
            }
        }

        public async Task SendConnectionCloseMessage(string name)
        {
            if ((IsServerConnected && name == ServerName) || (IsClientConnected && IsServerConnected && CurrentClient != null))
            {
                try
                {
                    if (name == ServerName && !IsClientConnected)
                    {
                        // Stop the TCPListener on the server-side when closing
                        _server?.Stop();
                        _server = null;
                        ServerName = null;
                        ClientName = null;
                        IsServerConnected = false;
                        _currentClient = null;
                    }

                    var connectionClose = new ConnectionRequest
                    {
                        MessageType = "connectionClose",
                        Name = name
                    };

                    string jsonRequest = JsonConvert.SerializeObject(connectionClose) + "\n";
                    byte[] data = Encoding.UTF8.GetBytes(jsonRequest);

                    NetworkStream stream = CurrentClient.GetStream();
                    await stream.WriteAsync(data, 0, data.Length);

                    if (name == ServerName)
                    {
                        // Stop the TCPListener on the server-side when closing
                        _server?.Stop();
                        _server = null;
                        ServerName = null;
                        ClientName = null;
                        IsServerConnected = false;
                        _currentClient = null;
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Messages.Clear();
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending connection close message: {ex.Message}");
                }
            }
        }
    }
}
