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
   public class BoxPlotsSpecs : ContextForReporting
   {
      private int _figureCounter;
      protected override void Context()
      {
         base.Context();

         _pdfFileName = "BoxPlots";
         _settings.DeleteWorkingDir = true;
         _settings.SaveArtifacts = true;
         _figureCounter = 0;

         var colors = new List<Color> { Color.Blue, Color.Red };
         _objectsToReport.Add(new Chapter("Box Plots"));

         var axisOptions = getAxisOptions();

         var plotOptions1 = getBoxWhiskerPlotOptions(15F, 20F, 25F, 30F, 35F, 1F, Color.Blue.Name);
         var plotOptions2 = getBoxWhiskerPlotOptions(26F,31F,34F,41F,44F,1F, Color.Red.Name);
         var plotOptions3 = getBoxWhiskerPlotOptions(18F, 22F, 27F, 32F, 38F, 2F, Color.Blue.Name);
         var plotOptions4 = getBoxWhiskerPlotOptions(27F, 31F, 35F, 39F, 47F, 2F, Color.Red.Name);
         
         var coordinates = new List<Coordinate> {new Coordinate(0F, 5F)};
         var boxplot1 = new Plot(coordinates, plotOptions1) {LegendEntry = "Male"};
         var boxplot2 = new Plot(coordinates, plotOptions2) { LegendEntry = "Female" };
         var boxplot3 = new Plot(new List<Coordinate>(), plotOptions3);
         var boxplot4 = new Plot(new List<Coordinate>(), plotOptions4);
         var plotItem = new TeXReporting.Items.Plot(colors,
                                       axisOptions,
                                       new List<Plot> {boxplot1, boxplot2, boxplot3, boxplot4},
                                       new Text("Test Figure"));

         _objectsToReport.Add(new Section("Simple Box Plot"));
         _objectsToReport.Add(plotItem);
         _figureCounter++;
      }

      private static PlotOptions getBoxWhiskerPlotOptions(float lowerWhisker, float lowerQuartile, float median, float upperQuartile, float upperWhisker, float drawPosition, string color)
      {
         var plotOptions1 = new PlotOptions
                               {
                                  LineStyle = PlotOptions.LineStyles.Solid,
                                  Marker = PlotOptions.Markers.Circle,
                                  MarkColor = color,
                                  Color = color,
                                  BoxPlotPrepared = new BoxPlotPrepared(lowerWhisker, lowerQuartile, median, upperQuartile, upperWhisker, drawPosition),
                                  ThicknessSize = Helper.Length(1, Helper.MeasurementUnits.pt)
                               };
         return plotOptions1;
      }

      private static AxisOptions getAxisOptions()
      {
         var axisOptions = new AxisOptions(NoConverter.Instance)
                              {
                                 Title = "Test Title",
                                 YLabel = "Y Axis with = sign",
                                 IsUsedForBoxPlots = true,
                                 BoxPlotDrawDirection = AxisOptions.BoxPlotDrawDirections.y,
                                 YMode = AxisOptions.AxisMode.normal,
                                 XTicks = new[] {1F, 2F},
                                 XTickLabels = new[] {"Group A, with special character", "Group B"},
                                 XAxisPosition = AxisOptions.AxisXLine.bottom,
                                 YAxisPosition = AxisOptions.AxisYLine.left,
                                 YMajorGrid = true,
                                 YAxisArrow = true,
                                 EnlargeLimits = true,
                                 LegendOptions = new LegendOptions {Columns = 2, LegendPosition = LegendOptions.LegendPositions.South}
                              };
         axisOptions.GroupLines.Add(new AxisOptions.GroupLine(1F, 2F, "All", 1));
         axisOptions.GroupLines.Add(new AxisOptions.GroupLine(1F, 2F, "Total", 2));

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