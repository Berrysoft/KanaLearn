using System;
using System.Windows.Input;

namespace KanaLearn
{
    public class Command : ICommand
    {
        readonly Func<bool>? canExecute;
        readonly Action execute;

        public Command(Action execute)
        {
            this.execute = execute;
        }

        public Command(Action execute, Func<bool>? canExecute) : this(execute)
        {
            if (canExecute != null)
                this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

        public event EventHandler? CanExecuteChanged;

        public void Execute(object? parameter) => execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
