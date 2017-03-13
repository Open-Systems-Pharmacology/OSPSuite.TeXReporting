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
   public class BarPlotsSpecs : ContextForReporting
   {
      private int _figureCounter;
      protected override void Context()
      {
         base.Context();

         _pdfFileName = "BarPlots";
         _settings.DeleteWorkingDir = true;
         _settings.SaveArtifacts = true;
         _figureCounter = 0;

         var colors = new List<Color> { Color.Blue, Color.Red };
         _objectsToReport.Add(new Chapter("Bar Plots"));

         var plotOptions1 = getPlotOptions(Color.Blue.Name);
         var plotOptions2 = getPlotOptions(Color.Red.Name);

         var coordinates1 = new List<Coordinate> { new Coordinate(1F, 5F), new Coordinate(2F, 10F), new Coordinate(3F,5F) };
         var barplot1 = new Plot(coordinates1, plotOptions1) { LegendEntry = "Male" };
         var coordinates2 = new List<Coordinate> { new Coordinate(1F, 3F), new Coordinate(2F, 8F) , new Coordinate(3F, 9F)};
         var barplot2 = new Plot(coordinates2, plotOptions2) { LegendEntry = "Female" };

         // simple bar plot
         var barPlotOptionsSideBySide = new BarPlotOptions
                                  {
                                     BarPlotType = BarPlotOptions.BarPlotTypes.SideBySide,
                                     Width = "0.45",
                                     Shift = Helper.Length(2,Helper.MeasurementUnits.pt),
                                     NodesNearCoords = true
                                  };

         _objectsToReport.Add(new Section("Simple Bar Plot"));
         _objectsToReport.Add(new BarPlot(colors,
                                     getAxisOptions(LegendOptions.LegendPositions.OuterNorthEast), barPlotOptionsSideBySide,
                                     new List<Plot> { barplot1, barplot2 },
                                     new Text("Test Figure")));
         _figureCounter++;

         _objectsToReport.Add(new Section("Simple Bar Plot With Legend Inside"));
         _objectsToReport.Add(new BarPlot(colors,
                                     getAxisOptions(LegendOptions.LegendPositions.NorthEast), barPlotOptionsSideBySide,
                                     new List<Plot> { barplot1, barplot2 },
                                     new Text("Test Figure")));
         _figureCounter++;

         // simple stacked bar plot
         var barPlotOptionsStacked = new BarPlotOptions
                                  {
                                     BarPlotType = BarPlotOptions.BarPlotTypes.Stacked,
                                     Width = "0.9"
                                  };

         _objectsToReport.Add(new Section("Simple Stacked Bar Plot"));
         _objectsToReport.Add(new BarPlot(colors,
                                     getAxisOptions(LegendOptions.LegendPositions.OuterNorthEast), barPlotOptionsStacked,
                                     new List<Plot> { barplot1, barplot2 },
                                     new Text("Test Figure")));
         _figureCounter++;

         _objectsToReport.Add(new Section("Simple Stacked Bar Plot With Legend Inside"));
         _objectsToReport.Add(new BarPlot(colors,
                                     getAxisOptions(LegendOptions.LegendPositions.NorthEast), barPlotOptionsStacked,
                                     new List<Plot> { barplot1, barplot2 },
                                     new Text("Test Figure")));
         _figureCounter++;

         // simple interval bar plot
         var barPlotOptionsInterval = new BarPlotOptions
         {
            BarPlotType = BarPlotOptions.BarPlotTypes.Interval,
            TickLabelIntervalBoundaries = true
         };

         // add dummy coordinate 
         var coordinatesInt1 = new List<Coordinate>(coordinates1);
         var coordinatesInt2 = new List<Coordinate>(coordinates2);

         coordinatesInt1.Add(new Coordinate(4F, 0F));
         coordinatesInt2.Add(new Coordinate(4F, 0F));
         var barplotInt1 = new Plot(coordinatesInt1, plotOptions1) { LegendEntry = "Male" };
         var barplotInt2 = new Plot(coordinatesInt2, plotOptions2) { LegendEntry = "Female" };

         _objectsToReport.Add(new Section("Simple Interval Bar Plot"));
         _objectsToReport.Add(new BarPlot(colors,
                                     getAxisOptionsForInterval(LegendOptions.LegendPositions.OuterNorthEast), barPlotOptionsInterval,
                                     new List<Plot> { barplotInt1, barplotInt2 },
                                     new Text("Test Figure")));
         _figureCounter++;

         _objectsToReport.Add(new Section("Simple Interval Bar Plot With Legend Inside"));
         _objectsToReport.Add(new BarPlot(colors,
                                     getAxisOptionsForInterval(LegendOptions.LegendPositions.NorthEast), barPlotOptionsInterval,
                                     new List<Plot> { barplotInt1, barplotInt2 },
                                     new Text("Test Figure")));
         _figureCounter++;

         // simple stacked interval bar plot
         var barPlotOptionsIntervalStacked = new BarPlotOptions
         {
            BarPlotType = BarPlotOptions.BarPlotTypes.IntervalStacked,
            TickLabelIntervalBoundaries = true
         };

         _objectsToReport.Add(new Section("Simple Stacked Interval Bar Plot"));
         _objectsToReport.Add(new BarPlot(colors,
                                     getAxisOptionsForInterval(LegendOptions.LegendPositions.OuterNorthEast), 
                                     barPlotOptionsIntervalStacked,
                                     new List<Plot> { barplotInt1, barplotInt2 },
                                     new Text("Test Figure")));
         _figureCounter++;

         _objectsToReport.Add(new Section("Simple Stacked Interval Bar Plot With Legend Inside"));
         _objectsToReport.Add(new BarPlot(colors,
                                     getAxisOptionsForInterval(LegendOptions.LegendPositions.NorthEast), 
                                     barPlotOptionsIntervalStacked,
                                     new List<Plot> { barplotInt1, barplotInt2 },
                                     new Text("Test Figure")));
         _figureCounter++;
      }

      private PlotOptions getPlotOptions(string color)
      {
         return new PlotOptions {Color = color, FillColor = String.Format("{0}!{1}", color, 40),
            PlotType = PlotOptions.PlotTypes.BarPlot, 
            LineStyle = PlotOptions.LineStyles.Solid};
      }

      private static AxisOptions getAxisOptionsForInterval(LegendOptions.LegendPositions legendPosition)
      {
         var axisOptions = new AxisOptions(NoConverter.Instance)
         {
            Title = "Test Title",
            YLabel = "Y Axis",
            YMode = AxisOptions.AxisMode.normal,
            YMin = 0F,
            XTicks = new[] { 1F, 2F, 3F, 4F },
            XAxisPosition = AxisOptions.AxisXLine.bottom,
            YAxisPosition = AxisOptions.AxisYLine.left,
            YMajorGrid = true,
            YAxisArrow = true,
            EnlargeLimits = false,
            LegendOptions = new LegendOptions { Columns = 1, LegendPosition = legendPosition }
         };

         return axisOptions;
      }

      private static AxisOptions getAxisOptions(LegendOptions.LegendPositions legendPosition)
      {
         var axisOptions = new AxisOptions(NoConverter.Instance)
         {
            Title = "Test Title",
            YLabel = "Y Axis",
            YMode = AxisOptions.AxisMode.normal,
            YMin = 0F,
            XMin = 0F,
            XMax = 4F,
            XTicks = new[] { 1F, 2F, 3F},
            XTickLabels = new[] {"Group A", "Group B", "Group C"},
            XAxisPosition = AxisOptions.AxisXLine.bottom,
            YAxisPosition = AxisOptions.AxisYLine.left,
            YMajorGrid = true,
            YAxisArrow = true,
            EnlargeLimits = false,
            LegendOptions = new LegendOptions { Columns = 1, LegendPosition = legendPosition }
         };
         axisOptions.GroupLines.Add(new AxisOptions.GroupLine(1F, 2F, "A-B", 1));

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