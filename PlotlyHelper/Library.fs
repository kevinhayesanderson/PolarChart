namespace PlotlyHelper

open Plotly.NET
open Plotly.NET.ImageExport
open Plotly.NET.LayoutObjects
open Plotly.NET.TraceObjects

module ChartInitializer =

    let PointDensity x y width height =
        Defaults.DefaultTemplate <- ChartTemplates.lightMirrored
        Defaults.DefaultWidth <- width
        Defaults.DefaultHeight<- height
        Defaults.DefaultDisplayOptions <- DisplayOptions.init(PlotlyJSReference = Plotly.NET.PlotlyJSReference.Full)
        PuppeteerSharpRendererOptions.launchOptions.Timeout <- 0
        let pointDensityChart = Chart.PointDensity(x = x, y = y,
                                                PointMarkerColor = Color.fromKeyword White,
                                                PointMarkerSymbol = StyleParam.MarkerSymbol.Circle,
                                                PointMarkerSize= 2,
                                                ColorScale= StyleParam.Colorscale.Greens,
                                                ColorBar= ColorBar.init (Title = Title.init("Density")),
                                                ShowContourLabels= true)
        pointDensityChart

    let PointPolar r theta width height =
        Defaults.DefaultTemplate <- ChartTemplates.lightMirrored
        Defaults.DefaultWidth <- width
        Defaults.DefaultHeight<- height
        Defaults.DefaultDisplayOptions <- DisplayOptions.init(PlotlyJSReference = Plotly.NET.PlotlyJSReference.Full)
        PuppeteerSharpRendererOptions.launchOptions.Timeout <- 0
        let radialAxis = RadialAxis.init(Angle = 0)
        let angularAxis = AngularAxis.init(Rotation = 270)
        let pointPolar = Chart.PointPolar(r = r, theta = theta,
                                          Marker = Marker.init(Size = 3, Color = Color.fromKeyword Green),
                                          UseDefaults = false,
                                          UseWebGL = true,
                                          ShowLegend = true)
                            |> Chart.withRadialAxis(radialAxis)
                            |> Chart.withAngularAxis(angularAxis)
        pointPolar