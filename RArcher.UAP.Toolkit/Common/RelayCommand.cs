using System;
using System.Windows.Input;

namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>Implements to ICommand interface. Used to hook UI components to ViewModel command receivers</summary>
    public class RelayCommand : ICommand
    {
        /// <summary>True if the command can be executed</summary>
        public event EventHandler CanExecuteChanged;

        private readonly Action<object> _targetExecuteMethod;
        private readonly Predicate<object> _targetCanExecuteMethod;

        /// <summary>Returns true if the command can be executed</summary>
        public bool CanExecute(object parameter)
        {
            return _targetCanExecuteMethod == null || _targetCanExecuteMethod(parameter);
        }

        /// <summary>Executes the command</summary>
        public void Execute(object parameter)
        {
            // Call the delegate if it's not null
            if (_targetExecuteMethod != null) _targetExecuteMethod(parameter);
        }

        /// <summary>Delegate for the method to be executed</summary>
        public RelayCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod = null)
        {
            _targetExecuteMethod = executeMethod;
            _targetCanExecuteMethod = canExecuteMethod;
        }

        /// <summary>Raises the CanExecuteChanged event</summary>
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}