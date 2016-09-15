using System;
using System.Windows.Input;

namespace DevExpressGridInconsistencyDemo
{
    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<Object> _canExecute;

        public RelayCommand(Action execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action execute, Predicate<Object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = p => execute();
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute, Predicate<Object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
