using System;
using System.Collections.Generic;
using System.Text;

namespace InvoiceGeneratorCore.Model
{
    class Invoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Client { get; set; } // use another entity for client?
        public decimal Amount { get; set; }
    }
}
