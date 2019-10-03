using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using InvoiceGeneratorCore.Model;
using InvoiceGeneratorCore.Storage;
using Microsoft.EntityFrameworkCore;

namespace InvoiceGeneratorCore.ViewModel
{
    class MainVM : VM
    {
        public MainVM(DbContextOptions<InvoiceContext> contextOptions)
        {
            context = new InvoiceContext(contextOptions);
            addCommand = new SimpleCommand(() => ExecuteOperation(Add));
            deleteCommand = new SimpleCommand(() => ExecuteOperation(() => Delete(CurrentInvoice)));
            editCommand = new SimpleCommand(() => ExecuteOperation(() => Edit(CurrentInvoice)));
            filterCommand = new SimpleCommand(() => ExecuteOperation(Populate));
            ExecuteOperation(Populate); // will update command state
        }

        public ObservableCollection<InvoiceVM> Invoices { get; } = new ObservableCollection<InvoiceVM>();

        public int InvoiceCount => Invoices.Count;
        void RefreshInvoiceCount() => RaisePropertyChanged(nameof(InvoiceCount));

        async void ExecuteOperation(Func<Task> operation)
        {
            if (!IsReady)
                return;
            try
            {
                IsReady = false;
                LastError = null;
                UpdateCommandState();
                await operation();
            }
            catch (DbException e)
            {
                LastError = e.Message;
            }
            finally
            {
                IsReady = true;
                UpdateCommandState();
            }
        }

        // operations
        async Task Populate()
        {
            CurrentInvoice = null;
            Invoices.Clear();
            RefreshInvoiceCount();
            var currentFilterState = ClientFilter.Trim();
            IQueryable<Invoice> invoices = context.Invoices;
            bool haveFilter = !string.IsNullOrWhiteSpace(currentFilterState);
            IsUsingFilter = haveFilter;
            if (haveFilter)
            {
                invoices = invoices.Where(
                    invoice => invoice.Client.Contains(currentFilterState));
            }
            await foreach (var invoice in invoices.AsAsyncEnumerable())
            {
                Invoices.Add(new InvoiceVM(invoice));
                RefreshInvoiceCount();
            }
        }

        async Task Add()
        {
            var invoice = new Invoice() { Date = DateTime.Today, Amount = 1, Client = "New Client" };
            var invoiceVM = new InvoiceVM(invoice);
            if (await DisplayEdit(invoiceVM)) // need to save
            {
                Invoices.Add(invoiceVM);
                RefreshInvoiceCount();
                await context.AddAsync(invoice);
                await context.SaveChangesAsync();
                invoiceVM.RefreshId();
            }
            // else the adding is cancelled so no action is needed
        }

        async Task Delete(InvoiceVM invoiceVM)
        {
            context.Remove(invoiceVM.Invoice);
            await context.SaveChangesAsync();
            var index = Invoices.IndexOf(invoiceVM);
            if (index < 0)
                return; // must not happen

            if (CurrentInvoice == invoiceVM) // move currrent object
            {
                if (index < Invoices.Count - 1) // not last, moving down
                    CurrentInvoice = Invoices[index + 1];
                else if (index > 0) // not first, moving up
                    CurrentInvoice = Invoices[index - 1];
                else // first and last means that it's the only item in the list
                    CurrentInvoice = null;
            }
            Invoices.RemoveAt(index);
            RefreshInvoiceCount();
        }

        async Task Edit(InvoiceVM invoiceVM)
        {
            var originalState = invoiceVM.GetStateSnapshot();
            if (await DisplayEdit(invoiceVM))
            {
                await context.SaveChangesAsync();
            }
            else // edit cancelled, need to restore values
            {
                invoiceVM.RestoreState(originalState);
            }
        }

        async Task<bool> DisplayEdit(InvoiceVM invoiceVM)
        {
            try
            {
                CurrentlyEditedInvoice = new InvoiceEditVM(invoiceVM);
                return await CurrentlyEditedInvoice.RunEdit();
            }
            finally
            {
                CurrentlyEditedInvoice = null;
            }
        }

        bool isReady = true;
        public bool IsReady
        {
            get => isReady;
            set => Set(ref isReady, value);
        }

        string lastError;
        public string LastError
        {
            get => lastError;
            set => Set(ref lastError, value);
        }

        InvoiceEditVM currentlyEditedInvoice;
        public InvoiceEditVM CurrentlyEditedInvoice
        {
            get => currentlyEditedInvoice;
            set => Set(ref currentlyEditedInvoice, value);
        }

        readonly SimpleCommand addCommand, deleteCommand, editCommand, filterCommand;
        public ICommand AddCommand => addCommand;
        public ICommand DeleteCommand => deleteCommand;
        public ICommand EditCommand => editCommand;
        public ICommand FilterCommand => filterCommand;

        InvoiceVM currentInvoice;
        public InvoiceVM CurrentInvoice
        {
            get => currentInvoice;
            set
            {
                if (Set(ref currentInvoice, value))
                    UpdateCommandState();
            }
        }

        void UpdateCommandState()
        {
            var hasCurrent = CurrentInvoice != null;
            var isReady = IsReady;
            addCommand.AllowExecute = isReady;
            editCommand.AllowExecute = isReady && hasCurrent;
            deleteCommand.AllowExecute = isReady && hasCurrent;
        }

        string clientFilter = "";
        public string ClientFilter
        {
            get => clientFilter;
            set => Set(ref clientFilter, value);
        }

        bool isUsingFilter;
        public bool IsUsingFilter
        {
            get => isUsingFilter;
            set => Set(ref isUsingFilter, value);
        }

        // simple variant: single InvoiceContext
        InvoiceContext context;
    }
}
