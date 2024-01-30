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

            model.Axes.Add(new AngleAxis { StartAngle = Math.PI, EndAngle = Math.PI + 360, Minimum = 0, Maximum = 360, MajorStep = 30, MinorStep = 15 });
            model.Axes.Add(new MagnitudeAxis { Minimum = 0, Maximum = 100, MajorStep = 25, MinorStep = 5 });
            model.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Rainbow(500), HighColor = OxyColors.Gray, LowColor = OxyColors.Black });
            model.Series.Add(new PolarHeatMapSeries { Data = matrix, Angle0 = 0, Angle1 = 360, Magnitude0 = 0, Magnitude1 = 100, Interpolate = true });

            return model;
        }

    }


}