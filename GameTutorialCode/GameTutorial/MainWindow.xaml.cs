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
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100); //重复间隔
            dispatcherTimer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //Spirit.Source = new BitmapImage(new Uri(@"Player\" + count + ".png", UriKind.Relative));
            Spirit.Source = cutImage(@"Player\PlayerMagic.png", count * 150, 0, 150, 150);

            count = count == 9 ? 0 : count + 1;
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
