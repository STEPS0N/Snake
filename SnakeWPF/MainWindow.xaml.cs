using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace SnakeWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary> Главное окно, используется для общения между страницами </summary>
        public static MainWindow mainWindow;
        /// <summary> Модель данных при передачи IP адреса устройства, порта и никнейма </summary>
        public ViewModelUserSettings viewModelUserSettings = new ViewModelUserSettings();
        /// <summary> Модель игрока в который передаются координаты змеи, яблок и точки ... </summary>
        public ViewModelGames ViewModelGames = null;
        /// <summary> Удаленный IP адрес для подключения к серверу </summary>
        public static IPAddress remoteIPAddress = IPAddress.Parse("127.0.0.1");
        /// <summary> Удаленный порт для подключения к серверу  </summary>
        public static int remotePort = 5001;
        /// <summary> Основной поток для получения данных о игре  </summary>
        public Thread tRec;
        /// <summary> UDP клиент для получения данных </summary>
        public UdpClient receivingUdpClient;
        /// <summary> Страница HOME </summary>
        public Pages.Home Home = new Pages.Home();
        /// <summary> Страница GAME </summary>
        public Pages.Game Game = new Pages.Game();

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            OpenPage(Home);
        }

        /// <summary>
        /// Начинаем случать ответы от сервера
        /// </summary>
        public void StartReceiver()
        {
            tRec = new Thread(new ThreadStart(Receiver));
            tRec.Start();
        }

        public void OpenPage(Page PageOpen)
        {
            DoubleAnimation startAnimation = new DoubleAnimation();
            startAnimation.From = 1;
            startAnimation.To = 0;
            startAnimation.Duration = TimeSpan.FromSeconds(0.6);
            startAnimation.Completed += delegate
            {
                frame.Navigate(PageOpen);
                DoubleAnimation endAnimation = new DoubleAnimation();
                endAnimation.From = 0;
                endAnimation.To = 1;
                endAnimation.Duration = TimeSpan.FromSeconds(0.6);
                frame.BeginAnimation(OpacityProperty, endAnimation);
            };
            frame.BeginAnimation(OpacityProperty, startAnimation);
        }

        public void Receiver()
        {
            receivingUdpClient = new UdpClient(int.Parse(viewModelUserSettings.Port));
            IPEndPoint RemouteIpEndPoint = null;

            try
            {
                while (true)
                {
                    byte[] receiveBytes = receivingUdpClient.Receive(
                        ref RemouteIpEndPoint);

                    string returnData = Encoding.UTF8.GetString(receiveBytes);
                    if (ViewModelGames == null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            OpenPage(Game);
                        });
                    }

                    ViewModelGames = JsonConvert.DeserializeObject<ViewModelGames>(returnData.ToString());

                    if (ViewModelGames.SnakesPlayers.GameOver)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            OpenPage(new Pages.EndGame());
                        });
                    }
                    else
                    {
                        Game.CreateUI();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Возникло исключение: " + ex.ToString() + "\n " + ex.Message);
            }
        }

        public static void Send(string datagram)
        {
            UdpClient sender = new UdpClient();
            IPEndPoint endPoint = new IPEndPoint(remoteIPAddress, remotePort);
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(datagram);
                sender.Send(bytes, bytes.Length, endPoint);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Возникло исключение: " + ex.ToString() + "\n " + ex.Message);
            }
            finally
            {
                sender.Close();
            }
        }

        private void EventKeyUp(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(viewModelUserSettings.IPAddress) && !string.IsNullOrEmpty(viewModelUserSettings.Port) && (ViewModelGames != null && !ViewModelGames.SnakesPlayers.GameOver))
            {
                if (e.Key == Key.Up)
                    Send($"Up|{JsonConvert.SerializeObject(viewModelUserSettings)}");
                else if (e.Key == Key.Down)
                    Send($"Down|{JsonConvert.SerializeObject(viewModelUserSettings)}");
                else if (e.Key == Key.Left)
                    Send($"Left|{JsonConvert.SerializeObject(viewModelUserSettings)}");
                else if (e.Key == Key.Right)
                    Send($"Right|{JsonConvert.SerializeObject(viewModelUserSettings)}");
            }
        }

        private void QuitApplication(object sender, System.ComponentModel.CancelEventArgs e)
        {
            receivingUdpClient.Close();
            tRec.Abort();
        }
    }
}
