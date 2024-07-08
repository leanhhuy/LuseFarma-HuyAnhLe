using GestionDeNotas.BusinessLogic;
using System.Configuration;
using System.Data;
using System.Windows;

namespace GestionDeNotasWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // TODO: load from configuration file
        private string _serviceBaseURL = "http://localhost:3000/";

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            // We use the Dependency Injection pattern in order to have decoupled code
            // By this way, we can implement Unit Test for the FrmNota using a Mock object (or a very simple implementation) of service interface INoteService

            // Real service
            INoteService service = new NoteServiceWrapper(_serviceBaseURL);

            // Instead of real service, we can test the form with a Service Simulator
            // (o any Mock object or implementation of the interface INoteSerivce)
            /*INoteService service = new NoteServiceSimulator();*/

            MainWindow frm = new MainWindow(service);
            frm.ShowDialog();
        }

    }

}
