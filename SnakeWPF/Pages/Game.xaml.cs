using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SnakeWPF.Pages
{
    /// <summary>
    /// Логика взаимодействия для Game.xaml
    /// </summary>
    public partial class Game : Page
    {
        public int StepCadr = 0;

        private readonly Color[] headColors = new Color[] {
            Color.FromArgb(255, 0, 127, 14),  
            Color.FromArgb(255, 255, 0, 0),   
            Color.FromArgb(255, 0, 0, 255),   
            Color.FromArgb(255, 255, 165, 0), 
            Color.FromArgb(255, 128, 0, 128), 
            Color.FromArgb(255, 0, 255, 255)  
        };

        private readonly Color[] bodyColors = new Color[] {
            Color.FromArgb(255, 0, 198, 19),   
            Color.FromArgb(255, 255, 100, 100),
            Color.FromArgb(255, 100, 100, 255),
            Color.FromArgb(255, 255, 200, 100),
            Color.FromArgb(255, 200, 100, 200),
            Color.FromArgb(255, 100, 255, 255) 
        };

        public Game()
        {
            InitializeComponent();
        }

        public void CreateUI()
        {
            Dispatcher.Invoke(() =>
            {
                if (StepCadr == 0) StepCadr = 1;
                else StepCadr = 0;
                canvas.Children.Clear();
                var allSnakes = MainWindow.mainWindow.ViewModelGames.AllSnakes;
                if (allSnakes == null) return;

                foreach (var snake in allSnakes)
                {
                    int colorIndex = Math.Abs(snake.IdSnake) % headColors.Length;
                    Brush currentHeadColor = new SolidColorBrush(headColors[colorIndex]);
                    Brush currentBodyColor = new SolidColorBrush(bodyColors[colorIndex]);

                    for (int iPoint = snake.Points.Count - 1; iPoint >= 0; iPoint--)
                    {
                        int drawX = snake.Points[iPoint].X;
                        int drawY = snake.Points[iPoint].Y;

                        if (iPoint != 0)
                        {
                            int nextX = snake.Points[iPoint - 1].X;
                            int nextY = snake.Points[iPoint - 1].Y;

                            if (drawX > nextX || drawX < nextX)
                            {
                                if (iPoint % 2 == 0) drawY += (StepCadr % 2 == 0) ? -1 : 1;
                                else drawY += (StepCadr % 2 == 0) ? 1 : -1;
                            }
                            else if (drawY > nextY || drawY < nextY)
                            {
                                if (iPoint % 2 == 0) drawX += (StepCadr % 2 == 0) ? -1 : 1;
                                else drawX += (StepCadr % 2 == 0) ? 1 : -1;
                            }
                        }

                        Brush fillColor = (iPoint == 0) ? currentHeadColor : currentBodyColor;

                        Ellipse ellipse = new Ellipse()
                        {
                            Width = 20,
                            Height = 20,
                            Margin = new Thickness(drawX - 10, drawY - 10, 0, 0),
                            Fill = fillColor,
                            Stroke = Brushes.Black
                        };
                        canvas.Children.Add(ellipse);
                    }
                }

                ImageBrush myBrush = new ImageBrush();
                myBrush.ImageSource = new BitmapImage(new Uri("C:\\Users\\msi\\source\\repos\\Zmeya\\SnakeWPF\\Image\\apple.png", UriKind.Relative));
                Ellipse points = new Ellipse()
                {
                    Width = 40,
                    Height = 40,
                    Margin = new Thickness(
                        MainWindow.mainWindow.ViewModelGames.Points.X - 20,
                        MainWindow.mainWindow.ViewModelGames.Points.Y - 20, 0, 0),
                    Fill = myBrush
                };
                canvas.Children.Add(points);
            });
        }
    }
}
