using System.Windows;

using Microsoft.EntityFrameworkCore;

using InvoiceGeneratorCore.ViewModel;
using InvoiceGeneratorCore.View;
using InvoiceGeneratorCore.Storage;

namespace InvoiceGeneratorCore
{
    public partial class App : Application
    {
        MainVM mainVM;
        DbContextOptions<InvoiceContext> dbOptions = new InvoiceContextFactory().CreateOptions();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            mainVM = new MainVM(dbOptions);
            var mainWindow = new MainWindow() { DataContext = mainVM };
            mainWindow.Show();
        }
    }
}
