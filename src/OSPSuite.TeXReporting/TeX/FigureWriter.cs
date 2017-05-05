using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.TeXReporting.TeX.PGFPlots;
using Plot = OSPSuite.TeXReporting.TeX.PGFPlots.Plot;

namespace OSPSuite.TeXReporting.TeX
{
   
   public static class FigureWriter
   {
      /// <summary>
      /// These are supported positions for the float environment figure.
      /// </summary>
      public enum FigurePositions
      {
         /// <summary>
         /// This position means if possible the current position (here) should be used, if not then new page.
         /// </summary>
         ht,

         /// <summary>
         /// This position means try to put it on bottom of current page.
         /// </summary>
         b,

         /// <summary>
         /// This position means try to put it on top of current page.
         /// </summary>
         t,

         /// <summary>
         /// This position means to allow the float to take a whole page to itself.
         /// </summary>
         p,

         /// <summary>
         /// This position means if possible the current position (here) should be used, if not, then top, then bottom, then new page.
         /// </summary>
         htbp,

         /// <summary>
         /// This is the position for really hear. Comes with float package.
         /// </summary>
         H
      }

      /// <summary>
      /// This method creates TEX chunk for inclusion of images.
      /// </summary>
      /// <param name="options">To specify scale, height or width parameters.</param>
      /// <param name="fileName">Filename of image to include.</param>
      /// <param name="converter">TEX converter.</param>
      /// <returns>TEX chunk.</returns>
      private static string includeGraphics(string options, string fileName, ITeXConverter converter)
      {
         return $"\\includegraphics[{options}]{{{converter.FilePathToTeX(fileName)}}}\n";
      }

      internal static string InlineGraphic(string fileName, ITeXConverter converter)
      {
         return includeGraphics($"height={Helper.BaseLineSkip(1)}", fileName, converter);
      }

      /// <summary>
      /// This method appends the inclusion of a figure.
      /// </summary>
      /// <remarks>A figure environment will be placed somewhere according to position settings.
      /// It is an floating environment.</remarks>      
      /// <param name="position">>Where should the figure be placed.</param>
      /// <param name="caption">Text for figure caption.</param>
      /// <param name="label">Text for figure lable. Should start with fig:.</param>
      /// <param name="fileName">Relative path of graphic file to include.</param>
      /// <param name="landscape">Indicates whether the figure is in landscape environment or not.</param>
      /// <param name="converter">TEX converter.</param>
      internal static string IncludeFigure(FigurePositions position, string caption, string label, string fileName, bool landscape, ITeXConverter converter)
      {
         var tex = new StringBuilder();
         tex.Append(Helper.Begin(Helper.Environments.figure));
         tex.AppendFormat("[{0}]\n", position);
         tex.Append(Helper.NoIndent());
         tex.Append(Helper.Centering());

         var options = landscape
                          ? $"max height={Helper.GetLengthInPercentageOfTextWidth(90)}, max width={Helper.GetLengthInPercentageOfTextHeight(100)}"
            : $"max height={Helper.GetLengthInPercentageOfTextHeight(90)}, max width={Helper.GetLengthInPercentageOfTextWidth(100)}";

         options += ", keepaspectratio";
         tex.Append(includeGraphics(options, fileName, converter));

         tex.Append(Helper.Caption(caption));
         tex.Append(Helper.Label(label));
         tex.Append(Helper.End(Helper.Environments.figure));
         return tex.ToString();
      }

      private static string setSubfolderForPlots()
      {
         return $"\\tikzsetexternalprefix{{{Constants.ArtifactFolderForPlots}/}}\n";
      }

