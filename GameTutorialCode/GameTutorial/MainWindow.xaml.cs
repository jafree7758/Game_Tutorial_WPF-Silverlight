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
            ResetMatrix();  //初始化二维矩阵
            InitPlayer();   //初始化目标对象
        }

        Ellipse player = new Ellipse(); //用一个圆来模拟目标对象
        private void InitPlayer()
        {
            player.Fill = new SolidColorBrush(Colors.Blue);
            player.Width = GridSize;
            player.Height = GridSize;
            Carrier.Children.Add(player);
            //开始位置(1,1)
            Canvas.SetLeft(player, GridSize);
            Canvas.SetTop(player, 5 * GridSize);
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
            for (int y = 3; y < 30; y++)
            {
                //障碍物在矩阵中用0表示
                Matrix[3, y] = 0;
                rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Colors.Red);
                rect.Width = GridSize;
                rect.Height = GridSize;
                Carrier.Children.Add(rect);
                Canvas.SetLeft(rect, 3 * GridSize);
                Canvas.SetTop(rect, y * GridSize);
            }
            for (int y = 3; y < 20; y++)
            {
                //障碍物在矩阵中用0表示
                Matrix[24, y] = 0;
                rect = new Rectangle();
                rect.Fill = new SolidColorBrush(Colors.Red);
                rect.Width = GridSize;
                rect.Height = GridSize;
                Carrier.Children.Add(rect);
                Canvas.SetLeft(rect, 24 * GridSize);
                Canvas.SetTop(rect, y * GridSize);
            }
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
        }

        private void Carrier_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(Carrier);
            //进行坐标系缩小
            int start_x = (int)Canvas.GetLeft(player) / GridSize;
            int start_y = (int)Canvas.GetTop(player) / GridSize;
            Start = new System.Drawing.Point(start_x, start_y); //设置起点坐标
            int end_x = (int)p.X / GridSize;
            int end_y = (int)p.Y / GridSize;
            End = new System.Drawing.Point(end_x, end_y); //设置终点坐标

            PathFinder = new PathFinderFast(Matrix);
            PathFinder.Formula = HeuristicFormula.Manhattan; //使用我个人觉得最快的曼哈顿A*算法
            PathFinder.HeavyDiagonals = true; //使用对角线移动
            PathFinder.HeuristicEstimate = 0;
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
                Storyboard.SetTarget(keyFramesAnimationX, player);
                Storyboard.SetTargetProperty(keyFramesAnimationX, new PropertyPath("(Canvas.Left)"));
                //创建Y轴方向逐帧动画
                DoubleAnimationUsingKeyFrames keyFramesAnimationY = new DoubleAnimationUsingKeyFrames();
                keyFramesAnimationY.Duration = new Duration(TimeSpan.FromMilliseconds(path.Count * cost));
                Storyboard.SetTarget(keyFramesAnimationY, player);
                Storyboard.SetTargetProperty(keyFramesAnimationY, new PropertyPath("(Canvas.Top)"));
                for (int i = 0; i < framePosition.Count(); i++)
                {
                    //加入X轴方向的匀速关键帧
                    LinearDoubleKeyFrame keyFrame = new LinearDoubleKeyFrame();
                    keyFrame.Value = i == 0 ? Canvas.GetLeft(player) : framePosition[i].X; //平滑衔接动画
                    keyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(cost * i));
                    keyFramesAnimationX.KeyFrames.Add(keyFrame);
                    //加入X轴方向的匀速关键帧
                    keyFrame = new LinearDoubleKeyFrame();
                    keyFrame.Value = i == 0 ? Canvas.GetTop(player) : framePosition[i].Y;
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
    }
}
