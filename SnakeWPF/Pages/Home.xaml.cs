using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace SnakeWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private MainWindow _mainWindow;
        public Home()
        {
            InitializeComponent();
            _mainWindow = MainWindow.mainWindow;
        }

        private void StartGame(object sender, RoutedEventArgs e)
        {
            if (_mainWindow == null) return;

            if (_mainWindow.receivingUdpClient != null)
                _mainWindow.receivingUdpClient.Close();

            if (_mainWindow.tRec != null)
                _mainWindow.tRec.Abort();

            IPAddress UserIPAddress;

            if (!IPAddress.TryParse(ip.Text, out UserIPAddress))
            {
                MessageBox.Show("Please use the IP address in the format X.X.X.X.");
                return;
            }

            int UserPort;

            if (!int.TryParse(port.Text, out UserPort))
            {
                MessageBox.Show("Please use the port as a number.");
                return;
            }

            _mainWindow.StartReceiver();
            _mainWindow.viewModelUserSettings.IPAdress = ip.Text;
            _mainWindow.viewModelUserSettings.Port = port.Text;
            _mainWindow.viewModelUserSettings.Name = name.Text;
            MainWindow.Send("/start|" + JsonConvert.SerializeObject(_mainWindow.viewModelUserSettings));
        }
    }
}