      /// <summary>
      /// This method creates a figure.
      /// </summary>
      /// <remarks>A figure environment will be placed somewhere according to position settings.
      /// It is an floating environment.</remarks>      
      /// <param name="caption">Text for figure caption.</param>
      /// <param name="groupPlot">GroupPlot settings needed for generation.</param>
      /// <param name="converter">TEX converter.</param>
      internal static string GroupPlotFigure(string caption, GroupPlot groupPlot, ITeXConverter converter)
      {
         var allPlots = new List<PGFPlots.Plot>();
         foreach (var groupedPlot in groupPlot.GroupedPlots)
            allPlots.AddRange(groupedPlot.Plots);

         if (groupPlot.GroupOptions.GroupLegendOptions != null)
         {
            var maxLegendEntry = Enumerable.Max<string>(groupPlot.GroupOptions.GroupLegendOptions.LegendEntries, l => l.Length);

            if (String.IsNullOrEmpty(groupPlot.GroupOptions.GroupLegendOptions.LegendOptions.TextWidth))
               groupPlot.GroupOptions.GroupLegendOptions.LegendOptions.TextWidth = getLegendWidth(groupPlot.GroupOptions.GroupLegendOptions.LegendOptions.LegendPosition,
                                                                                                  groupPlot.Landscape, maxLegendEntry);
         }

         adjustAxisDimensionForGroupPlots(groupPlot);

         var pictureText = new PlotWriter(groupPlot.Colors, converter).PictureGroupPlot(groupPlot.GroupedPlots,
                                                                                        groupPlot.AxisOptions,
                                                                                        groupPlot.GroupOptions);

         return plotFigure(caption, groupPlot, pictureText);
      }

      private static void adjustAxisDimensionForGroupPlots(GroupPlot groupPlot)
      {
         var axisOptions = groupPlot.AxisOptions;
         var landscape = groupPlot.Landscape;
         var columns = groupPlot.GroupOptions.Columns;
         var rows = groupPlot.GroupOptions.Rows;

         const float percentageHeigt = 70F; // reserve some space for groupings and captions
         const float percentageWidth = 100F;

         var isLegendBeside = false;
         if (groupPlot.GroupOptions.GroupLegendOptions.LegendOptions != null)
            isLegendBeside = getIsLegendBeside(groupPlot.GroupOptions.GroupLegendOptions.LegendOptions.LegendPosition);

         var factor = isLegendBeside ? 0.6 : 1;
         if (landscape)
         {
            axisOptions.Height = Helper.GetLengthInPercentageOfTextHeight(percentageWidth / rows);
            axisOptions.Width = Helper.GetLengthInPercentageOfTextWidth(percentageHeigt / columns);
         }
         else
         {
            axisOptions.Height = Helper.GetLengthInPercentageOfTextHeight(percentageHeigt / rows);
            axisOptions.Width = Helper.GetLengthInPercentageOfTextWidth(percentageWidth * factor / columns);
         }
      }

      private static void adjustAxisDimensionForFigures(AxisOptions axisOptions, bool landscape, bool isLegendBeside, int percentageLandscape, int percentageLegendBeside, int percentageNoLegendBeside = 100)
      {
         if (landscape)
         {
            axisOptions.Height = Helper.GetLengthInPercentageOfTextWidth(percentageLandscape);
         }
         else
         {
            axisOptions.Width = Helper.GetLengthInPercentageOfTextWidth(isLegendBeside ? percentageLegendBeside : percentageNoLegendBeside);
         }
      }

      /// <summary>
      /// This method creates a figure.
      /// </summary>
      /// <remarks>A figure environment will be placed somewhere according to position settings.
      /// It is an floating environment.</remarks>      
      /// <param name="caption">Text for figure caption.</param>
      /// <param name="plot">Plot settings needed for generation.</param>
      /// <param name="converter">TEX converter.</param>
      internal static string PlotFigure(string caption, Items.Plot plot, ITeXConverter converter)
      {
         var isLegendBeside = plot.AxisOptions.LegendOptions != null && getIsLegendBeside(plot.AxisOptions.LegendOptions.LegendPosition);
         adjustAxisDimensionForFigures(plot.AxisOptions, plot.Landscape, isLegendBeside, 90, 60);

         var maxLegendEntry = getMaxLegendEntryLength(plot.Plots);

         if (plot.AxisOptions.LegendOptions != null)
            if (String.IsNullOrEmpty(plot.AxisOptions.LegendOptions.TextWidth))
               plot.AxisOptions.LegendOptions.TextWidth = getLegendWidth(plot.AxisOptions.LegendOptions.LegendPosition,
                                                                         plot.Landscape, maxLegendEntry);

         var pictureText = new PlotWriter(plot.Colors, converter).Picture(plot.AxisOptions, plot.Plots);
         return plotFigure(caption, plot, pictureText);
      }

