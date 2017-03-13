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
   public class PlotsSpecs : ContextForReporting
   {
      private int _figureCounter;
      protected override void Context()
      {
         base.Context();

         _pdfFileName = "Plots";
         _settings.DeleteWorkingDir = true;
         _settings.SaveArtifacts = true;
         _figureCounter = 0;

         PlotFigures("Test difference less than 1", new List<Coordinate>
                                {
                                   new Coordinate(2F, 4F),
                                   new Coordinate(3F, 4.78F) {errY = 0.4F}
                                });

         PlotFigures("Test difference less than 1 in same dimension", new List<Coordinate>
                                {
                                   new Coordinate(2F, 4.134F),
                                   new Coordinate(3F, 4.222F) {errY = 0.04F}
                                });

         PlotFigures("Test difference less than 1 in same dimension bigger scale", new List<Coordinate>
                                {
                                   new Coordinate(2F, 1821F),
                                   new Coordinate(3F, 1232F) {errY = 40F}
                                });

         PlotFigures("Test difference less than 1 in same dimension bigger scale", new List<Coordinate>
                                {
                                   new Coordinate(2F, 413.4F),
                                   new Coordinate(3F, 498.2F) {errY = 4F}
                                });

         PlotFigures("Test less than 10", new List<Coordinate>
                                             {
                                                new Coordinate(2F, 4F),
                                                new Coordinate(3F, 8F) {errY = 0.4F}
                                             });

         PlotFigures("Test less than 20", new List<Coordinate>
                                             {
                                                new Coordinate(2F, 4F),
                                                new Coordinate(3F, 11F) {errY = 0.4F}
                                             });

         PlotFigures("Test from 0 to 100", new List<Coordinate>
                                              {
                                                 new Coordinate(2F, 4F),
                                                 new Coordinate(3F, 81F) {errY = 4F}
                                              });

         PlotFigures("Test from 0 to 1000", new List<Coordinate>
                                              {
                                                 new Coordinate(2F, 4F),
                                                 new Coordinate(3F, 810F) {errY = 40F}
                                              });

         PlotFigures("Test from 0 to 10 with much between 0 and 1", new List<Coordinate>
                                              {
                                                 new Coordinate(0F, 8F),
                                                 new Coordinate(0.1F, 7F),
                                                 new Coordinate(0.2F, 5F),
                                                 new Coordinate(0.2F, 5F),
                                                 new Coordinate(0.3F, 2F),
                                                 new Coordinate(0.4F, 1F) {errY = 0.1F},
                                                 new Coordinate(1F, 0.8F),
                                                 new Coordinate(2F, 0.2F),
                                                 new Coordinate(12F, 0.1F)
                                              });

         PlotFigures("Test from 0 to 10^15", new List<Coordinate>
                                              {
                                                 new Coordinate(2F, 4F),
                                                 new Coordinate(3F, 8.1e15F) {errY = 4e14F}
                                              }, new Text("Test from 0 to {0}", 
                                                 new Text(@"\boldmath $10^{15}$ \unboldmath") { Converter = NoConverter.Instance }));

         PlotFigures("Test from 10^9 to 10^15", new List<Coordinate>
                                              {
                                                 new Coordinate(2F, 4e9F),
                                                 new Coordinate(3F, 8.1e15F) {errY = 4e14F}
                                              }, new Text("Test from {0} to {1}",
                                                 new Text(@"\boldmath $10^9$ \unboldmath") { Converter = NoConverter.Instance }, 
                                                 new Text(@"\boldmath $10^{15}$ \unboldmath") { Converter = NoConverter.Instance }));

         PlotFigures("Test from 10^12 to 10^15", new List<Coordinate>
                                              {
                                                 new Coordinate(2F, 4e12F),
                                                 new Coordinate(3F, 8.1e15F) {errY = 4e14F}
                                              }, new Text("Test from {0} to {1}",
                                                 new Text(@"\boldmath $10^{12}$ \unboldmath") { Converter = NoConverter.Instance },
                                                 new Text(@"\boldmath $10^{15}$ \unboldmath") { Converter = NoConverter.Instance }));

         PlotFigures("Test Profile", new List<Coordinate>
                                        {
                                           //new Coordinate(0F, 0F),
                                           new Coordinate(0.5F, 21F) {errY = 2.1F},
                                           new Coordinate(1F, 81F) {errY = 4F},
                                           new Coordinate(2F, 41F) {errY = 2F},
                                           new Coordinate(3F, 11F) {errY = 0.4F},
                                           new Coordinate(4F, 2F) {errY = 0.3F},
                                           new Coordinate(10F, 1F) {errY = 0.2F}
                                        });
      }

      private void PlotFigures(string chapterName, List<Coordinate> coordinates, Text chapterTitle = null)
      {
         if (chapterTitle == null) 
            chapterTitle = new Text(chapterName);

         _objectsToReport.Add(new Chapter(chapterName, chapterTitle));

         var colors = new List<Color>() {Color.Blue, Color.Red};

         var axisOptions = new AxisOptions(NoConverter.Instance)
                              {
                                 Title = "Test Title",
                                 XLabel = "X Axis",
                                 YLabel = "Y Axis",
                                 YMode = AxisOptions.AxisMode.log,
                                 LogTicksWithFixedPoint = true,
                                 XAxisPosition = AxisOptions.AxisXLine.bottom,
                                 YAxisPosition = AxisOptions.AxisYLine.left,
                                 LegendOptions = new LegendOptions {LegendPosition = LegendOptions.LegendPositions.NorthWest},
                                 YMajorGrid = true
                              };

         var plotOptions1 = new PlotOptions
                               {
                                  LineStyle = PlotOptions.LineStyles.Solid,
                                  Marker = PlotOptions.Markers.Triangle,
                                  Color = Color.Blue.Name,
                                  ErrorBars = true,
                                  ShadedErrorBars = true,
                                  Thickness = PlotOptions.Thicknesses.UltraThick
                               };
         var plot1 = new Plot(coordinates, plotOptions1) {LegendEntry = "legend entry 1"};
         var plotItem = new TeXReporting.Items.Plot(colors, axisOptions, new List<Plot> {plot1}, new Text("Test Figure"));

         var plotOptions2 = new PlotOptions
                               {
                                  LineStyle = PlotOptions.LineStyles.DashDotted,
                                  Marker = PlotOptions.Markers.Circle,
                                  Color = Color.Red.Name,
                                  ErrorBars = true,
                                  ThicknessSize = Helper.Length(2, Helper.MeasurementUnits.pt),
                                  MarkSize = Helper.Length(3, Helper.MeasurementUnits.pt),
                                  MarkColor = Color.Blue.Name,
                                  MarkFillColor = String.Format("{0}!{1}", Color.Blue.Name, 20)
                               };
         var plot2 = new Plot(coordinates, plotOptions2)
                        {
                           LegendEntry =
                              "0123456789 0123456789 0123456789 0123456789 0123456789 0123456789"
                        };
         var plotItem2 = new TeXReporting.Items.Plot(colors, axisOptions, new List<Plot> {plot2}, new Text("Test Figure 2"));

         // Two Ordinates Plot
         var axisOptionsY1 = new AxisOptions(NoConverter.Instance)
                                {
                                   EnlargeLimits = true,
                                   Title = "Test Title",
                                   XLabel = "X Axis",
                                   YLabel = "Y Axis",
                                   YMin = 0,
                                   XTickMin = 2F,
                                   XTickMax = 3F,
                                   MinorXTickNum = 1,
                                   MinorYTickNum = 1,
                                   BackgroundColor = "blue!10",
                                   YMode = AxisOptions.AxisMode.normal,
                                   XAxisPosition = AxisOptions.AxisXLine.bottom,
                                   YAxisPosition = AxisOptions.AxisYLine.left,
                                   YMajorGrid = true
                                };
         var axisOptionsY2 = new AxisOptions(DefaultConverter.Instance)
                                {
                                   YLabel = "Y2 Axis",
                                   YAxisPosition = AxisOptions.AxisYLine.right,
                                   YAxisArrow = false,
                                   YScaledTicks = false,
                                   YMode = AxisOptions.AxisMode.log,
                                   LegendOptions =
                                      new LegendOptions
                                         {
                                            FontSize = LegendOptions.FontSizes.scriptsize,
                                            LegendAlignment = LegendOptions.LegendAlignments.left,
                                            LegendPosition = LegendOptions.LegendPositions.OuterNorthWest,
                                            RoundedCorners = true,
                                            Columns = 1,
                                            //TextWidth = Helper.GetWidthInPercentageOfTextWidth(30)
                                         },
                                   XAxisPosition = AxisOptions.AxisXLine.none
                                };
         var plotTwoOrdinates = new PlotTwoOrdinates(colors, axisOptionsY1, new List<Plot> {plot1},
                                                     axisOptionsY2, new List<Plot> {plot2},
                                                     new Text("Test Two Ordinates"));

         var axisOptionsY3 = new AxisOptions(DefaultConverter.Instance)
                                {
                                   YLabel = "Y3 Axis",
                                   YAxisPosition = AxisOptions.AxisYLine.right,
                                   YAxisArrow = false,
                                   YScaledTicks = true,
                                   YMin = 0,
                                   YDiscontinuity = AxisOptions.Discontinuities.parallel,
                                   YMode = AxisOptions.AxisMode.normal,
                                   LegendOptions =
                                      new LegendOptions
                                         {
                                            FontSize = LegendOptions.FontSizes.scriptsize,
                                            LegendAlignment = LegendOptions.LegendAlignments.left,
                                            LegendPosition = LegendOptions.LegendPositions.OuterNorthEast,
                                            RoundedCorners = true,
                                            Columns = 1,
                                            //TextWidth = Helper.GetWidthInPercentageOfTextWidth(30)
                                         },
                                   XAxisPosition = AxisOptions.AxisXLine.none
                                };

         var plot3 = new Plot(new List<Coordinate> {new Coordinate(2.5F, 5F)},
                                           new PlotOptions {Marker = PlotOptions.Markers.Triangle}) {LegendEntry = "Points"};

         var plotThreeOrdinates = new PlotThreeOrdinates(colors, axisOptionsY1, new List<Plot> {plot1},
                                                         axisOptionsY2, new List<Plot> {plot2},
                                                         axisOptionsY3, new List<Plot> {plot3},
                                                         new Text("Test Three Ordinates"));


         _objectsToReport.Add(new Section("Simple Figure 1"));
         var axisOptionsNormal = new AxisOptions(NoConverter.Instance)
         {
            Title = "Test Title",
            XLabel = "X Axis",
            YLabel = "Y Axis",
            YMode = AxisOptions.AxisMode.normal,
            LogTicksWithFixedPoint = true,
            XAxisPosition = AxisOptions.AxisXLine.bottom,
            YAxisPosition = AxisOptions.AxisYLine.left,
            LegendOptions = new LegendOptions { LegendPosition = LegendOptions.LegendPositions.NorthWest },
            YMajorGrid = true
         };
         var plotItemNormal = new TeXReporting.Items.Plot(plotItem.Colors, axisOptionsNormal, plotItem.Plots, new Text("Test Figure with normal scaled Y-Axis"));
         _objectsToReport.Add(plotItemNormal);
         _figureCounter++;


         _objectsToReport.Add(new Section("Simple Figure 2"));
         _objectsToReport.Add(plotItem);
         _figureCounter++;

         _objectsToReport.Add(new Section("Simple Figure 3"));
         _objectsToReport.Add(plotItem2);
         _figureCounter++;

         _objectsToReport.Add(new Section("Two Ordinates Figure"));
         _objectsToReport.Add(plotTwoOrdinates);
         _figureCounter++;

         _objectsToReport.Add(new Section("Three Ordinates Figure"));
         _objectsToReport.Add(plotThreeOrdinates);
         _figureCounter++;
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