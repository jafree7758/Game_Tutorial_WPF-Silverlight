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
        //Rectangle rect;         //创建一个方块作为演示对象
        //double speed = 20;      //设置移动速度
        //double speedX = 1;
        //double speedY = 1;
        //Point moveTo;           //设置移动目标
        //bool moveFlag = false;  // 为false时表示不用移动

        int count = 1;
        Image Spirit;

        public MainWindow()
        {
            InitializeComponent();

            Spirit = new Image();
            Spirit.Width = 150;
            Spirit.Height = 150;
            Carrier.Children.Add(Spirit);

            Canvas.SetLeft(Spirit, 320);
            Canvas.SetTop(Spirit, 220);

            //定义线程
            DispatcherTimer dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Tick += new EventHandler(Timer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(200); //重复间隔
            dispatcherTimer.Start();
        }

        //private void Carrier_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    moveTo = e.GetPosition(Carrier);

        //    speedX = speed * Math.Cos(Math.Atan2(Math.Abs(moveTo.Y - Canvas.GetTop(rect)), Math.Abs(moveTo.X - Canvas.GetLeft(rect))));
        //    speedY = speed * Math.Sin(Math.Atan2(Math.Abs(moveTo.Y - Canvas.GetTop(rect)), Math.Abs(moveTo.X - Canvas.GetLeft(rect))));
        //    moveFlag = true;
        //}

        private void Timer_Tick(object sender, EventArgs e)
        {
            Spirit.Source = new BitmapImage(new Uri(@"Player\" + count + ".png", UriKind.Relative));

            count = count == 7 ? 0 : count + 1;
        }
    }
}