      private static string plotFigure(string caption, IPlot plot, string pictureText)
      {
         var tex = new StringBuilder();
         tex.Append(Helper.Begin(Helper.Environments.figure));
         tex.AppendFormat((string) "[{0}]\n", (object) plot.Position);
         tex.Append(Helper.NoIndent());
         tex.Append(Helper.Centering());
         tex.Append(setSubfolderForPlots());
         tex.Append(pictureText);
         tex.Append(Helper.Caption(caption));
         tex.Append(Helper.Label(plot.Label));
         tex.Append(Helper.End(Helper.Environments.figure));
         return tex.ToString();
      }

      /// <summary>
      /// This method creates a figure for a bar plot.
      /// </summary>
      /// <remarks>A figure environment will be placed somewhere according to position settings.
      /// It is an floating environment.</remarks>      
      /// <param name="caption">Text for figure caption.</param>
      /// <param name="plot">Plot settings needed for generation.</param>
      /// <param name="converter">TEX converter.</param>
      internal static string BarPlotFigure(string caption, BarPlot plot, ITeXConverter converter)
      {
         var isLegendBeside = plot.AxisOptions.LegendOptions != null && getIsLegendBeside(plot.AxisOptions.LegendOptions.LegendPosition);
         adjustAxisDimensionForFigures(plot.AxisOptions, plot.Landscape, isLegendBeside, 90, 60);

         var maxLegendEntry = getMaxLegendEntryLength(plot.Plots);

         if (plot.AxisOptions.LegendOptions != null)
            if (String.IsNullOrEmpty(plot.AxisOptions.LegendOptions.TextWidth))
               plot.AxisOptions.LegendOptions.TextWidth = getLegendWidth(plot.AxisOptions.LegendOptions.LegendPosition,
                                                                      plot.Landscape, maxLegendEntry);

         var pictureText = new PlotWriter(plot.Colors, converter).Picture(plot.BarPlotOptions, plot.AxisOptions, plot.Plots);
         return plotFigure(caption, plot, pictureText);
      }


      private static bool getIsLegendBeside(LegendOptions.LegendPositions legendPosition)
      {
         var isLegendBeside = legendPosition ==
                              LegendOptions.LegendPositions.OuterNorthEast ||
                              legendPosition ==
                              LegendOptions.LegendPositions.OuterNorthWest;
         return isLegendBeside;
      }

      private static int getMaxLegendEntryLength(IEnumerable<PGFPlots.Plot> plots)
      {
         var maxLegendEntry = 0;
         var enumerablePlots = plots as IList<PGFPlots.Plot> ?? plots.ToList();
         if (enumerablePlots.Any(p => !String.IsNullOrEmpty(p.LegendEntry)))
         {
            maxLegendEntry = enumerablePlots.Where(p => !String.IsNullOrEmpty(p.LegendEntry)).Max(p => p.LegendEntry.Length);
         }
         return maxLegendEntry;
      }

      /// <summary>
      /// This method creates a figure.
      /// </summary>
      /// <remarks>A figure environment will be placed somewhere according to position settings.
      /// It is an floating environment.</remarks>      
      /// <param name="caption">Text for figure caption.</param>
      /// <param name="plot">Plot settings needed for generation.</param>
      /// <param name="converter">TEX converter.</param>
      internal static string PlotTwoOrdinatesFigure(string caption, PlotTwoOrdinates plot, ITeXConverter converter)
      {
         var isLegendBeside = plot.AxisOptionsY2.LegendOptions != null && getIsLegendBeside(plot.AxisOptionsY2.LegendOptions.LegendPosition);
         var maxLegendEntry = getMaxLegendEntryLength(plot.Plots.Concat<Plot>(plot.PlotsY2));

         adjustAxisDimensionForFigures(plot.AxisOptions, plot.Landscape, isLegendBeside, 90, 50, 80);
         adjustAxisDimensionForFigures(plot.AxisOptionsY2, plot.Landscape, isLegendBeside, 90, 50, 80);

         if (plot.AxisOptionsY2.LegendOptions != null)
            if (String.IsNullOrEmpty(plot.AxisOptionsY2.LegendOptions.TextWidth))
               plot.AxisOptionsY2.LegendOptions.TextWidth = getLegendWidth(
                  plot.AxisOptionsY2.LegendOptions.LegendPosition, plot.Landscape, maxLegendEntry);

         var pictureText = new PlotWriter(plot.Colors, converter).PictureTwoOrdinates(plot.AxisOptions,
                                                                               plot.AxisOptionsY2,
                                                                               plot.Plots,
                                                                               plot.PlotsY2);

         return plotFigure(caption, plot, pictureText);
      }

