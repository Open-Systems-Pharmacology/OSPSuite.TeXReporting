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
   public class ScatterPlotsSpecs : ContextForReporting
   {
      private int _figureCounter;
      protected override void Context()
      {
         base.Context();

         _pdfFileName = "ScatterPlots";
         _settings.DeleteWorkingDir = true;
         _settings.SaveArtifacts = true;
         _figureCounter = 0;

         var colors = new List<Color> { Color.Blue, Color.Red };
         _objectsToReport.Add(new Chapter("Scatter Plots"));

         var axisOptions = getAxisOptions();

         var plotOptions1 = getPlotOptions(Color.Blue.Name);
         var plotOptions2 = getPlotOptions(Color.Red.Name);

         var coordinatesMale = new List<Coordinate>
                                  {
                                     new Coordinate(0F, 5F),
                                     new Coordinate(2F, 15F),
                                     new Coordinate(4F, 8F),
                                     new Coordinate(7F, 4F),
                                     new Coordinate(12F, 24F)
                                  };
         var coordinatesFemale = new List<Coordinate>
                                    {
                                       new Coordinate(0F, 10F),
                                       new Coordinate(2F, 12F),
                                       new Coordinate(0F, 11F),
                                       new Coordinate(3F, 9F),
                                       new Coordinate(6F, 8F),
                                       new Coordinate(7F, 7F),
                                       new Coordinate(14F, 27F)
                                    };
         var boxplot1 = new Plot(coordinatesMale, plotOptions1) { LegendEntry = "Male" };
         var boxplot2 = new Plot(coordinatesFemale, plotOptions2) { LegendEntry = "Female" };
         var plotItem = new TeXReporting.Items.Plot(colors,
                                       axisOptions,
                                       new List<Plot> { boxplot1, boxplot2},
                                       new Text("Test Figure"));

         _objectsToReport.Add(new Section("Simple Scatter Plot"));
         _objectsToReport.Add(plotItem);
         _figureCounter++;
      }

      private static PlotOptions getPlotOptions(string color)
      {
         var plotOptions1 = new PlotOptions
         {
            LineStyle = PlotOptions.LineStyles.None,
            Marker = PlotOptions.Markers.Circle,
            MarkColor = color,
            Color = color,
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