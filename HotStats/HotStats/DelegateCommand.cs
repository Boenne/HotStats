using System;
using System.Windows.Input;

namespace HotStats
{
    public class DelegateCommand : ICommand
    {
        private readonly Action execute;

        public DelegateCommand(Action execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            execute();
        }

        public event EventHandler CanExecuteChanged;
    }
}