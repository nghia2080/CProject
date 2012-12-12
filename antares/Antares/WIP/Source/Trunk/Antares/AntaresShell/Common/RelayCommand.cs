using System;
using System.Windows.Input;

namespace AntaresShell.Common
{
    /// <summary>
    /// Provide custom implementation for ICommand.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// Identifier of the method which will be executed when the command is invoked.
        /// </summary>
        private readonly Action<dynamic> _execute;

        /// <summary>
        /// Identifier of the method which will determine whether the command can be invoked.
        /// </summary>
        private readonly Predicate<dynamic> _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">Method which is called when the command is invoked.</param>
        public RelayCommand(Action<dynamic> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">Method which is called when the command is invoked.</param>
        /// <param name="canExecute">Method which determines whether the command can execute.</param>
        public RelayCommand(Action<dynamic> execute, Predicate<dynamic> canExecute)
        {
            if (execute == null)
            {
                return;
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}