using System;
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
   public class GroupPlotsSpecs : ContextForReporting
   {
      private int _figureCounter;
      protected override void Context()
      {
         base.Context();

         _pdfFileName = "GroupedPlots";
         _settings.DeleteWorkingDir = true;
         _settings.SaveArtifacts = true;
         _figureCounter = 0;

         var colors = new List<Color> { Color.Blue, Color.Red };
         _objectsToReport.Add(new Chapter("Box Plots Grouped"));

         var plotItem = getGroupPlot(colors, LegendOptions.LegendPositions.OuterSouth, 2);
         _objectsToReport.Add(new Section("Simple Grouped Plot"));
         _objectsToReport.Add(plotItem);
         _figureCounter++;

         var plotItem2 = getGroupPlot(colors, LegendOptions.LegendPositions.OuterSouth, 2);
         var section = new Section("Simple Grouped Plot in Landscape");
         plotItem2.Landscape = true;
         _objectsToReport.Add(new InLandscape(new object[] { section, plotItem2 }));
         _figureCounter++;

         var plotItem3 = getGroupPlot(colors, LegendOptions.LegendPositions.OuterNorthEast, 1);
         _objectsToReport.Add(new Section("Simple Grouped Plot With Legend Beside"));
         _objectsToReport.Add(plotItem3);
         _figureCounter++;

         var plotItem4 = getGroupPlot(colors, LegendOptions.LegendPositions.OuterNorthEast, 1);
         var section2 = new Section("Simple Grouped Plot in Landscape With Legend Beside");
         plotItem4.Landscape = true;
         _objectsToReport.Add(new InLandscape(new object[] { section2, plotItem4 }));
         _figureCounter++;

         //Legend Position test
         _objectsToReport.Add(new Chapter("Test of different legend position"));

         foreach (var legendPosition in Enum.GetValues(typeof(LegendOptions.LegendPositions)))
         {
            var legendPlotItem = getGroupPlot(colors, (LegendOptions.LegendPositions)legendPosition, 1);
            _objectsToReport.Add(new Section(legendPosition.ToString()));
            _objectsToReport.Add(legendPlotItem);
            _figureCounter++;
         }

      }

      private static GroupPlot getGroupPlot(List<Color> colors, LegendOptions.LegendPositions legendPosition, int columns)
      {
         var axisOptionsForGroup = getAxisOptionsForGroup();

         var plotOptions1 = getBoxPlotOptions(15F, 20F, 25F, 30F, 35F, 1F, Color.Blue.Name);
         var plotOptions2 = getBoxPlotOptions(26F, 31F, 34F, 41F, 44F, 1F, Color.Red.Name);
         var plotOptions3 = getBoxPlotOptions(18F, 22F, 27F, 32F, 38F, 2F, Color.Blue.Name);
         var plotOptions4 = getBoxPlotOptions(27F, 31F, 35F, 39F, 47F, 2F, Color.Red.Name);

         var coordinates = new List<Coordinate> {new Coordinate(0F, 5F)};
         var boxplot1 = new Plot(coordinates, plotOptions1);
         var boxplot2 = new Plot(coordinates, plotOptions2);
         var boxplot3 = new Plot(new List<Coordinate>(), plotOptions3);
         var boxplot4 = new Plot(new List<Coordinate>(), plotOptions4);

         var groupOptions = getGroupOptions(legendPosition, columns);

         var groupedPlots = new List<IBasePlot>();
         var plots = new List<Plot> {boxplot1, boxplot2, boxplot3, boxplot4};
         var axisOptionsForPlotWithoutGroupLines = getAxisOptionsForPlot();
         var axisOptionsForPlot = getAxisOptionsForPlot();
         axisOptionsForPlot.GroupLines.Add(new AxisOptions.GroupLine(1F, 2F, "All", 1));
         axisOptionsForPlot.GroupLines.Add(new AxisOptions.GroupLine(1F, 2F, "Total", 2));

         var groupedPlot1 = new BasePlot(new AxisOptions(NoConverter.Instance) {IsEmptyGroupPlot = true}, new List<Plot>());
         var groupedPlot2 = new BasePlot(axisOptionsForPlotWithoutGroupLines, plots);
         var groupedPlot3 = new BasePlot(axisOptionsForPlot, plots);

         groupedPlots.Add(groupedPlot1);
         groupedPlots.Add(groupedPlot2);
         groupedPlots.Add(groupedPlot3);
         groupedPlots.Add(groupedPlot3);

         var plotItem = new GroupPlot(colors,
                                      axisOptionsForGroup,
                                      groupOptions,
                                      groupedPlots,
                                      new Text("Test Figure With Grouped Plots")) {Position = FigureWriter.FigurePositions.H};
         return plotItem;
      }

      private static AxisOptions getAxisOptionsForPlot()
      {
         var plotAxisOptionsWithoutGroupLines = new AxisOptions(NoConverter.Instance)
                                                   {
                                                      Title = "Test Title using = and ,."
                                                   };
         return plotAxisOptionsWithoutGroupLines;
      }

      private static GroupOptions getGroupOptions(LegendOptions.LegendPositions legendPosition, int columns)
      {
         var groupOptions = new GroupOptions
                               {
                                  Title = "This is my Group Title using = and ,.",
                                  Columns = 2,
                                  Rows = 2,
                                  HorizontalSep = Helper.Length(2, Helper.MeasurementUnits.em),
                                  VerticalSep = Helper.Length(8, Helper.MeasurementUnits.ex),
                                  XLabelsAt = GroupOptions.GroupXPositions.EdgeBottom,
                                  XTickLabelsAt = GroupOptions.GroupXPositions.EdgeBottom,
                                  YLabelsAt = GroupOptions.GroupYPositions.EdgeLeft,
                                  YTickLabelsAt = GroupOptions.GroupYPositions.EdgeLeft,
                                  GroupLegendOptions = new GroupLegendOptions
                                  {
                                     LegendEntries = new[] { "Male", "Female" },
                                     LegendOptions = new LegendOptions
                                     {
                                        Columns = columns,
                                        LegendPosition = legendPosition
                                     }
                                  }
                               };
         return groupOptions;
      }

      private static PlotOptions getBoxPlotOptions(float lowerWhisker, float lowerQuartile, float median, float upperQuartile, float upperWhisker, float drawPosition, string color)
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

      private static AxisOptions getAxisOptionsForGroup()
      {
         var axisOptions = new AxisOptions(NoConverter.Instance)
                              {
                                 YLabel = "Concentration at t=100h",
                                 IsUsedForBoxPlots = true,
                                 BoxPlotDrawDirection = AxisOptions.BoxPlotDrawDirections.y,
                                 YMode = AxisOptions.AxisMode.normal,
                                 XTicks = new[] {1F, 2F},
                                 XTickLabels = new[] {"Group A", "Group B"},
                                 XAxisPosition = AxisOptions.AxisXLine.bottom,
                                 YAxisPosition = AxisOptions.AxisYLine.left,
                                 YMajorGrid = true,
                                 YAxisArrow = true,
                                 EnlargeLimits = true
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