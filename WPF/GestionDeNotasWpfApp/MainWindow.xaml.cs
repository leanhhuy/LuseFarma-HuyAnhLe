using GestionDeNotas.BusinessLogic;
using System.ComponentModel;

//using GestionDeNotasWpfApp.BusinessLogic;
using System.Net;
using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Security.Policy;
//using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GestionDeNotasWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private INoteService _noteService;
                
        //private const int CHECK_SERVICE_PER_MILLISECONDS = 5 * 1000; // 5 seconds
        private const int CHECK_SERVICE_PER_MILLISECONDS = 2 * 60 * 1000; // 2 minutes

        /* ---------------------------------------------------------------------------------
         * In principle, all necessary dependencies will injected through the constructor
         * Dependencies should be interfaces, so that we can test the behavior of UI easily using Mock or simple implementations
         --------------------------------------------------------------------------------- */

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(INoteService noteMgmt)
        {
            InitializeComponent();

            this._noteService = noteMgmt;

            lblServiceOn.Content = string.Empty;
            lblServiceOff.Content = string.Empty;

            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        #region check service availability
        private void Worker_DoWork(object? sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread thread = new Thread(new ThreadStart(CheckServiceAvailableInThread));
                thread.Start();
                Thread.Sleep(CHECK_SERVICE_PER_MILLISECONDS);
            }
        }

        private async void CheckServiceAvailableInThread()
        {
            /* Test history 
             * 2024 -07-08: test with the frequency of 5 seconds
             */
            bool available = await _noteService.CheckServiceAvailable();
            if (available)
            {
                lblServiceOn.Dispatcher.Invoke(() =>
                {
                    //lblServiceOn.Content = DateTime.Now.ToString() + ": " + "Servicio disponible.";
                    lblServiceOn.Content = "Servicio disponible.";
                });
                lblServiceOff.Dispatcher.Invoke(() =>
                {
                    lblServiceOff.Content = string.Empty;
                });
            }
            else
            {
                /*lblServiceOn.Dispatcher.Invoke(() =>
                {
                    lblServiceOn.Content = string.Empty;
                });
                lblServiceOff.Dispatcher.Invoke(() =>
                {
                    lblServiceOff.Content = DateTime.Now.ToString() + ": " + "Servicio no disponible. Inténtelo más tarde.";
                });*/
                this.Dispatcher.Invoke(() =>
                {
                    lblServiceOn.Content = string.Empty;
                    lblServiceOff.Content = DateTime.Now.ToString() + ": " + "Servicio no disponible. Inténtelo más tarde.";
                    AlertFailedService();
                });
            }
        }

        private void AlertFailedService()
        {
            MessageBox.Show("Servicio no disponible. Inténtelo más tarde.", "Servicio no disponible",
                MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None, MessageBoxOptions.ServiceNotification);
        }
        #endregion

        #region Obligatorio
        private void clearLabelNota()
        {
            lblNotaMsg.Content = string.Empty;
            lblNotaErr.Content = string.Empty;
        }

        private async void btnGuardarNota_Click(object sender, RoutedEventArgs e)
        {
            /* Test history 
             * 2024 -07-08: with service OFF
             * 2024 -07-08: with service ON
             */

            clearLabelNota();

            if (txtNota.Text.Trim() != string.Empty)
            {
                string note = txtNota.Text.Trim();
                string? savedNote = await _noteService.SaveNoteAsync(note);
                if (savedNote != null)
                {
                    // Reason: the note can be modified (TRIM, UPPER, LOWER...) by the service before being saved
                    txtNota.Text = savedNote;

                    lblNotaMsg.Content = "Nota guardada correctamente!";
                }
                else
                {
                    lblNotaErr.Content = "ERROR! Inténtelo mas tardes.";
                    AlertFailedService();

                    // We can also check if the service is not available or this is another error
                    /*bool available = await _noteService.CheckServiceAvailable();*/
                }
            }
            else
            {
                lblNotaErr.Content = "Por favor, introduzca una nota.";
                txtNota.Focus();
            }
        }

        private async void btnLeerNota_Click(object sender, RoutedEventArgs e)
        {
            /* Test history 
             * * 2024 -07-08: with service OFF
             * 2024 -07-08: with service ON
             */

            clearLabelNota();

            string? note = await _noteService.ReadNoteAsync();
            if (note != null)
            {
                txtNota.Text = note;
                lblNotaMsg.Content = "Nota leída correctamente!";
            }
            else
            {
                lblNotaErr.Content = "ERROR! Inténtelo mas tardes.";
                AlertFailedService();

                // We can also check if the service is not available or this is another error
                /*bool available = await _noteService.CheckServiceAvailable();*/
            }
        }

        private async void btnBorrarNota_Click(object sender, RoutedEventArgs e)
        {
            /* Test history 
             * 2024 -07-08: with service OFF
             * 2024 -07-08: with service ON
             */

            clearLabelNota();

            bool deleted = await _noteService.DeleteNoteAsync();
            if (deleted)
            {
                txtNota.Text = string.Empty;
                lblNotaMsg.Content = "Nota borrado correctamente!";
            }
            else
            {
                lblNotaErr.Content = "ERROR! Inténtelo mas tardes.";
                AlertFailedService();

                // We can also check if the service is not available or this is another error
                /*bool available = await _noteService.CheckServiceAvailable();*/
            }
        }
        #endregion

        #region Opcional
        private void clearLabelNotaOpc()
        {
            lblNotaOpcMsg.Content = string.Empty;
            lblNotaOpcErr.Content = string.Empty;
        }

        private async void btnGuardarNotaOpcional_Click(object sender, RoutedEventArgs e)
        {
            /* Test history 
             * 2024 -07-08: with service OFF
             * 2024 -07-08: with service ON
             */

            clearLabelNotaOpc();

            if (txtNotaOpcional.Text.Trim().Length > 0)
            {
                string note = txtNotaOpcional.Text.Trim();
                string? savedNote = await _noteService.AppendNoteAsync(note);

                if (savedNote != null)
                {
                    // Reason: the note can be modified (TRIM, UPPER, LOWER...) by the service
                    txtNotaOpcional.Text = savedNote;

                    lblNotaOpcMsg.Content = "Nota guardada correctamente!";
                }
                else
                {
                    lblNotaOpcErr.Content = "ERROR! Inténtelo mas tardes.";
                    AlertFailedService();

                    // We can also check if the service is not available or this is another error
                    /*bool available = await _noteService.CheckServiceAvailable();*/
                }
            }
            else
            {
                lblNotaOpcErr.Content = "Por favor, introduzca una nota.";
                txtNotaOpcional.Focus();
            }
        }

        private async void btnLeerNotaOpcional_Click(object sender, RoutedEventArgs e)
        {
            /* Test history 
             * 2024 -07-08: with service OFF
             * 2024 -07-08: with service ON
             */

            clearLabelNotaOpc();

            string[]? notes = await _noteService.ReadNotesAsync();
            if (notes != null)
            {
                lblNotaOpcMsg.Content = "Nota leída correctamente!";

                if (notes.Length > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    for (int i = 0; i < notes.Length; i++)
                    {
                        if (i > 0)
                        {
                            stringBuilder.Append(Environment.NewLine);
                        }
                        string index = (i + 1).ToString();
                        string note = notes[i];
                        stringBuilder.Append($"- Nota {index}: {note}.");
                    }
                    MessageBox.Show(stringBuilder.ToString());
                }
                else
                {
                    MessageBox.Show("No hay nota.");
                }
            }
            else
            {
                lblNotaOpcErr.Content = "ERROR! Inténtelo mas tardes.";
                AlertFailedService();

                // We can also check if the service is not available or this is another error
                /*bool available = await _noteService.CheckServiceAvailable();*/
            }
        }

        private async void btnBorrarNotaOpcional_Click(object sender, RoutedEventArgs e)
        {
            /* Test history 
             * 2024 -07-08: with service OFF
             * 2024 -07-08: with service ON
             */

            clearLabelNotaOpc();

            bool deleted = await _noteService.ClearNotesAsync();
            if (deleted)
            {
                txtNotaOpcional.Text = string.Empty;
                lblNotaOpcMsg.Content = "Notas borradas correctamente!";
            }
            else
            {
                lblNotaOpcErr.Content = "ERROR! Inténtelo mas tardes.";
                AlertFailedService();

                // We can also check if the service is not available or this is another error
                /*bool available = await _noteService.CheckServiceAvailable();*/
            }
        }
        #endregion

    }
}