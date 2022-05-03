using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

namespace ImBored
{
    public sealed partial class GameComponent : StackPanel
    {
        DispatcherTimer timer;
        Random random = new Random();

        private bool isRunning = false;
        private int speed = 4;
        private double jumpHeight = 150;
        private double playerFloorHeight = 225;
        private int currentTime = 0;
        private int nextObstacleSpawningTime = 10;

        Ellipse player = null;
        List<Rectangle> obstacles = new List<Rectangle>();

        private bool isJumping = false;
        private bool isFalling = false;
        public GameComponent()
        {
            this.InitializeComponent();
            drawGameArea();
            drawPlayer(50, playerFloorHeight);
            loadHighScore();

            Window.Current.CoreWindow.KeyDown += KeyDown_Event;
        }

        private void drawGameCanvas()
        {
            drawGameArea();
            drawPlayer(player.ActualOffset.X, player.ActualOffset.Y);
            drawObstacles();
        }

        private void drawGameArea()
        {
            Rectangle frame = new Rectangle();
            frame.Width = 450;
            frame.Height = 300;

            frame.Fill = new SolidColorBrush(Colors.Black);
            frame.StrokeThickness = 1;
            frame.Stroke = new SolidColorBrush(Colors.DarkGray);
            Canvas.SetLeft(frame, 0);
            Canvas.SetTop(frame, 0);
            gameCanvas.Children.Add(frame);

            Line line = new Line();
            line.X1 = 0;
            line.X2 = 450;
            line.Y1 = 250;
            line.Y2 = 250;

            line.Stroke = new SolidColorBrush(Colors.White);
            line.StrokeThickness = 2;
            Canvas.SetZIndex(line, 1);

            gameCanvas.Children.Add(line);
        }

        private void drawPlayer(double left, double top)
        {
            if (player == null)
            {
                player = new Ellipse();
                player.Width = 25;
                player.Height = 25;
            }
            
            player.Fill = new SolidColorBrush(Colors.White);
            Canvas.SetLeft(player, left);
            Canvas.SetTop(player, top);
            gameCanvas.Children.Add(player);
        }

        private void drawObstacles()
        {
            foreach (Rectangle obstacle in obstacles)
            {
                gameCanvas.Children.Add(obstacle);
            }
        }

