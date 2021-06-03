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
using System.Windows.Threading;

namespace Autosjatek
{
    
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Rectangle> itemRemover = new List<Rectangle>();
        Random rand = new Random();
        ImageBrush jatekosImage = new ImageBrush();
        ImageBrush starImage = new ImageBrush();
        Rect jatekosHitBox;

        int speed = 15;
        int jatekosSpeed = 10;
        int carNum;
        int starCounter = 30;
        int powerModeCounter = 200;

        double eredmeny;
        double i;
        bool moveLeft, moveRight, gameOver, powerMode;

        public MainWindow()
        {
            InitializeComponent();
            myCanvas.Focus();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            StartGame();
        }
        private void GameLoop(object sender, EventArgs e)
        {
            eredmeny += .05;
            starCounter -= 1;
            Eredmény.Content = "Túléled " + eredmeny.ToString("#.#") + " másodperce";
            jatekosHitBox = new Rect(Canvas.GetLeft(jatekos), Canvas.GetTop(jatekos), jatekos.Width, jatekos.Height);
            if (moveLeft == true && Canvas.GetLeft(jatekos) > 0)
            {
                Canvas.SetLeft(jatekos, Canvas.GetLeft(jatekos) - jatekosSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(jatekos) + 90 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(jatekos, Canvas.GetLeft(jatekos) + jatekosSpeed);
            }
            if (starCounter < 1)
            {
                MakeStar();
                starCounter = rand.Next(600, 900);
            }
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "RoadMark")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + speed);
                    if (Canvas.GetTop(x) > 510)
                    {
                        Canvas.SetTop(x, -152);
                    }
                }
                if ((string)x.Tag == "Car")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + speed);
                    if (Canvas.GetTop(x) > 500)
                    {
                        ChangeCars(x);
                    }
                    Rect carHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (jatekosHitBox.IntersectsWith(carHitBox) && powerMode == true)
                    {
                        ChangeCars(x);
                    }
                    else if (jatekosHitBox.IntersectsWith(carHitBox) && powerMode == false)
                    {
                        gameTimer.Stop();
                        Eredmény.Content += " Nyomdd meg az újrajátszáshoz";
                        gameOver = true;
                    }
                }
                if ((string)x.Tag == "star")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 5);
                    Rect starHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (jatekosHitBox.IntersectsWith(starHitBox))
                    {
                        itemRemover.Add(x);
                        powerMode = true;
                        powerModeCounter = 200;
                    }
                    if (Canvas.GetTop(x) > 400)
                    {
                        itemRemover.Add(x);
                    }
                }
            }
            if (powerMode == true)
            {
                powerModeCounter -= 1;
                PowerUp();
                if (powerModeCounter < 1)
                {
                    powerMode = false;
                }
            }
            else
            {
                jatekosImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/jatekosImage.png"));
                myCanvas.Background = Brushes.Gray;
            }
            foreach (Rectangle y in itemRemover)
            {
                myCanvas.Children.Remove(y);
            }
            if (eredmeny >= 10 && eredmeny < 20)
            {
                speed = 12;
            }

            if (eredmeny >= 20 && eredmeny < 30)
            {
                speed = 14;
            }
            if (eredmeny >= 30 && eredmeny < 40)
            {
                speed = 16;
            }
            if (eredmeny >= 40 && eredmeny < 50)
            {
                speed = 18;
            }
            if (eredmeny >= 50 && eredmeny < 80)
            {
                speed = 22;
            }
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right)
            {
                moveRight = true;
            }
        }
        private void OnKeyUP(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right)
            {
                moveRight = false;
            }
            if (e.Key == Key.Enter && gameOver == true)
            {
                StartGame();
            }
        }
        private void StartGame()
        {
            speed = 8;
            gameTimer.Start();
            moveLeft = false;
            moveRight = false;
            gameOver = false;
            powerMode = false;

            eredmeny = 0;
            Eredmény.Content = "Túléltél: 0 másodpercet  ";
            jatekosImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/jatekosImage.png"));
            starImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/star.png"));
            jatekos.Fill = jatekosImage;
            myCanvas.Background = Brushes.Gray;
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "Car")
                {
                    Canvas.SetTop(x, (rand.Next(100, 400) * -1));
                    Canvas.SetLeft(x, rand.Next(0, 430));
                    ChangeCars(x);
                }
                if ((string)x.Tag == "star")
                {
                    itemRemover.Add(x);
                }
            }
            itemRemover.Clear();
        }
        private void ChangeCars(Rectangle car)
        {
            carNum = rand.Next(1, 6);
            ImageBrush carImage = new ImageBrush();
            switch (carNum)
            {
                case 1:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/kocsi1.png"));
                    break;
                case 2:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/kocsi2.png"));
                    break;
                case 3:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/kocsi3.png"));
                    break;
                case 4:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/kocsi4.png"));
                    break;
                case 5:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/kocsi5.png"));
                    break;
                case 6:
                    carImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/kocsi6.png"));
                    break;
            }
            car.Fill = carImage;
            Canvas.SetTop(car, (rand.Next(100, 400) * -1));
            Canvas.SetLeft(car, rand.Next(0, 430));
        }
        private void PowerUp()
        {
            i += .5;
            if (i > 4)
            {
                i = 1;
            }
            switch (i)
            {
                case 1:
                    jatekosImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/powermode1.png"));
                    break;
                case 2:
                    jatekosImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/powermode2.png"));
                    break;
                case 3:
                    jatekosImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/powermode3.png"));
                    break;
                case 4:
                    jatekosImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/powermode4.png"));
                    break;
            }
            myCanvas.Background = Brushes.LightCoral;
        }
        private void MakeStar()
        {
            Rectangle newStar = new Rectangle
            {
                Height = 50,
                Width = 50,
                Tag = "star",
                Fill = starImage
            };
            Canvas.SetLeft(newStar, rand.Next(0, 430));
            Canvas.SetTop(newStar, (rand.Next(100, 400) * -1));
            myCanvas.Children.Add(newStar);
        }
    }
}
