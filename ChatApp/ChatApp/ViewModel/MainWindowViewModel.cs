using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ChatApp.Model;
using ChatApp.ViewModel.Command;

namespace ChatApp.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {

        private NetworkManager NetworkManager { get; set; } = new NetworkManager();
        private ICommand startServer;
        private ICommand startClient;
        private string _ipAddress;
        private string _port = string.Empty;
        private string _name;


   

        public string IPAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged("IPAddress");
            }
        }

        public string Port
        {
            get => _port;
            set
            {
                _port = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public MainWindowViewModel()
        {
           NetworkManager.RequestDenied += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Request was denied by server.");
                });
            };

            NetworkManager.NoListener += (s, e) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"No one is listening to the given port.");
                });
            };
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand StartServer
        {
            get
            {
                if (startServer == null)
                    startServer = new StartServerCommand(this);
                return startServer;
            }
            set
            {
                startServer = value;
            }
        }

        public ICommand StartClient
        {
            get
            {
                if (startClient == null)
                    startClient = new StartClientCommand(this);
                return startClient;
            }
            set
            {
                startClient = value;
            }
        }

        public async Task StartServerWindow()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(IPAddress))
            {
                MessageBox.Show("Please fill out all fields correctly.");
                return;
            }

            if (int.TryParse(Port, out int portNumber))
            {
                await NetworkManager.StartServer(IPAddress, portNumber, Name);
            }
            else
            {
                MessageBox.Show("Invalid port number.");
            }
        }

        public async Task StartClientWindow()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(IPAddress))
            {
                MessageBox.Show("Please fill out all fields correctly.");
                return;
            }

            if (int.TryParse(Port, out int portNumber))
            {
                await NetworkManager.StartClient(IPAddress, portNumber, Name);
            }
            else
            {
                MessageBox.Show("Invalid port number.");
            }
        }
    
    }
}
