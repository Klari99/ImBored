using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

    //todo: csak ezen az oldalon hallhassa a keyeventet
    //todo: timer startkor induljon el,
    //todo: highscore and score load/save
{
    public sealed partial class GameComponent : StackPanel
    {
        DispatcherTimer timer;
        Random random = new Random();

        private bool isRunning = false;
        private int speed = 5;
        private double jumpHeight = 150;
        private double playerFloorHeight = 225;
        private int currentTime = 0;
        private int nextObstacleSpawningTime = 10;

        Ellipse player = null;

        //todo: make it concurrent
        //ConcurrentDictionary<int, Rectangle> obstacles = new ConcurrentDictionary<int, Rectangle>(2, 50);
        //TryAdd(nextObstacleSpawningTime, rectangle)
        //TryRemove(nextObstacleSpawningTime, rectangle)
        //foreach (var obstacle in obstacles)
        List<Rectangle> obstacles = new List<Rectangle>();

        private bool isJumping = false;
        private bool isFalling = false;
        public GameComponent()
        {
            this.InitializeComponent();
            drawGameArea();
            drawPlayer(50, playerFloorHeight);

            Window.Current.CoreWindow.KeyDown += KeyDown_Event;
        }

        private void drawGameArea()
        {
            gameCanvas.Children.Clear();

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

        private void KeyDown_Event(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            if(!isRunning)
            {
                startGame();
            }
            jump();
        }

        private void startGame()
        {
            Score = 0;
            isRunning = true;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void stopGame()
        {
            isRunning = false;
            //todo: reset game
            timer.Stop();
        }

        private void Timer_Tick(object sender, object e)
        {
            //todo: clear canvas and make a draw method
            

            foreach (Rectangle obstacle in obstacles)
            {
                //todo: collision
            }

            currentTime++;
            Score = currentTime;

            if(currentTime == nextObstacleSpawningTime)
            {

                //todo: separe this to a method
                nextObstacleSpawningTime = currentTime + random.Next(30, 80);
                Rectangle obstacle = new Rectangle();
                obstacle.Width = 30;
                obstacle.Height = 30;

                obstacle.Fill = new SolidColorBrush(Colors.White);
                Canvas.SetLeft(obstacle, 450);
                Canvas.SetTop(obstacle, 220);
                gameCanvas.Children.Add(obstacle);
                obstacles.Add(obstacle);
            }

            foreach (Rectangle obstacle in obstacles)
            {
                moveLeft(obstacle);
            }

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
            if(obstacle.ActualOffset.X + obstacle.Width <= 0)
            {
                //obstacles.Remove(obstacle);
                return;
            }

            Canvas.SetLeft(obstacle, obstacle.ActualOffset.X - speed);
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
    }
}
