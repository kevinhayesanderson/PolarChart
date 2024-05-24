using Plotly.NET.ImageExport;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Plotly_density
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //PointPolar();
            XYDensity();
        }

        private (double[], double[]) ConvertData(List<(double X, double Y)> datas)
        {
            var theta = new List<double>();
            var r = new List<double>();
            foreach (var (X, Y) in datas)
            {
                var angle = Y / Math.PI * 180;
                var magnitude = Math.Sqrt((X * X) + (Y * Y));
                if (angle < 0)
                {
                    angle += 360;
                }
                if (angle > 360)
                {
                    angle -= 360;
                }
                theta.Add(angle);
                r.Add(magnitude);
            }

            return (theta.ToArray(), r.ToArray());
        }

        private Task<string> InitializePointPolarChart((double[] theta, double[] r) polarData, int width, int height)
        {
            return PlotlyHelper
                .ChartInitializer
                .PointPolar(polarData.r, polarData.theta, width, height)
                .ToBase64PNGStringAsync(Width: width, Height: height);
        }

        private Task<string> InitializeXYDensityChart((List<double> X, List<double> Y) data, int width, int height)
        {
            return PlotlyHelper
                .ChartInitializer
                .PointDensity(data.X, data.Y, width, height)
                .ToBase64PNGStringAsync(Width: width, Height: height);
        }

        private void PointPolar()
        {
            var data = ReadData();
            var polarData = ConvertData(data);

            int width = (int)this.Width;
            int height = (int)this.Height;
            string imageString = Task.Run(() => InitializePointPolarChart(polarData, width, height)).Result;
            plotImage.Source = CreateBitmapImage(width, imageString);
        }

        private static BitmapImage CreateBitmapImage(int width, string imageString)
        {
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.StreamSource = new MemoryStream(Convert.FromBase64String(imageString.Replace("data:image/png;base64,", "")));
            myBitmapImage.DecodePixelWidth = width;
            myBitmapImage.EndInit();
            return myBitmapImage;
        }

        private List<(double, double)> ReadData()
        {
            var data = new List<(double, double)>();
            using var reader = new StreamReader(@"C:\Users\kevin\Downloads\Book08_26_58_6459972.csv");
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');
                data.Add((double.Parse(values[0]), double.Parse(values[1])));
            }
            return data;
        }

        private void XYDensity()
        {
            var data = ReadData();
            int width = (int)this.Width;
            int height = (int)this.Height;
            string imageString = Task.Run(() => InitializeXYDensityChart((data.Select(x => x.Item1).ToList(), data.Select(x => x.Item2).ToList()), width, height)).Result;
            plotImage.Source = CreateBitmapImage(width, imageString);
        }
    }
}