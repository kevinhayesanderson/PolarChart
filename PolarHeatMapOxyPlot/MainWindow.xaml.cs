using OxyPlot;
using OxyPlot.Axes;
using System.Windows;

namespace PolarHeatMapOxyPlot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public PlotModel MyModel { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            //this.MyModel = new PlotModel { Title = "Example 1" };
            //this.MyModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));

            MyModel = PolarHeatMapInterpolated();
        }

        public PlotModel PolarHeatMapInterpolated()
        {
            var model = new PlotModel { Title = "Polar heat map (interpolated)", PlotMargins = new OxyThickness(40, 80, 40, 40), PlotType = PlotType.Polar, PlotAreaBorderThickness = new OxyThickness(0) };

            var matrix = new double[2, 2];
            matrix[0, 0] = 0;
            matrix[0, 1] = 2;
            matrix[1, 0] = 1.5;
            matrix[1, 1] = 0.2;

            // generate 1d normal distribution
            var singleData = new double[100];
            for (int x = 0; x < 100; ++x)
            {
                singleData[x] = Math.Exp((-1.0 / 2.0) * Math.Pow((x - 50.0) / 20.0, 2.0));
            }

            // generate 2d normal distribution
            var data = new double[100, 100];
            for (int x = 0; x < 100; ++x)
            {
                for (int y = 0; y < 100; ++y)
                {
                    data[y, x] = singleData[x] * singleData[(y + 30) % 100] * 100;
                }
            }

            model.Axes.Add(new AngleAxis { StartAngle = Math.PI, EndAngle = Math.PI + 360, Minimum = 0, Maximum = 360, MajorStep = 30, MinorStep = 15 });
            model.Axes.Add(new MagnitudeAxis { Minimum = 0, Maximum = 100, MajorStep = 25, MinorStep = 5 });
            model.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Rainbow(500), HighColor = OxyColors.Gray, LowColor = OxyColors.Black });
            model.Series.Add(new PolarHeatMapSeries { Data = data, Angle0 = 0, Angle1 = 360, Magnitude0 = 0, Magnitude1 = 100, Interpolate = true });

            return model;
        }

    }


}