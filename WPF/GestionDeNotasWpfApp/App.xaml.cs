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

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // use the Dependency Injection pattern in order to have decoupled code
            // with this way, we can implement Unit Test for the FrmNota using a Mock object (or a very simple implementation) of INoteService

            // Real service
            INoteService service = new NoteServiceWrapper(_serviceBaseURL);
            /*// Use this line of code if for testing the form with a service simulator or a Mock object
            INoteService service = new NoteServiceSimulator();*/

            MainWindow frm = new MainWindow(service);
            frm.ShowDialog();
        }

    }

}
