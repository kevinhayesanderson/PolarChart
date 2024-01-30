using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GradientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        Line gradientLine;
        RadialGradientBrush gradBrush = new RadialGradientBrush();



        void timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < gradBrush.GradientStops.Count; ++i)
            {
                gradBrush.GradientStops[i].Offset = gradBrush.GradientStops[i].Offset - 0.01;
                if (gradBrush.GradientStops[i].Offset < -1) gradBrush.GradientStops[i].Offset += 4;
            }

        }

        private void mainGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            chartCanvas.Children.Clear();

            gradBrush.GradientStops.Add(new GradientStop(Colors.Red, 0));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Orange, 0.125));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.25));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.375));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.5));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Indigo, 0.625));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Violet, 0.75));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Purple, 1));

            gradBrush.Center = new Point(mainGrid.ActualWidth / 2, mainGrid.ActualHeight / 2);
            gradBrush.GradientOrigin = new Point(mainGrid.ActualWidth / 2, mainGrid.ActualHeight / 2);
            gradientLine = new Line() { X1 = 500, Y1 = 500, X2 = 1000, Y2 = 1000, StrokeThickness = 10, Stroke = gradBrush };
            mainGrid.Children.Add(gradientLine);


            var lineSeries = new Polyline();
            lineSeries.Stroke = gradBrush;
            lineSeries.StrokeThickness = 2;
            //lineSeries.Fill = gradBrush;
            for (int i = 0; i < 360; i++)
            {
                double theta = 1.0 * i;
                double r = Math.Log(1.001 + Math.Sin(2 * theta * Math.PI / 180));
                lineSeries.Points.Add(new Point(theta, r));
            }

            double xc = mainGrid.ActualWidth / 2;
            double yc = mainGrid.ActualHeight / 2;


            for (int i = 0; i < lineSeries.Points.Count; i++)
            {
                double r = lineSeries.Points[i].Y;
                double theta = lineSeries.Points[i].X * Math.PI / 180;
                //if (chartStylePolar.AngleDirection == AngleDirection.CounterClockWise)
                //{
                //    theta = -theta;
                //}
                double x = xc + RNormalize(r) * Math.Cos(theta);
                double y = yc + RNormalize(r) * Math.Sin(theta);
                lineSeries.Points[i] = new Point(x, y);

                //var circle = new EllipseGeometry(new Point(xc, yc), x, y);

                Ellipse circle = new()
                {
                    Width = RNormalize(r),
                    Height = RNormalize(r),
                    Stroke = gradBrush,
                    UseLayoutRounding = true
                };



                circle.SetValue(Canvas.LeftProperty, (mainGrid.ActualWidth / 2) - RNormalize(r));
                circle.SetValue(Canvas.TopProperty, (mainGrid.ActualHeight / 2) - RNormalize(r));



                chartCanvas.Children.Add(circle);
            }
            //chartCanvas.Children.Add(lineSeries);


            //timer.Tick += timer_Tick;
            //timer.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 30);
            //timer.Start();
        }

        public double RNormalize(double r)
        {
            var RMax = 1.0;
            var RMin = -7.0;
            double width = Math.Min(mainGrid.ActualWidth, mainGrid.ActualHeight);
            return (r - RMin) * width / 2 / (RMax - RMin);
        }
    }
}