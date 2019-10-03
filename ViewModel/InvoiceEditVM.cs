using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InvoiceGeneratorCore.ViewModel
{
    class InvoiceEditVM : VM
    {
        public InvoiceEditVM(InvoiceVM invoiceVM)
        {
            Edited = invoiceVM;
            okCommand = new SimpleCommand(() => tcs.TrySetResult(true));
            cancelCommand = new SimpleCommand(() => tcs.TrySetResult(false));
        }

        public Task<bool> RunEdit() => tcs.Task;

        SimpleCommand okCommand, cancelCommand;

        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        public ICommand OkCommand => okCommand;
        public ICommand CancelCommand => cancelCommand;

        public InvoiceVM Edited { get; }
    }
}
