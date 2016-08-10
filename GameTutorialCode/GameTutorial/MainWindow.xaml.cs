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
        int count = 1;
        Image Spirit;
        Storyboard storyboard;
        Point moveTo;

        public MainWindow()
        {
            InitializeComponent();

            moveTo.X = 0;
            moveTo.Y = 0;
            Spirit = new Image();
            Spirit.Width = 150;
            Spirit.Height = 150;
            Carrier.Children.Add(Spirit);

            Canvas.SetLeft(Spirit, 0);
            Canvas.SetTop(Spirit, 0);

            //定义线程
            DispatcherTimer dispatcherTimer = new DispatcherTimer(DispatcherPriority.Normal);
            dispatcherTimer.Tick += new EventHandler(Timer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100); //重复间隔
            dispatcherTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if ((moveTo.X == Canvas.GetLeft(Spirit)) && (moveTo.Y == Canvas.GetTop(Spirit)))
            {
                Spirit.Source = cutImage(@"Player\PlayerMagic.png", 0, 0, 150, 150);
            }
            else
            {
                Spirit.Source = new BitmapImage((new Uri(@"Player\" + count + ".png", UriKind.Relative)));
                count = count == 7 ? 0 : count + 1;
            }
        }

        private void Carrier_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            moveTo = e.GetPosition(Carrier);

            moveTo.X = moveTo.X - 75;   // 转换成脚底位置
            moveTo.Y = moveTo.Y - 115;  // 转换成脚底位置

            Move(moveTo);
        }


        private void Move(Point p)
        {

            //创建移动动画
            storyboard = new Storyboard();

            //创建X轴方向动画
            DoubleAnimation doubleAnimation = new DoubleAnimation(
              Canvas.GetLeft(Spirit),
              p.X,
              new Duration(TimeSpan.FromSeconds(1))
            );

            Storyboard.SetTarget(doubleAnimation, Spirit);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(doubleAnimation);

            //创建Y轴方向动画
            doubleAnimation = new DoubleAnimation(
              Canvas.GetTop(Spirit),
              p.Y,
              new Duration(TimeSpan.FromSeconds(1))
            );

            Storyboard.SetTarget(doubleAnimation, Spirit);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(doubleAnimation);

            //将动画动态加载进资源内
            if (!Resources.Contains("rectAnimation"))
            {
                Resources.Add("rectAnimation", storyboard);
            }

            //动画播放
            storyboard.Begin();
        }

        /// <summary>
        /// 截取图片
        /// </summary>
        /// <param name="imgaddress">文件名(包括地址+扩展名)</param>
        /// <param name="x">左上角点X</param>
        /// <param name="y">左上角点Y</param>
        /// <param name="width">截取的图片宽</param>
        /// <param name="height">截取的图片高</param>
        /// <returns>截取后图片数据源</returns>
        private BitmapSource cutImage(string imgaddress, int x, int y, int width, int height)
        {
            return new CroppedBitmap(BitmapFrame.Create(new Uri(imgaddress, UriKind.Relative)), new Int32Rect(x, y, width, height));
        }
    }
}
