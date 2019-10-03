using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using InvoiceGeneratorCore.Storage;
using System.IO;
using System.Reflection;

namespace InvoiceGeneratorCore
{
    class InvoiceContextFactory : IDesignTimeDbContextFactory<InvoiceContext>
    {
        public DbContextOptions<InvoiceContext> CreateOptions()
        {
            //var connectionString = ConfigurationManager.ConnectionStrings["InvoiceDatabase"].ConnectionString;
            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            var connectionString = configuration.GetConnectionString("InvoiceDatabase");

            var optionsBuilder = new DbContextOptionsBuilder<InvoiceContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return optionsBuilder.Options;
        }
        public InvoiceContext CreateDbContext(string[] args) => new InvoiceContext(CreateOptions());
    }

}
