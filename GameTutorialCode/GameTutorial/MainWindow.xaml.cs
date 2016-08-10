using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using QX.Game.PathFinder;

namespace GameTutorial
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Rectangle rect;
        private IPathFinder PathFinder = null;
        private byte[,] Matrix = new byte[1024, 1024]; //寻路用二维矩阵
        private int GridSize = 20; //单位格子大小
        private System.Drawing.Point Start = System.Drawing.Point.Empty; //移动起点坐标
        private System.Drawing.Point End = System.Drawing.Point.Empty; //移动终点坐标

        public MainWindow()
        {
            InitializeComponent();

            ResetMatrix(); //初始化二维矩阵
        }

        private void ResetMatrix()
        {
            for (int y = 0; y < Matrix.GetUpperBound(1); y++)
            {
                for (int x = 0; x < Matrix.GetUpperBound(0); x++)
                {
                    //默认值可以通过在矩阵中用1表示
                    Matrix[x, y] = 1;
                }
            }
            //构建障碍物
            for (int i = 0; i < 18; i++)
            {
                //障碍物在矩阵中用0表示
                Matrix[i, 12] = 0;
                rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Colors.Red);
                rect.Width = GridSize;
                rect.Height = GridSize;
                Carrier.Children.Add(rect);
                Canvas.SetLeft(rect, i * GridSize);
                Canvas.SetTop(rect, 12 * GridSize);
            }
            for (int i = 12; i < 17; i++)
            {
                Matrix[17, i] = 0;
                rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Colors.Red);
                rect.Width = GridSize;
                rect.Height = GridSize;
                Carrier.Children.Add(rect);
                Canvas.SetLeft(rect, 17 * GridSize);
                Canvas.SetTop(rect, i * GridSize);
            }
            for (int i = 3; i < 18; i++)
            {
                Matrix[i, 16] = 0;
                rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Colors.Red);
                rect.Width = GridSize;
                rect.Height = GridSize;
                Carrier.Children.Add(rect);
                Canvas.SetLeft(rect, i * GridSize);
                Canvas.SetTop(rect, 16 * GridSize);
            }
            Start = new System.Drawing.Point(1, 1); //设置起点坐标
        }

        private void Carrier_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(Carrier);
            int x = (int)p.X / GridSize;
            int y = (int)p.Y / GridSize;
            End = new System.Drawing.Point(x, y); //计算终点坐标

            PathFinder = new PathFinderFast(Matrix);
            PathFinder.Formula = HeuristicFormula.Manhattan; //使用我个人觉得最快的曼哈顿A*算法
            PathFinder.SearchLimit = 2000; //即移动经过方块(20*20)不大于2000个(简单理解就是步数)

            List<PathFinderNode> path = PathFinder.FindPath(Start, End); //开始寻径

            if (path == null)
            {
                MessageBox.Show("路径不存在！");
            }
            else {
                string output = string.Empty;
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    output = string.Format(output
                        + "{0}"
                        + path[i].X.ToString()
                        + "{1}"
                        + path[i].Y.ToString()
                        + "{2}",
                        "(", ",", ") ");
                    rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.Green);
                    rect.Width = GridSize;
                    rect.Height = GridSize;
                    Carrier.Children.Add(rect);
                    Canvas.SetLeft(rect, path[i].X * GridSize);
                    Canvas.SetTop(rect, path[i].Y * GridSize);
                }
                MessageBox.Show("路径坐标分别为:" + output);
            }
        }

        //private void Timer_Tick(object sender, EventArgs e)
        //{
        //    if ((moveTo.X == Canvas.GetLeft(Spirit)) && (moveTo.Y == Canvas.GetTop(Spirit)))
        //    {
        //        Spirit.Source = cutImage(@"Player\PlayerMagic.png", 0, 0, 150, 150);
        //    }
        //    else
        //    {
        //        Spirit.Source = new BitmapImage((new Uri(@"Player\" + count + ".png", UriKind.Relative)));
        //        count = count == 7 ? 0 : count + 1;
        //    }
        //}

        //private void Carrier_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    moveTo = e.GetPosition(Carrier);

        //    moveTo.X = moveTo.X - 75;   // 转换成脚底位置
        //    moveTo.Y = moveTo.Y - 115;  // 转换成脚底位置

        //    Move(moveTo);
        //}


        //private void Move(Point p)
        //{

        //    //创建移动动画
        //    storyboard = new Storyboard();

        //    //创建X轴方向动画
        //    DoubleAnimation doubleAnimation = new DoubleAnimation(
        //      Canvas.GetLeft(Spirit),
        //      p.X,
        //      new Duration(TimeSpan.FromSeconds(1))
        //    );

        //    Storyboard.SetTarget(doubleAnimation, Spirit);
        //    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
        //    storyboard.Children.Add(doubleAnimation);

        //    //创建Y轴方向动画
        //    doubleAnimation = new DoubleAnimation(
        //      Canvas.GetTop(Spirit),
        //      p.Y,
        //      new Duration(TimeSpan.FromSeconds(1))
        //    );

        //    Storyboard.SetTarget(doubleAnimation, Spirit);
        //    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
        //    storyboard.Children.Add(doubleAnimation);

        //    //将动画动态加载进资源内
        //    if (!Resources.Contains("rectAnimation"))
        //    {
        //        Resources.Add("rectAnimation", storyboard);
        //    }

        //    //动画播放
        //    storyboard.Begin();
        //}

        ///// <summary>
        ///// 截取图片
        ///// </summary>
        ///// <param name="imgaddress">文件名(包括地址+扩展名)</param>
        ///// <param name="x">左上角点X</param>
        ///// <param name="y">左上角点Y</param>
        ///// <param name="width">截取的图片宽</param>
        ///// <param name="height">截取的图片高</param>
        ///// <returns>截取后图片数据源</returns>
        //private BitmapSource cutImage(string imgaddress, int x, int y, int width, int height)
        //{
        //    return new CroppedBitmap(BitmapFrame.Create(new Uri(imgaddress, UriKind.Relative)), new Int32Rect(x, y, width, height));
        //}
    }
}
