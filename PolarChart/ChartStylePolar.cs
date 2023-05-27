using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PolarChart
{
    public enum AngleDirection
    {
        CounterClockWise = 0,
        ClockWise = 1
    }

    public enum LinePattern
    {
        Solid = 1,
        Dash = 2,
        Dot = 3,
        DashDot = 4,
        None = 5
    }

    public class ChartStylePolar
    {
        public AngleDirection AngleDirection { get; set; } = AngleDirection.CounterClockWise;

        public double AngleStep { get; set; } = 30;

        public Canvas ChartCanvas { get; set; }

        public Brush LineColor { get; set; } = Brushes.Black;

        public LinePattern LinePattern { get; set; } = LinePattern.Dash;

        public double LineThickness { get; set; } = 1;

        public int NTicks { get; set; } = 4;

        public double RMax { get; set; } = 1;

        public double RMin { get; set; } = 0;

        /// <summary>
        /// Point(r, theta)
        /// theta has the same unit, degrees, in both the world and device coordinate systems
        /// RNormalize -> transforms the r value in the world coordinate system to an r value in the device coordinate system
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public double RNormalize(double r)
        {
            double width = Math.Min(ChartCanvas.Width, ChartCanvas.Height);
            return (r - RMin) * width / 2 / (RMax - RMin);
        }

        public DoubleCollection SetLinePattern()
        {
            DoubleCollection collection = new();
            switch (LinePattern)
            {
                case LinePattern.Dash:
                    collection = new DoubleCollection(new double[2] { 4, 3 });
                    break;

                case LinePattern.Dot:
                    collection = new DoubleCollection(new double[2] { 1, 2 });
                    break;

                case LinePattern.DashDot:
                    collection = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
            return collection;
        }

        public void SetPolarAxes()
        {
            double xc = ChartCanvas.Width / 2;
            double yc = ChartCanvas.Height / 2;

            //Draw Circles:
            double dr = RNormalize(RMax / NTicks) - RNormalize(RMin / NTicks); // radius increment
            for (int i = 0; i < NTicks; i++)
            {
                Ellipse circle = CircleLine();
                Canvas.SetLeft(circle, xc - (i + 1) * dr);
                Canvas.SetTop(circle, yc - (i + 1) * dr);
                circle.Width = 2.0 * (i + 1) * dr;
                circle.Height = 2.0 * (i + 1) * dr;
                ChartCanvas.Children.Add(circle);
            }

            //Draw radius lines:
            for (int i = 0; i < 360 / AngleStep; i++)
            {
                Line line = RadiusLine();
                line.X1 = RNormalize(RMax) * Math.Cos(i * AngleStep * Math.PI / 180) + xc;
                line.Y1 = RNormalize(RMax) * Math.Sin(i * AngleStep * Math.PI / 180) + yc;
                line.X2 = xc;
                line.Y2 = yc;
                ChartCanvas.Children.Add(line);
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
                ChartCanvas.Children.Add(textBlock);
            }

            //Add angle Labels:
            double angleLabel = 0;
            for (int i = 0; i < 360 / AngleStep; i++)
            {
                if (AngleDirection == AngleDirection.ClockWise)
                {
                    angleLabel = i * AngleStep;
                }
                else if (AngleDirection == AngleDirection.CounterClockWise)
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
                textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size size = textBlock.DesiredSize;

                double x = RNormalize(RMax * 1.35) *
                    Math.Cos(i * AngleStep * Math.PI / 180) + xc; //1.35 gap factor from outer circle
                double y = RNormalize(RMax * 1.35) *
                    Math.Sin(i * AngleStep * Math.PI / 180) + yc;

                Canvas.SetLeft(textBlock, x - size.Width / 2);
                Canvas.SetTop(textBlock, y - size.Height / 2);
                ChartCanvas.Children.Add(textBlock);
            }
        }

        private Ellipse CircleLine()
        {
            Ellipse ellipse = new()
            {
                Stroke = LineColor,
                StrokeThickness = LineThickness,
                StrokeDashArray = SetLinePattern(),
                Fill = Brushes.Transparent
            };
            return ellipse;
        }

        private Line RadiusLine()
        {
            Line line = new()
            {
                Stroke = LineColor,
                StrokeThickness = LineThickness,
                StrokeDashArray = SetLinePattern()
            };
            return line;
        }
    }

    public class DataCollection
    {
        public DataCollection()
        {
            DataList = new List<DataSeries>();
        }

        public List<DataSeries> DataList { get; set; }
    }

    public class DataCollectionPolar : DataCollection
    {
        public void AddPolar(ChartStylePolar chartStylePolar)
        {
            double xc = chartStylePolar.ChartCanvas.Width / 2;
            double yc = chartStylePolar.ChartCanvas.Height / 2;

            int j = 0;
            foreach (DataSeries ds in DataList)
            {
                if (ds.SeriesName == "Default Name")
                {
                    ds.SeriesName = "DataSeries" + j.ToString();
                }
                ds.AddLinePattern();
                for (int i = 0; i < ds.LineSeries.Points.Count; i++)
                {
                    double r = ds.LineSeries.Points[i].Y;
                    double theta = ds.LineSeries.Points[i].X * Math.PI / 180;
                    if (chartStylePolar.AngleDirection == AngleDirection.CounterClockWise)
                    {
                        theta = -theta;
                    }
                    double x = xc + chartStylePolar.RNormalize(r) * Math.Cos(theta);
                    double y = yc + chartStylePolar.RNormalize(r) * Math.Sin(theta);
                    ds.LineSeries.Points[i] = new Point(x, y);
                }
                chartStylePolar.ChartCanvas.Children.Add(ds.LineSeries);
                j++;
            }
        }
    }

    public class DataSeries
    {
        public DataSeries()
        {
            LineColor = Brushes.Black;
        }

        public Brush LineColor { get; set; }

        public LinePattern LinePattern { get; set; }

        public Polyline LineSeries { get; set; } = new();

        public double LineThickness { get; set; } = 1;

        public string SeriesName { get; set; } = "Default Name";

        public void AddLinePattern()
        {
            LineSeries.Stroke = LineColor;
            LineSeries.StrokeThickness = LineThickness;

            switch (LinePattern)
            {
                case LinePattern.Dash:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;

                case LinePattern.Dot:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;

                case LinePattern.DashDot:
                    LineSeries.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;

                case LinePattern.None:
                    LineSeries.Stroke = Brushes.Transparent;
                    break;
            }
        }
    }
}