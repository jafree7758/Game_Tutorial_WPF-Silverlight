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
using System.Windows.Threading;
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
        private System.Drawing.Point Start = System.Drawing.Point.Empty;    //移动起点坐标
        private System.Drawing.Point End = System.Drawing.Point.Empty;      //移动终点坐标

        public MainWindow()
        {
            InitializeComponent();
            ResetMatrix();  //初始化二维矩阵
            InitPlayer();   //初始化目标对象
            InitMap(); //初始化地图

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Start();
        }

        Image Map = new Image();
        private void InitMap()
        {
            Map.Width = 800;
            Map.Height = 600;
            Map.Source = new BitmapImage((new Uri(@"Map\Map.jpg", UriKind.Relative)));
            Carrier.Children.Add(Map);
            Map.SetValue(Canvas.ZIndexProperty, -1);
        }

        int count = 1;
        Image Spirit = new Image(); //创建主角
        int SpiritCenterX = 4; //主角脚底离主角图片左边的距离(游戏坐标系中)
        int SpiritCenterY = 5; //主角脚底离主角顶部的距离(游戏坐标系中)
        //游戏坐标系中Spirit坐标(缩小操作)
        int _SpiritGameX;
        int SpiritGameX
        {
            get { return ((int)Canvas.GetLeft(Spirit) / GridSize) + SpiritCenterX; }
            set { _SpiritGameX = value; }
        }
        int _SpiritGameY;
        int SpiritGameY
        {
            get { return ((int)Canvas.GetTop(Spirit) / GridSize) + SpiritCenterY; }
            set { _SpiritGameY = value; }
        }
        //窗口坐标系中Spirit坐标(放大操作)
        int SpiritWindowX
        {
            get { return (SpiritGameX - SpiritCenterX) * GridSize; }
        }
        int SpiritWindowY
        {
            get { return (SpiritGameY - SpiritCenterY) * GridSize; }
        }

        private void InitPlayer()
        {
            Spirit.Width = 150;
            Spirit.Height = 150;
            Carrier.Children.Add(Spirit);
            // 初始化主角位置
            Canvas.SetLeft(Spirit, 20);
            Canvas.SetTop(Spirit, 20);
            // 初始时认为终点就在主角脚下
            End.X = 5;
            End.Y = 6;
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

            //构建障碍物(第10节用)
            //for (int x = 10; x < 20; x++) {
            //    for (int y = 0; y < 10; y++) {
            //        Matrix[x, y] = 0;
            //        rect = new Rectangle();
            //        rect.Fill = new SolidColorBrush(Colors.GreenYellow);
            //        rect.Opacity = 0.3;
            //        rect.Stroke = new SolidColorBrush(Colors.Gray);
            //        rect.Width = GridSize;
            //        rect.Height = GridSize;
            //        Carrier.Children.Add(rect);
            //        Canvas.SetLeft(rect, x * GridSize);
            //        Canvas.SetTop(rect, y * GridSize);
            //    }
            //}
            //构建障碍物(第9节用)
            for (int y = 12; y <= 27; y++)
            {
                for (int x = 0; x <= 7; x++)
                {
                    //障碍物在矩阵中用0表示
                    Matrix[x, y] = 0;
                    rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.GreenYellow);
                    rect.Opacity = 0.3;
                    rect.Stroke = new SolidColorBrush(Colors.Gray);
                    rect.Width = GridSize;
                    rect.Height = GridSize;
                    Carrier.Children.Add(rect);
                    Canvas.SetLeft(rect, x * GridSize);
                    Canvas.SetTop(rect, y * GridSize);
                }
            }
            int move = 0;
            for (int x = 8; x <= 15; x++)
            {
                for (int y = 12; y <= 18; y++)
                {
                    Matrix[x, y - move] = 0;
                    rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.GreenYellow);
                    rect.Opacity = 0.3;
                    rect.Stroke = new SolidColorBrush(Colors.Gray);
                    rect.Width = GridSize;
                    rect.Height = GridSize;
                    Carrier.Children.Add(rect);
                    Canvas.SetLeft(rect, x * GridSize);
                    Canvas.SetTop(rect, (y - move) * GridSize);
                }
                move = x % 2 == 0 ? move + 1 : move;
            }
            int start_y = 4;
            int end_y = 10;
            for (int x = 16; x <= 23; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Matrix[x, y + move] = 0;
                    rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.GreenYellow);
                    rect.Opacity = 0.3;
                    rect.Stroke = new SolidColorBrush(Colors.Gray);
                    rect.Width = GridSize;
                    rect.Height = GridSize;
                    Carrier.Children.Add(rect);
                    Canvas.SetLeft(rect, x * GridSize);
                    Canvas.SetTop(rect, (y + move) * GridSize);
                }
                start_y = x % 3 == 0 ? start_y + 1 : start_y;
                end_y = x % 3 == 0 ? end_y - 1 : end_y;
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if ((End.X == SpiritGameX) && (End.Y == SpiritGameY))
            {
                // 未移动使用静止图片
                Spirit.Source = cutImage(@"Player\PlayerMagic.png", 0, 0, 150, 150);
            }
            else
            {
                // 移动时使用动态图片
                Spirit.Source = new BitmapImage((new Uri(@"Player\" + count + ".png", UriKind.Relative)));
                count = count == 7 ? 0 : count + 1;
            }
        }

        private void Carrier_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(Carrier);
            //进行坐标系缩小
            int start_x = SpiritGameX;
            int start_y = SpiritGameY;
            Start = new System.Drawing.Point(start_x, start_y); //设置起点坐标
            int end_x = (int)p.X / GridSize;
            int end_y = (int)p.Y / GridSize;
            End = new System.Drawing.Point(end_x, end_y); //设置终点坐标

            PathFinder = new PathFinderFast(Matrix);
            PathFinder.Formula = HeuristicFormula.Manhattan; //使用我个人觉得最快的曼哈顿A*算法
            List<PathFinderNode> path = PathFinder.FindPath(Start, End); //开始寻径

            if (path == null)
            {
                MessageBox.Show("路径不存在！");
            }
            else
            {
                Point[] framePosition = new Point[path.Count]; //定义关键帧坐标集
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    //从起点开始以GridSize为单位，顺序填充关键帧坐标集，并进行坐标系放大
                    framePosition[path.Count - 1 - i] = new Point(path[i].X * GridSize, path[i].Y * GridSize);
                }
                //创建故事板
                Storyboard storyboard = new Storyboard();
                int cost = 100; //每移动一个方格花费100毫秒
                //创建X轴方向逐帧动画
                DoubleAnimationUsingKeyFrames keyFramesAnimationX = new DoubleAnimationUsingKeyFrames();
                //总共花费时间 = path.Count * cost
                keyFramesAnimationX.Duration = new Duration(TimeSpan.FromMilliseconds(path.Count * cost));
                Storyboard.SetTarget(keyFramesAnimationX, Spirit);
                Storyboard.SetTargetProperty(keyFramesAnimationX, new PropertyPath("(Canvas.Left)"));
                //创建Y轴方向逐帧动画
                DoubleAnimationUsingKeyFrames keyFramesAnimationY = new DoubleAnimationUsingKeyFrames();
                keyFramesAnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(path.Count * cost));
                Storyboard.SetTarget(keyFramesAnimationY, Spirit);
                Storyboard.SetTargetProperty(keyFramesAnimationY, new PropertyPath("(Canvas.Top)"));
                for (int i = 0; i < framePosition.Count(); i++)
                {
                    //加入X轴方向的匀速关键帧
                    LinearDoubleKeyFrame keyFrame = new LinearDoubleKeyFrame();
                    //平滑衔接动画
                    keyFrame.Value = i == 0 ? Canvas.GetLeft(Spirit) : (framePosition[i].X - SpiritCenterX * GridSize);
                    keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(cost * i));
                    keyFramesAnimationX.KeyFrames.Add(keyFrame);
                    //加入X轴方向的匀速关键帧
                    keyFrame = new LinearDoubleKeyFrame();
                    keyFrame.Value = i == 0 ? Canvas.GetTop(Spirit) : (framePosition[i].Y - SpiritCenterY * GridSize);
                    keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(cost * i));
                    keyFramesAnimationY.KeyFrames.Add(keyFrame);
                }
                storyboard.Children.Add(keyFramesAnimationX);
                storyboard.Children.Add(keyFramesAnimationY);
                //故事板动画开始
                storyboard.Begin();
                //用白色点记录移动轨迹
                for (int i = path.Count - 1; i >= 0; i--)
                {
                    rect = new Rectangle();
                    rect.Fill = new SolidColorBrush(Colors.Snow);
                    rect.Width = 5;
                    rect.Height = 5;
                    Carrier.Children.Add(rect);
                    Canvas.SetLeft(rect, path[i].X * GridSize);
                    Canvas.SetTop(rect, path[i].Y * GridSize);
                }
            }
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
