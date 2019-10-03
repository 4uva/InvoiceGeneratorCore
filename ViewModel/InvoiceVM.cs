using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using InvoiceGeneratorCore.Model;

namespace InvoiceGeneratorCore.ViewModel
{
    public class InvoiceVM : VM
    {
        internal InvoiceVM(Invoice invoice) => Invoice = invoice;

        public int Id => Invoice.Id;
        internal void RefreshId() => RaisePropertyChanged(nameof(Id)); // id will be updated after adding to the database

        public DateTime Date
        {
            get => Invoice.Date;
            set { if (Invoice.Date != value) { Invoice.Date = value; RaisePropertyChanged(); } }
        }

        public string Client
        {
            get => Invoice.Client;
            set { if (Invoice.Client != value) { Invoice.Client = value; RaisePropertyChanged(); } }
        }

        public decimal Amount
        {
            get => Invoice.Amount;
            set { if (Invoice.Amount != value) { Invoice.Amount = value; RaisePropertyChanged(); } }
        }

        internal Invoice GetStateSnapshot() =>
            new Invoice()
            {
                Date = Date,
                Client = Client,
                Amount = Amount
            };

        internal void RestoreState(Invoice invoice)
        {
            Date = invoice.Date;
            Client = invoice.Client;
            Amount = invoice.Amount;
        }

        internal readonly Invoice Invoice;
    }
}
