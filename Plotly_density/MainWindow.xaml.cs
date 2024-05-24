using Plotly.NET.CSharp;
using Plotly.NET.ImageExport;
using Plotly.NET.TraceObjects;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using static Plotly.NET.StyleParam;

namespace Plotly_density
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PointPolar();
            //XYDensity();
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
            Plotly.NET.Defaults.DefaultTemplate = Plotly.NET.ChartTemplates.lightMirrored;

            Plotly.NET.Defaults.DefaultWidth = width;

            Plotly.NET.Defaults.DefaultHeight = height;

            Plotly.NET.Defaults.DefaultDisplayOptions = Plotly.NET.DisplayOptions.init(PlotlyJSReference: Plotly.NET.PlotlyJSReference.Full);

            PuppeteerSharpRendererOptions.launchOptions.Timeout = 0;

            var pointPolar = Chart.PointPolar<double, double, string>(theta: polarData.theta,

                                                                        r: polarData.r,

                                                                        Name: "_title",

                                                                        Marker: Marker.init(Size: 3, Color: Plotly.NET.Color.fromKeyword(Plotly.NET.ColorKeyword.Green)),

                                                                        UseDefaults: false,

                                                                        UseWebGL: true,

                                                                        ShowLegend: true
                                                                        );

            return pointPolar.ToBase64PNGStringAsync(Width: width, Height: height);
        }

        private async Task<(string, string)> InitializeXYDensityChart((List<double> X, List<double> Y) data, int width, int height)
        {
            var name = "xy_density";
            Plotly.NET.Defaults.DefaultWidth = width;
            Plotly.NET.Defaults.DefaultHeight = height;
            Plotly.NET.Defaults.DefaultDisplayOptions = Plotly.NET.DisplayOptions.init(PlotlyJSReference: Plotly.NET.PlotlyJSReference.Full);
            var pointPolar = Chart.PointDensity(x: data.X,
                                                y: data.Y,
                                                PointMarkerColor: Plotly.NET.Color.fromKeyword(Plotly.NET.ColorKeyword.White),
                                                PointMarkerSymbol: MarkerSymbol.Circle,
                                                PointMarkerSize: 1,
                                                ColorScale: Colorscale.Greens,
                                                ColorBar: Plotly.NET.ColorBar.init<double, double>(Title: Plotly.NET.Title.init("Density")),
                                                ShowContourLabels: true);
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
            var htmlPath = path + ".html";
            var imagePath = path;
            pointPolar.SaveHtml(htmlPath);
            await pointPolar.SavePNGAsync(imagePath, Width: width, Height: height);
            return (htmlPath, imagePath + ".png");
        }

        private void PointPolar()
        {
            var data = ReadData();
            var polarData = ConvertData(data);

            int width = (int)this.Width;
            int height = (int)this.Height;
            string imageString = Task.Run(() => InitializePointPolarChart(polarData, width, height)).Result;
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.StreamSource = new MemoryStream(Convert.FromBase64String(imageString.Replace("data:image/png;base64,", "")));
            myBitmapImage.DecodePixelWidth = width;
            myBitmapImage.EndInit();
            plotImage.Source = myBitmapImage;
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
            (string htmlPath, string imagePath) = Task.Run(() => InitializeXYDensityChart((data.Select(x => x.Item1).ToList(), data.Select(x => x.Item2).ToList()), width, height)).Result;
            BitmapImage myBitmapImage = new BitmapImage();
            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(imagePath);
            myBitmapImage.DecodePixelWidth = width;
            myBitmapImage.EndInit();
            plotImage.Source = myBitmapImage;
        }
    }
}