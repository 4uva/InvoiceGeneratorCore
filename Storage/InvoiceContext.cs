using InvoiceGeneratorCore.Model;
using Microsoft.EntityFrameworkCore;

namespace InvoiceGeneratorCore.Storage
{
    class InvoiceContext : DbContext
    {
        public InvoiceContext(DbContextOptions<InvoiceContext> options) : base(options) { }

        public DbSet<Invoice> Invoices { get; set; }
    }
}
