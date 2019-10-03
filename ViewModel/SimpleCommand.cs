using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace InvoiceGeneratorCore.ViewModel
{
    class SimpleCommand : ICommand
    {
        public SimpleCommand(Action action) => this.action = action;

        bool allowExecute = true;
        public bool AllowExecute
        {
            get => allowExecute;
            set { if (allowExecute != value) { allowExecute = value; RaiseCanExecuteChanged(); } }
        }

        void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => allowExecute;
        public void Execute(object parameter) { if (allowExecute) action(); }

        private readonly Action action;
    }
}
