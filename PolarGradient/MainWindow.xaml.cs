using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolarGradient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double RMax;
        private double RMin;
        private int NTicks;
        private double AngleStep;
        private AngleDirection angleDirection;

        public enum AngleDirection
        {
            CounterClockWise = 0,
            ClockWise = 1
        }

        public MainWindow()
        {
            InitializeComponent();
            RMax = 1.0;
            RMin = -7.0;
            NTicks = 4;
            AngleStep = 30;
            angleDirection = AngleDirection.CounterClockWise;

        }

        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            chartCanvas.Children.Clear();
            DrawAxis();
            DrawCurve();
            DrawLine();
        }

        private void DrawLine()
        {
            var gradientLine = new Line() { StrokeThickness = 25, Stroke = GetLinearGradientBrush() };

            gradientLine.X1 = 0;
            gradientLine.Y1 = chartCanvas.ActualHeight;
            gradientLine.X2 = 0;
            gradientLine.Y2 = 0;

            chartCanvas.Children.Add(gradientLine);
        }

        private LinearGradientBrush GetLinearGradientBrush()
        {
            LinearGradientBrush linGradBrush = new LinearGradientBrush();
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Red, 0));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Orange, 0.1));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.2));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.3));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.4));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Indigo, 0.5));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.6));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.7));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.8));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Orange, 0.9));
            linGradBrush.GradientStops.Add(new GradientStop(Colors.Red, 1));

            //linGradBrush.StartPoint = new Point(0, chartCanvas.ActualHeight);
            //linGradBrush.EndPoint = new Point(0, chartCanvas.ActualWidth);
            return linGradBrush;
        }

        private RadialGradientBrush GetRadialGradientBrush()
        {
            RadialGradientBrush gradBrush = new RadialGradientBrush();
            gradBrush.GradientStops.Add(new GradientStop(Colors.Red, 0));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Orange, 0.15));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.30));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.45));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.75));
            gradBrush.GradientStops.Add(new GradientStop(Colors.Indigo, 1));
            return gradBrush;
        }

        private void DrawCurve()
        {
            List<Polyline> dataList = new List<Polyline>();

            Polyline sinLine = new Polyline();
            for (int i = 0; i < 360; i++)
            {
                double theta = 1.0 * i;
                double r = Math.Log(1.001 + Math.Sin(2 * theta * Math.PI / 180));
                sinLine.Points.Add(new System.Windows.Point(theta, r));
            }
            dataList.Add(sinLine);

            Polyline cosLine = new Polyline();
            for (int i = 0; i < 360; i++)
            {
                double theta = 1.0 * i;
                double r = Math.Log(1.001 + Math.Cos(2 * theta * Math.PI / 180));
                cosLine.Points.Add(new System.Windows.Point(theta, r));
            }
            //dataList.Add(cosLine);

            double xc = chartCanvas.ActualWidth / 2;
            double yc = chartCanvas.ActualHeight / 2;
            var radialGradient = GetRadialGradientBrush();

            // Set the GradientOrigin to the center of the area being painted.
            radialGradient.GradientOrigin = new System.Windows.Point(0.5, 0.5);

            // Set the gradient center to the center of the area being painted.
            radialGradient.Center = new System.Windows.Point(0.5, 0.5);

            // Set the radius of the gradient circle so that it extends to the edges of the area being painted.
            radialGradient.RadiusX = 0.5;
            radialGradient.RadiusY = 0.5;



            // Freeze the brush (make it unmodifiable) for performance benefits.
            radialGradient.Freeze();

            var linGradBrush = GetLinearGradientBrush();

            linGradBrush.Freeze();


            for (int i = 0; i < sinLine.Points.Count; i++)
            {
                
                double r = sinLine.Points[i].Y;
                Ellipse circle = new()
                {
                    Stroke = radialGradient,
                    StrokeThickness = 1,
                };
                Canvas.SetLeft(circle, xc - RNormalize(r));
                Canvas.SetTop(circle, yc - RNormalize(r));
                circle.Width = 2.0 * RNormalize(r);
                circle.Height = 2.0 * RNormalize(r);
                chartCanvas.Children.Add(circle);
            }

        }

        private void DrawAxis()
        {
            double xc = chartCanvas.ActualWidth / 2;
            double yc = chartCanvas.ActualHeight / 2;

            //Draw Circles:
            double dr = RNormalize(RMax / NTicks) - RNormalize(RMin / NTicks); // radius increment
            for (int i = 0; i < NTicks; i++)
            {
                Ellipse circle = CircleLine();
                Canvas.SetLeft(circle, xc - (i + 1) * dr);
                Canvas.SetTop(circle, yc - (i + 1) * dr);
                circle.Width = 2.0 * (i + 1) * dr;
                circle.Height = 2.0 * (i + 1) * dr;
                chartCanvas.Children.Add(circle);
            }

            //Draw radius lines:
            for (int i = 0; i < 360 / AngleStep; i++)
            {
                Line line = RadiusLine();
                line.X1 = RNormalize(RMax) * Math.Cos(i * AngleStep * Math.PI / 180) + xc;
                line.Y1 = RNormalize(RMax) * Math.Sin(i * AngleStep * Math.PI / 180) + yc;
                line.X2 = xc;
                line.Y2 = yc;
                chartCanvas.Children.Add(line);
            }

            //Add radius labels:
            for (int i = 1; i <= NTicks; i++)
            {
                double rLabel = RMin + i * (RMax - RMin) / NTicks;
                TextBlock textBlock = new()
                {
                    Text = rLabel.ToString()
                };
                Canvas.SetLeft(textBlock, xc + 3);
                Canvas.SetTop(textBlock, yc - i * dr + 2);
                chartCanvas.Children.Add(textBlock);
            }

            //Add angle Labels:
            double angleLabel = 0;
            for (int i = 0; i < 360 / AngleStep; i++)
            {
                if (angleDirection == AngleDirection.ClockWise)
                {
                    angleLabel = i * AngleStep;
                }
                else if (angleDirection == AngleDirection.CounterClockWise)
                {
                    angleLabel = 360 - i * AngleStep;
                    if (i == 0)
                    {
                        angleLabel = 0;
                    }
                }
                TextBlock textBlock = new()
                {
                    Text = angleLabel.ToString(),
                    TextAlignment = TextAlignment.Center
                };
                textBlock.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                System.Windows.Size size = textBlock.DesiredSize;

                double x = RNormalize(RMax * 1.35) *
                    Math.Cos(i * AngleStep * Math.PI / 180) + xc; //1.35 gap factor from outer circle
                double y = RNormalize(RMax * 1.35) *
                    Math.Sin(i * AngleStep * Math.PI / 180) + yc;

                Canvas.SetLeft(textBlock, x - size.Width / 2);
                Canvas.SetTop(textBlock, y - size.Height / 2);
                chartCanvas.Children.Add(textBlock);
            }
        }

        public double RNormalize(double r)
        {
            double width = Math.Min(chartCanvas.ActualWidth, chartCanvas.ActualHeight);
            return (r - RMin) * width / 2 / (RMax - RMin);
        }
        private Ellipse CircleLine(bool withFill = true)
        {
            Ellipse ellipse = new()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 }),
            };
            if (withFill) ellipse.Fill = Brushes.Transparent;
            return ellipse;
        }

        private Line RadiusLine()
        {
            Line line = new()
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 })
            };
            return line;
        }
    }
}