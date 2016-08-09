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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameTutorial
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Rectangle rect;         //创建一个方块作为演示对象
        double speed = 20;      //设置移动速度
        double speedX = 1;
        double speedY = 1;
        Point moveTo;           //设置移动目标
        bool moveFlag = false;  // 为false时表示不用移动

        public MainWindow()
        {
            InitializeComponent();

            rect = new Rectangle();
            rect.Fill = new SolidColorBrush(Colors.Green);
            rect.Width = 50;
            rect.Height = 50;
            rect.RadiusX = 5;
            rect.RadiusY = 5;
            Carrier.Children.Add(rect);
            Canvas.SetLeft(rect, 0);
            Canvas.SetTop(rect, 0);

            //定义线程
            DispatcherTimer dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Tick += new EventHandler(Timer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(10); //重复间隔
            dispatcherTimer.Start();
        }

        private void Carrier_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveTo = e.GetPosition(Carrier);

            speedX = speed * Math.Cos(Math.Atan2(Math.Abs(moveTo.Y - Canvas.GetTop(rect)), Math.Abs(moveTo.X - Canvas.GetLeft(rect))));
            speedY = speed * Math.Sin(Math.Atan2(Math.Abs(moveTo.Y - Canvas.GetTop(rect)), Math.Abs(moveTo.X - Canvas.GetLeft(rect))));
            moveFlag = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double rect_X = Canvas.GetLeft(rect);
            double rect_Y = Canvas.GetTop(rect);

            if (moveFlag)
            {
                Canvas.SetLeft(rect, rect_X + (rect_X < moveTo.X ? speedX : -speedX));
                Canvas.SetTop(rect, rect_Y + (rect_Y < moveTo.Y ? speedY : -speedY));
            }
            else
            {
                Canvas.SetLeft(rect, moveTo.X);
                Canvas.SetTop(rect, moveTo.Y);
            }
            
            if ((Math.Abs(rect_X - moveTo.X) <= speed) && (Math.Abs(rect_Y - moveTo.Y) <= speed))
            {
                moveFlag = false;
            }
        }
    }
}
