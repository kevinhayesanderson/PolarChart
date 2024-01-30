using System;
using System.Windows;
using System.Windows.Media;

namespace PolarChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChartStylePolar cs;
        private DataCollectionPolar dc;
        private DataSeries ds;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = chartGrid.ActualWidth;
            double height = chartGrid.ActualHeight;
            double size = width;
            if (width > height)
            {
                size = height;
            }
            chartCanvas.Width = size;
            chartCanvas.Height = size;
            chartCanvas.Children.Clear();
            AddChart();
            //AddChart1();
        }

        private void AddChart()
        {
            cs = new ChartStylePolar();
            dc = new DataCollectionPolar();
            cs.ChartCanvas = chartCanvas;
            cs.RMax = 0.5;
            cs.RMin = 0;
            cs.NTicks = 4;
            cs.AngleStep = 30;
            cs.AngleDirection = AngleDirection.CounterClockWise;
            cs.LinePattern = LinePattern.Dot;
            cs.LineColor = Brushes.Black;
            cs.SetPolarAxes();
            dc.DataList.Clear();
            ds = new DataSeries
            {
                LineColor = Brushes.Red
            };
            for (int i = 0; i < 360; i++)
            {
                double theta = 1.0 * i;
                double r = Math.Abs(Math.Cos(2.0 * theta * Math.PI / 180) *
                    Math.Sin(2.0 * theta * Math.PI / 180));
                ds.LineSeries.Points.Add(new Point(theta, r));
            }
            dc.DataList.Add(ds);
            dc.AddPolar(cs);
        }

        private void AddChart1()
        {
            cs = new ChartStylePolar();
            dc = new DataCollectionPolar();
            cs.ChartCanvas = chartCanvas;
            cs.RMax = 1.0;
            cs.RMin = -7.0;
            cs.NTicks = 4;
            cs.AngleStep = 30;
            cs.AngleDirection = AngleDirection.CounterClockWise;
            cs.LinePattern = LinePattern.Dot;
            cs.LineColor = Brushes.Black;
            cs.SetPolarAxes();
            dc.DataList.Clear();
            ds = new DataSeries
            {
                LineColor = Brushes.Red
            };
            for (int i = 0; i < 360; i++)
            {
                double theta = 1.0 * i;
                double r = Math.Log(1.001 + Math.Sin(2 * theta * Math.PI / 180));
                ds.LineSeries.Points.Add(new Point(theta, r));
            }
            dc.DataList.Add(ds);

            ds = new DataSeries
            {
                LineColor = Brushes.Blue
            };
            for (int i = 0; i < 360; i++)
            {
                double theta = 1.0 * i;
                double r = Math.Log(1.001 + Math.Cos(2 * theta * Math.PI / 180));
                ds.LineSeries.Points.Add(new Point(theta, r));
            }
            dc.DataList.Add(ds);
            dc.AddPolar(cs);
        }
    }
}