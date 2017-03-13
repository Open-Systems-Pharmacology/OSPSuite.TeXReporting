using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.TeXReporting.TeX.PGFPlots;
using Plot = OSPSuite.TeXReporting.TeX.PGFPlots.Plot;

namespace OSPSuite.TeXReporting.Tests
{
   public class RangePlotsSpecs : ContextForReporting
   {
      private int _figureCounter;
      protected override void Context()
      {
         base.Context();

         _pdfFileName = "RangePlots";
         _settings.DeleteWorkingDir = true;
         _settings.SaveArtifacts = true;
         _figureCounter = 0;

         var colors = new List<Color> { Color.Blue, Color.Red };
         _objectsToReport.Add(new Chapter("Range Plots"));

         var axisOptions = getAxisOptions();

         var coordinatesMale = new List<Coordinate>
                                  {
                                     new Coordinate(0F, 5F) {errY=0.4F, errY2 = 0.5F},
                                     new Coordinate(2F, 15F) {errY=0.4F, errY2 = 0.1F},
                                     new Coordinate(4F, 8F) {errY=1.4F, errY2 = 1.5F},
                                     new Coordinate(7F, 4F) {errY=0.4F, errY2 = 0.2F},
                                     new Coordinate(12F, 24F) {errY=0.4F, errY2 = 0.5F}
                                  };
         var coordinatesFemale = new List<Coordinate>
                                    {
                                       new Coordinate(0F, 10F) {errY=0.4F, errY2 = 0.5F},
                                       new Coordinate(2F, 12F) {errY=1.4F, errY2 = 1.3F},
                                       new Coordinate(4F, 11F) {errY=2.4F, errY2 = 1.5F},
                                       new Coordinate(5F, 9F) {errY=0.8F, errY2 = 0.7F},
                                       new Coordinate(6F, 8F) {errY=0.4F, errY2 = 0.1F},
                                       new Coordinate(7F, 7F) {errY=0.4F, errY2 = 0.1F},
                                       new Coordinate(14F, 27F) {errY=3.4F, errY2 = 2.5F}
                                    };
         var plotRangeMale = new Plot(coordinatesMale, getPlotOptions(Color.Blue.Name, true, false));
         var plotMale = new Plot(coordinatesMale, getPlotOptions(Color.Blue.Name, false, true)) { LegendEntry = "Male" };
         var plotRangeFemale = new Plot(coordinatesFemale, getPlotOptions(Color.Red.Name, true, false));
         var plotFemale = new Plot(coordinatesFemale, getPlotOptions(Color.Red.Name, false, true)) { LegendEntry = "Female" };
         var plotItem = new TeXReporting.Items.Plot(colors,
                                       axisOptions,
                                       new List<Plot> { plotRangeMale, plotMale, plotRangeFemale, plotFemale },
                                       new Text("Test Figure"));

         _objectsToReport.Add(new Section("Simple Range Plot"));
         _objectsToReport.Add(plotItem);
         _figureCounter++;
      }

      private static PlotOptions getPlotOptions(string color, bool shadedErrorBars, bool showInLegend)
      {
         var plotOptions1 = new PlotOptions
         {
            PlotType = PlotOptions.PlotTypes.LinearPlot,
            ShadedErrorBars = shadedErrorBars,
            LineStyle = PlotOptions.LineStyles.Solid,
            Marker = PlotOptions.Markers.None,
            MarkColor = color,
            Color = color,
            ShowInLegend = showInLegend,
            ThicknessSize = Helper.Length(1, Helper.MeasurementUnits.pt)
         };
         return plotOptions1;
      }

      private static AxisOptions getAxisOptions()
      {
         var axisOptions = new AxisOptions(NoConverter.Instance)
         {
            Title = "Test Title",
            YLabel = "Y Axis",
            YMode = AxisOptions.AxisMode.normal,
            XAxisPosition = AxisOptions.AxisXLine.bottom,
            YAxisPosition = AxisOptions.AxisYLine.left,
            YMajorGrid = true,
            YAxisArrow = true,
            XAxisArrow = true,
            EnlargeLimits = true,
            LegendOptions = new LegendOptions { Columns = 1, LegendPosition = LegendOptions.LegendPositions.OuterNorthEast }
         };

         return axisOptions;
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }

      [Observation]
      public void should_create_artifacts_directory()
      {
         var artifactsFolder = Path.Combine(_outputDir, string.Concat(_pdfFileName, "_", Constants.ArtifactOutputFolder));
         Directory.Exists(artifactsFolder).ShouldBeTrue();
      }

      [Observation]
      public void should_create_artifacts_directory_plots_folder()
      {
         var artifactsFolder = Path.Combine(_outputDir, string.Concat(_pdfFileName, "_", Constants.ArtifactOutputFolder));
         var outputFolder = Path.Combine(artifactsFolder, Constants.ArtifactFolderForPlots);
         Directory.Exists(outputFolder).ShouldBeTrue();
      }

      [Observation]
      public void should_create_plots_as_pdf()
      {
         var artifactsFolder = Path.Combine(_outputDir, string.Concat(_pdfFileName, "_", Constants.ArtifactOutputFolder));
         var outputFolder = Path.Combine(artifactsFolder, Constants.ArtifactFolderForPlots);
         var dir = new DirectoryInfo(outputFolder);
         dir.GetFiles("*.pdf").Length.ShouldBeEqualTo(_figureCounter);
      }

      [Observation]
      public void should_create_plots_as_png()
      {
         var artifactsFolder = Path.Combine(_outputDir, string.Concat(_pdfFileName, "_", Constants.ArtifactOutputFolder));
         var outputFolder = Path.Combine(artifactsFolder, Constants.ArtifactFolderForPlots);
         var dir = new DirectoryInfo(outputFolder);
         dir.GetFiles("*.png").Length.ShouldBeEqualTo(_figureCounter);
      }

   }
}