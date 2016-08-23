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
        private TextBlock message = new TextBlock();

        public MainWindow()
        {
            InitializeComponent();
            ResetMatrix(); //初始化二维矩阵
            InitPlayer(); //初始化目标对象
            InitMap(); //初始化地图
            InitMask(); //初始化地图遮罩层
            Carrier.Children.Add(message);
            Canvas.SetLeft(message, 10);
            Canvas.SetTop(message, 10);

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(150);
            dispatcherTimer.Start();

            //注册界面刷新事件
            CompositionTarget.Rendering += new EventHandler(dispatcherTimer1_Tick);
        }

        Image Map = new Image();
        private void InitMap()
        {
            Map.Width = 800;
            Map.Height = 600;
            Map.Source = new BitmapImage((new Uri(@"Map\Map2.jpg", UriKind.Relative)));
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

        //创建遮罩层
        Image Mask1 = new Image();
        Image Mask2 = new Image();
        private void InitMask()
        {
            Mask1.Width = 238;
            Mask1.Height = 244;
            Mask1.Source = new BitmapImage((new Uri(@"Map\Mask1.png", UriKind.Relative)));
            Mask1.Opacity = 0.7;
            Carrier.Children.Add(Mask1);
            Canvas.SetZIndex(Mask1, 10000);
            Canvas.SetLeft(Mask1, 185);
            Canvas.SetTop(Mask1, 220);
            Mask2.Width = 198;
            Mask2.Height = 221;
            Mask2.Source = new BitmapImage((new Uri(@"Map\Mask2.png", UriKind.Relative)));
            Mask2.Opacity = 0.7;
            Carrier.Children.Add(Mask2);
            Canvas.SetZIndex(Mask2, 10000);
            Canvas.SetLeft(Mask2, 466);
            Canvas.SetTop(Mask2, 11);
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
            
            //构建障碍物
            for (int y = 22; y <= 24; y++)
            {
                for (int x = 5; x <= 16; x++)
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

            for (int y = 11; y <= 14; y++)
            {
                for (int x = 27; x <= 31; x++)
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

            for (int y = 18; y <= 21; y++)
            {
                for (int x = 33; x <= 37; x++)
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
        }

        //图片拾色
        private Color pickColor(BitmapSource bitmapsource, int x, int y)
        {
            CroppedBitmap crop = new CroppedBitmap(bitmapsource as BitmapSource, new Int32Rect(x, y, 1, 1));
            byte[] pixels = new byte[4];
            try
            {
                crop.CopyPixels(pixels, 4, 0);
                crop = null;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
            //蓝pixels[0] 绿pixels[1]  红pixels[2] 透明度pixels[3]
            return Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
        }

        BitmapSource Deeper = new BitmapImage((new Uri(@"Map\Deeper.jpg", UriKind.Relative))); //设置地图副本
        int X, Y; //主角当前的窗口真实坐标(非缩放)
        Point target; //主角移动的最终目的
        private void dispatcherTimer1_Tick(object sender, EventArgs e)
        {
            X = Convert.ToInt32(Canvas.GetLeft(Spirit) + SpiritCenterX * GridSize);
            Y = Convert.ToInt32(Canvas.GetTop(Spirit) + SpiritCenterY * GridSize);

            //message.Text = "坐标:" + X + "  " + Y;
            message.Text = pickColor(Deeper, X, Y).ToString();
            //假如碰到障碍物则采用A*寻路
            if (pickColor(Deeper, X, Y) == Colors.Black)
            {
                AStarMove(target);
            }
            else if (pickColor(Deeper, X, Y) == Colors.Yellow)
            {
                //假如是传送点则跳到坐标(200,20)
                storyboard.Stop();
                Canvas.SetLeft(Spirit, 200 - SpiritCenterX * GridSize);
                Canvas.SetTop(Spirit, 20 - SpiritCenterY * GridSize);
            }
            //用白色点记录移动轨迹
            rect = new Rectangle();
            rect.Fill = new SolidColorBrush(Colors.Snow);
            rect.Width = 5;
            rect.Height = 5;
            Carrier.Children.Add(rect);
            Canvas.SetLeft(rect, X);
            Canvas.SetTop(rect, Y);
        }

        Storyboard storyboard;
        //普通移动
        private void NormalMove(Point p)
        {
            //重新定位
            p = new Point(p.X - SpiritCenterX * GridSize, p.Y - SpiritCenterY * GridSize);
            //创建移动动画
            storyboard = new Storyboard();
            //创建X轴方向动画
            DoubleAnimation doubleAnimation = new DoubleAnimation(
              Canvas.GetLeft(Spirit),
              p.X,
              new Duration(TimeSpan.FromMilliseconds(1000))
            );
            Storyboard.SetTarget(doubleAnimation, Spirit);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(doubleAnimation);
            //创建Y轴方向动画
            doubleAnimation = new DoubleAnimation(
              Canvas.GetTop(Spirit),
              p.Y,
              new Duration(TimeSpan.FromMilliseconds(1000))
            );
            Storyboard.SetTarget(doubleAnimation, Spirit);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(doubleAnimation);
            //动画播放
            storyboard.Begin();
        }

        //A*移动
        private void AStarMove(Point p)
        {
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
                //MessageBox.Show("路径不存在！");
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
            //假如点到的地方不是障碍物
            Point p = e.GetPosition(Carrier);
            if (pickColor(Deeper, (int)p.X, (int)p.Y) != Colors.Black)
            {
                target = p;
                NormalMove(p); //直线移动
                //AStarMove(p); //纯A*寻路算法
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
