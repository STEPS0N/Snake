using Newtonsoft.Json;
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
            if (_mainWindow == null)
            {
                _mainWindow = MainWindow.mainWindow;
            }    

            _mainWindow.viewModelUserSettings.IPAddress = ip.Text;
            _mainWindow.viewModelUserSettings.Port = port.Text;
            _mainWindow.viewModelUserSettings.Name = name.Text;

            if (!IPAddress.TryParse(_mainWindow.viewModelUserSettings.IPAddress, out _))
            {
                MessageBox.Show("Неверный формат IP адреса. Используйте формат X.X.X.X");
                return;
            }
            if (!int.TryParse(_mainWindow.viewModelUserSettings.Port, out _))
            {
                MessageBox.Show("Порт должен быть числом.");
                return;
            }

            if (_mainWindow.receivingUdpClient != null)
            {
                _mainWindow.receivingUdpClient.Close();
            }
            if (_mainWindow.tRec != null && _mainWindow.tRec.IsAlive)
            {
                _mainWindow.tRec.Join(100);
            }

            _mainWindow.StartReceiver();

            MainWindow.Send("/start|" + Newtonsoft.Json.JsonConvert.SerializeObject(_mainWindow.viewModelUserSettings));
        }
    }
}
