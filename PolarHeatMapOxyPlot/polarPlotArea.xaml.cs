using OxyPlot;
using OxyPlot.Axes;
using System.IO;
using System.Windows.Controls;

namespace PolarHeatMapOxyPlot
{
    /// <summary>
    /// Interaction logic for polarPlotArea.xaml
    /// </summary>
    public partial class polarPlotArea : UserControl
    {
        public PlotModel PlotModel { get; private set; }

        public polarPlotArea()
        {
            InitializeComponent();
            DataContext = this;
            PlotModel = PolarHeatMapInterpolated();
        }

        public PlotModel PolarHeatMapInterpolated()
        {
            var model = new PlotModel
            {
                Title = "Polar heat map (interpolated)",
                PlotMargins = new OxyThickness(40, 80, 40, 40),
                PlotType = PlotType.Polar,
                PlotAreaBorderThickness = new OxyThickness(0)
            };

            var matrix = new double[4, 2];
            matrix[0, 0] = 1;
            matrix[0, 1] = 0;
            matrix[1, 0] = 0;
            matrix[1, 1] = 1;
            matrix[2, 0] = -1;
            matrix[2, 1] = 0;
            matrix[3, 0] = 0;
            matrix[3, 1] = -1;

            matrix = new double[10998, 2];
            using (var reader = new StreamReader(@"C:\Users\kevin\Downloads\Book08_26_58_6459972.csv"))
            {
                int i = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    matrix[i, 0] = double.Parse(values[0]);
                    matrix[i, 1] = double.Parse(values[1]);
                    i++;
                }
            }

            using (StreamWriter file = new StreamWriter(@$"D:\PolarChart\PolarHeatMapOxyPlot\Book{DateTime.UtcNow.TimeOfDay.ToString().Replace(".", "_").Replace(":", "_")}.csv"))
            {
                for (int i = 0; i < 101; i++)
                {
                    file.Write($"{matrix[i, 0]},{matrix[i, 1]}");
                    file.Write(Environment.NewLine);
                }
            }

            double[] GetColumn(double[,] matrix, int columnNumber)
            {
                return Enumerable.Range(0, matrix.GetLength(0))
                        .Select(x => matrix[x, columnNumber])
                        .ToArray();
            }

            var x = GetColumn(matrix, 0);
            var y = GetColumn(matrix, 1);

            var xmin = x.Min();
            var xmax = x.Max();

            var xFirst = x.First();
            var yFirst = y.First();
            var xLast = x.Last();
            var yLast = y.Last();

            model.Axes.Add(new AngleAxis
            {
                StartAngle = 270,
                EndAngle = 270 + 360,
                Minimum = 0,
                Maximum = 360,
                MajorStep = 3.6,
                MinorStep = 1.8,
                IsAxisVisible = true,
                IsPanEnabled = true,
                //IsZoomEnabled = true,
                TickStyle = TickStyle.Outside,
                MinorTickSize = 2,
                MajorTickSize = 5,
                Unit = "°",
                FontSize = 10,
                TitleFontSize = 5,
            });
            model.Axes.Add(new MagnitudeAxis
            {
                Minimum = xFirst,
                Maximum = xLast,
                MajorStep = x.Length,
                MinorStep = x.Length,
            });
            model.Axes.Add(new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Palette = OxyPalettes.Rainbow(500),
                HighColor = OxyColors.Gray,
                LowColor = OxyColors.Black
            });
            model.Series.Add(new PolarHeatMapSeries
            {
                Data = matrix,
                Angle0 = 0,
                Angle1 = 360,
                Magnitude0 = xmin,
                Magnitude1 = xmax,
                Interpolate = true
            });

            return model;
        }
    }
}