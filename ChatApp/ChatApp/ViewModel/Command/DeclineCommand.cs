using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class DeclineCommand : ICommand
    {
        private readonly ServerViewModel _viewModel;

        public DeclineCommand(ServerViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter) => true;

        public async void Execute(object parameter)
        {
            await _viewModel.DeclineConnectionAsync();
        }

        public event EventHandler CanExecuteChanged;
    }
}
