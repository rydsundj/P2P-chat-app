using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatApp.ViewModel;

namespace ChatApp.ViewModel.Command
{
    internal class AcceptCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private ServerViewModel parent;

        public AcceptCommand(ServerViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true; // Can always execute for simplicity
        }

        public void Execute(object parameter)
        {
            // Delegate the call to the ServerViewModel, which then calls NetworkManager
            Task.Run(async () => await parent.AcceptConnectionAsync());
        }
    }
}