      /// <summary>
      /// This method creates a figure.
      /// </summary>
      /// <remarks>A figure environment will be placed somewhere according to position settings.
      /// It is an floating environment.</remarks>      
      /// <param name="caption">Text for figure caption.</param>
      /// <param name="plot">Plot settings needed for generation.</param>
      /// <param name="converter">TEX converter.</param>
      internal static string PlotThreeOrdinatesFigure(string caption, PlotThreeOrdinates plot, ITeXConverter converter)
      {
         var isLegendBeside = getIsLegendBeside(plot.AxisOptionsY3.LegendOptions.LegendPosition);
         var maxLegendEntry = getMaxLegendEntryLength(plot.Plots.Concat<Plot>(plot.PlotsY2).Concat<Plot>(plot.PlotsY3));

         adjustAxisDimensionForFigures(plot.AxisOptions, plot.Landscape, isLegendBeside, 90, 40, 70);
         adjustAxisDimensionForFigures(plot.AxisOptionsY2, plot.Landscape, isLegendBeside, 90, 40, 70);
         adjustAxisDimensionForFigures(plot.AxisOptionsY3, plot.Landscape, isLegendBeside, 90, 40, 70);

         if (String.IsNullOrEmpty(plot.AxisOptionsY3.LegendOptions.TextWidth))
            plot.AxisOptionsY3.LegendOptions.TextWidth = getLegendWidth(
               plot.AxisOptionsY3.LegendOptions.LegendPosition, plot.Landscape, maxLegendEntry);

         var pictureText = new PlotWriter(plot.Colors, converter).PictureThreeOrdinates(plot.AxisOptions,
                                                                                 plot.AxisOptionsY2,
                                                                                 plot.AxisOptionsY3,
                                                                                 plot.Plots,
                                                                                 plot.PlotsY2,
                                                                                 plot.PlotsY3);
         return plotFigure(caption, plot, pictureText);
      }

      /// <summary>
      /// This method arranges the width of legends by specifying max text width.
      /// </summary>
      /// <remarks>
      /// The legend entries get then word wrapped automatically.
      /// </remarks>
      /// <param name="position"></param>
      /// <param name="isLandscape"></param>
      /// <param name="maxLegendEntry"></param>
      /// <returns>Good adjusted width string.</returns>
      private static string getLegendWidth(LegendOptions.LegendPositions position, bool isLandscape, int maxLegendEntry)
      {
         var isInnerPosition = position == LegendOptions.LegendPositions.NorthWest ||
                               position == LegendOptions.LegendPositions.NorthEast ||
                               position == LegendOptions.LegendPositions.SouthEast ||
                               position == LegendOptions.LegendPositions.SouthWest;
         var isLegendBeside = getIsLegendBeside(position);
         var isLegendWide = position == LegendOptions.LegendPositions.OuterNorth ||
                            position == LegendOptions.LegendPositions.OuterSouth ||
                            position == LegendOptions.LegendPositions.OuterSouthEast ||
                            position == LegendOptions.LegendPositions.OuterSouthWest;

         if (isInnerPosition && maxLegendEntry > 40)
            return Helper.GetLengthInPercentageOfTextWidth(40);

         if (isLegendBeside && !isLandscape && maxLegendEntry > 25)
            return Helper.GetLengthInPercentageOfTextWidth(25);

         if (isLegendWide && maxLegendEntry > 100)
            return Helper.GetLengthInPercentageOfTextWidth(90);

         if (maxLegendEntry > 40 && !isLegendWide)
            return Helper.GetLengthInPercentageOfTextWidth(40);

         return String.Empty;
      }
   }
}