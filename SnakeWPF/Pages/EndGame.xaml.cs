using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SnakeWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для EndGame.xaml
    /// </summary>
    public partial class EndGame : Page
    {
        public EndGame()
        {
            InitializeComponent();
            name.Content = MainWindow.mainWindow.viewModelUserSettings.Name;
            top.Content = MainWindow.mainWindow.ViewModelGames.Top;
            glasses.Content = $"{MainWindow.mainWindow.ViewModelGames.SnakesPlayers.Point.Count - 3} glasses";
            MainWindow.mainWindow.receivingUdpClient.Close();
            MainWindow.mainWindow.tRec.Abort();
            MainWindow.mainWindow.ViewModelGames = null;
        }

        /// <summary>
        /// Переход на начальную старницу 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenHome(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow.OpenPage(MainWindow.mainWindow.Home);
        }
    }
}
