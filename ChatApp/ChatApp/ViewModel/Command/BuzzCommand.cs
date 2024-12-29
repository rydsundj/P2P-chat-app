using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    public class BuzzCommand : ICommand
    {
        private readonly Action _execute;

        public BuzzCommand(Action execute)
        {
            _execute = execute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute?.Invoke();
        }
    }
}
