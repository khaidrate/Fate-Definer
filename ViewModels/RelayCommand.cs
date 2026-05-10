using System;
using System.Windows.Input;

namespace FateDefiner.ViewModels
{
    /// <summary>
    /// A generic ICommand implementation that wraps delegates.
    /// Enables binding UI actions to ViewModel methods without code-behind (MVVM technique).
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute    = execute    ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // WPF will call CanExecute whenever CommandManager.RequerySuggested fires
        public event EventHandler? CanExecuteChanged
        {
            add    => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter)    => _execute(parameter);

        /// <summary>Force WPF to re-evaluate CanExecute for all commands on the thread.</summary>
        public static void RaiseAll() => CommandManager.InvalidateRequerySuggested();
    }
}