        public void KeyDown_Event(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {   
            if(this.Visibility == Visibility.Visible)
            {
                if (!isRunning)
                {
                    startGame();
                }
                jump();
            }
        }

        private void startGame()
        {
            if (this.Visibility == Visibility.Visible)
            { 
                Score = 0;
                isRunning = true;
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        private void stopGame()
        {
            if(isRunning && this.Visibility == Visibility.Visible)
            {
                isRunning = false;
                timer.Stop();
                handleHighScore();
                showScore();
                resetGame();
            }
            else
            {
                timer.Stop();
                resetGame();
            }
        }

        private void resetGame()
        {
            isRunning = false;
            currentTime = 0;
            nextObstacleSpawningTime = 10;
            Score = 0;
            gameCanvas.Children.Clear();
            drawGameArea();
            drawPlayer(50, playerFloorHeight);
            obstacles.Clear();
        }

        private async void showScore()
        {
            ContentDialog cd = new ContentDialog()
            {
                CloseButtonText = "OK",
                Content = "Your score: " + Score.ToString(),
                Title = "GAME OVER"
            };
            await cd.ShowAsync();
        }

        private void handleHighScore()
        {
            if(Score > HighScore)
            {
                HighScore = Score;
                saveHighScore();
            }
        }        

        private Task loadHighScore()
        {
            ApplicationDataContainer ls = ApplicationData.Current.LocalSettings;
            if (ls.Values.ContainsKey("HighScore"))
            {
                HighScore = int.Parse(ls.Values["HighScore"].ToString());
            }
            else
            {
                HighScore = 0;
            }

            return Task.CompletedTask;
        }

        private void saveHighScore()
        {
            ApplicationDataContainer ls = ApplicationData.Current.LocalSettings;
            ls.Values["HighScore"] = HighScore;
        }

        private void Timer_Tick(object sender, object e)
        {
            currentTime++;
            Score = currentTime;

            if (obstacles.Any(obstacle => collidesWithPlayer(obstacle)) || this.Visibility != Visibility.Visible)
            {
                stopGame();
            }

            obstacles.RemoveAll(obstacle => obstacle.ActualOffset.X <= 0);

            gameCanvas.Children.Clear();
            drawGameCanvas();

            if(currentTime == nextObstacleSpawningTime)
            {
                nextObstacleSpawningTime = currentTime + random.Next(30, 90);
                spawnObstacle();
            }

            foreach (Rectangle obstacle in obstacles)
            {
                moveLeft(obstacle);
            }

            //player movement
            if (isJumping && player.ActualOffset.Y > jumpHeight)
            {
                Canvas.SetTop(player, player.ActualOffset.Y - speed);
            }
            else if(isJumping && player.ActualOffset.Y <= jumpHeight)
            {
                isJumping = false;
                isFalling = true;
            }

            if (isFalling && player.ActualOffset.Y < playerFloorHeight)
            {
                Canvas.SetTop(player, player.ActualOffset.Y + speed);
            }
            else if (isFalling && player.ActualOffset.Y >= playerFloorHeight)
            {
                isFalling = false;
            }
        }

        private void jump()
        {
            if (isRunning && !isJumping && !isFalling)
            {
                isJumping = true;
            }
        }

        private void moveLeft(Rectangle obstacle)
        {
            Canvas.SetLeft(obstacle, obstacle.ActualOffset.X - speed);
        }

        private bool collidesWithPlayer(Rectangle obstacle)
        {
            Rectangle playerRect = new Rectangle();
            playerRect.Width = player.Width;
            playerRect.Height = player.Height;

            Canvas.SetLeft(playerRect, player.ActualOffset.X);
            Canvas.SetTop(playerRect, player.ActualOffset.Y);

            if (((playerRect.ActualOffset.X < (obstacle.ActualOffset.X + obstacle.Width)) && (obstacle.ActualOffset.X < (playerRect.ActualOffset.X + playerRect.Width))) && (playerRect.ActualOffset.Y < (obstacle.ActualOffset.Y + obstacle.Height)) && (obstacle.ActualOffset.Y < (playerRect.ActualOffset.Y + playerRect.Height)))
            {
                Vector2 rect1Centre = new Vector2((float)(playerRect.ActualOffset.X + playerRect.Width / 2), (float)(playerRect.ActualOffset.Y + playerRect.Height / 2));
                Vector2 rect2Centre = new Vector2((float)(obstacle.ActualOffset.X + obstacle.Width / 2), (float)(obstacle.ActualOffset.Y + playerRect.Height / 2));
                double radius1 = ((playerRect.Width / 2) + (playerRect.Height / 2)) / 2;
                double radius2 = ((obstacle.Width / 2) + (obstacle.Height / 2)) / 2;

                double widthTri = rect1Centre.X - rect2Centre.X;
                double heightTri = rect1Centre.Y - rect2Centre.Y;
                double distance = Math.Sqrt(Math.Pow(widthTri, 2) + Math.Pow(heightTri, 2));

                if (distance <= (radius1 + radius2))
                    return true;
            }
            return false;
        }

        private void spawnObstacle()
        {
            Rectangle obstacle = new Rectangle();
            obstacle.Width = 30;
            obstacle.Height = 30;

            obstacle.Fill = new SolidColorBrush(Colors.White);
            Canvas.SetLeft(obstacle, 420);
            Canvas.SetTop(obstacle, 220);
            gameCanvas.Children.Add(obstacle);
            obstacles.Add(obstacle);
        }

        public static readonly DependencyProperty TimeProperty =
                DependencyProperty.Register(nameof(Time), typeof(TimeSpan),
                typeof(GameComponent), new PropertyMetadata(new TimeSpan(0, 0, 50)));
        public TimeSpan Time
        {
            get
            {
                return (TimeSpan)GetValue(TimeProperty);
            }
            set
            {
                SetValue(TimeProperty, value);
            }
        }

        public int Score
        {
            get { return (int)GetValue(ScoreProperty); }
            set { SetValue(ScoreProperty, value); }
        }

        public static readonly DependencyProperty ScoreProperty =
            DependencyProperty.Register("Score", typeof(int),
                typeof(GameComponent), null);

        public int HighScore
        {
            get { return (int)GetValue(HighScoreProperty); }
            set { SetValue(HighScoreProperty, value); }
        }

        public static readonly DependencyProperty HighScoreProperty =
            DependencyProperty.Register("HighScore", typeof(int),
                typeof(GameComponent), null);
    }
}
