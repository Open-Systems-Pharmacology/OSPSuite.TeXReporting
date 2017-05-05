using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.TeX.PGFPlots
{
   public class PlotWriter
   {
      private readonly IEnumerable<Color> _colors;
      private readonly ITeXConverter _converter;
      private const float SWITCH_FROM_LOG_TO_NORMAL_LIMIT = 1.2F;

      public PlotWriter(IEnumerable<Color> colors, ITeXConverter converter)
      {
         _colors = colors;
         _converter = converter;
      }

      private string addLegendEntryWithLabelRef(Plot plot)
      {
         if (String.IsNullOrEmpty(plot.LegendEntry))
            return String.Empty;
         
         var tex = new StringBuilder();
         //TeX.AppendFormat("\\addlegendimage{{/pgfplots/refstyle={{{0}}}}};", plot.Options.Label);
         tex.AppendFormat("\\addlegendimage{{{0}}};", plot.Options.Label);
         tex.Append(addLegendEntry(plot));
         
         return tex.ToString();
      }
      private string addLegendEntry(Plot plot)
      {
         if (String.IsNullOrEmpty(plot.LegendEntry)) return String.Empty;
         return $"\\addlegendentry{{{_converter.StringToTeX(plot.LegendEntry)}}};\n";
      }
      private string defineStyleForLegendRef(string label, string style)
      {
         return $"\\pgfplotsset{{{label}/.style={{{style}}}}}\n";
      }

      private enum Axis
      {
         X,
         Y
      }

      private float getMax(Axis axis, IEnumerable<Plot> plots)
      {
         float max = float.NegativeInfinity;

         if (axis == Axis.Y && plots.All(x => x.Options.BoxPlotPrepared != null))
         {
            var maxBox = plots.Max(x => x.Options.BoxPlotPrepared.UpperWhisker);
            max = maxBox > max ? maxBox : max;
         }

         foreach (var plot in plots)
         {
            if (plot.Coordinates.Any(c => (axis == Axis.X ? c.X : c.Y) > 0))
            {
               var maxCoordinate = plot.Coordinates.Where(c => (axis == Axis.X ? c.X : c.Y) > 0).Max(c => axis == Axis.X ? c.X : c.Y);
               max = maxCoordinate > max ? maxCoordinate : max;               
            }
         }

         return max;
      }
      private float getMin(Axis axis, IEnumerable<Plot> plots)
      {
         float min = float.PositiveInfinity;

         if (axis == Axis.Y && plots.All(x => x.Options.BoxPlotPrepared != null))
         {
            var minBox = plots.Min(x => x.Options.BoxPlotPrepared.LowerWhisker);
            min = minBox < min ? minBox : min;
         }

         foreach (var plot in plots)
         {
            if (plot.Coordinates.Any(c => (axis == Axis.X ? c.X : c.Y) > 0))
            {
               var minCoordinate = plot.Coordinates.Where(c => (axis == Axis.X ? c.X : c.Y) > 0).Min(c => axis == Axis.X ? c.X : c.Y);
               min = minCoordinate < min ? minCoordinate : min;
            }
         }
         return min;
      }

      private float[] getTicksForPlots(Axis axis, IEnumerable<Plot> plots, out bool switchFromLogToNormal, out bool useFixedPoints)
      {
         plots = plots.ToArray();
         switchFromLogToNormal = false;
         var max = getMax(axis, plots);
         var min = getMin(axis, plots);
         var maxDim = (int)Math.Log10(max);
         var minDim = (int)Math.Log10(min);

         useFixedPoints = checkFixedPointsUsage(maxDim, minDim);

         if (maxDim - minDim == 0)
            if (max / min < SWITCH_FROM_LOG_TO_NORMAL_LIMIT)
               switchFromLogToNormal = true;
            else
            {
               var diffDim = (int)Math.Log10(max - min);
               var baseValue = (int)(min / Math.Pow(10, diffDim));
               var baseTick = calcTick(baseValue, diffDim);

               var ticks = new float[7];
               ticks[0] = baseTick.Equals(0) ? calcTick(1, diffDim - 1) : baseTick;
               ticks[1] = baseTick + calcTick(2, diffDim - 1);
               ticks[2] = baseTick + calcTick(5, diffDim - 1);
               ticks[3] = baseTick + calcTick(1, diffDim);
               ticks[4] = baseTick + calcTick(2, diffDim);
               ticks[5] = baseTick + calcTick(5, diffDim);
               ticks[6] = baseTick + calcTick(10, diffDim);

               return ticks;
            }

         if (maxDim - minDim == 1)
         {
            var ticks = new float[6];
            ticks[0] = calcTick(1, minDim);
            for (var i = 1; i <= 2; i++)
               ticks[i] = calcTick(5 * i, minDim); 
            ticks[3] = calcTick(2, maxDim); 
            ticks[4] = calcTick(5, maxDim);
            ticks[5] = calcTick(10, maxDim);
            return ticks;
         }

         return null;
      }

      private float calcTick(int labelValue, float exponent)
      {
         return Convert.ToSingle(labelValue * Math.Pow(10, exponent));
      }

      /// <summary>
      /// If the range of dimension used is too large the number gets very big. This could cause the whole plot to be shifted too much.
      /// Therefor we use scientific number format instead (10^2, 10^3 etc) to avoid the large numbers.
      /// </summary>
      /// <param name="maxDim"></param>
      /// <param name="minDim"></param>
      /// <returns></returns>
      private static bool checkFixedPointsUsage(int maxDim, int minDim)
      {
         return !((maxDim - minDim) > 5 || maxDim > 5 || minDim < -5);
      }

      private float[] getMinorTicksForPlots(Axis axis, IEnumerable<Plot> plots)
      {
         plots = plots.ToArray();
         var max = getMax(axis, plots);
         var min = getMin(axis, plots);
         var maxDim = (int)Math.Log10(max);
         var minDim = (int)Math.Log10(min);

         if (maxDim - minDim == 0)
            if (max / min < SWITCH_FROM_LOG_TO_NORMAL_LIMIT)
               return null;
            else
            {
               var diffDim = (int) Math.Log10(max - min);
               var baseValue = (int)(min/Math.Pow(10, diffDim));
               var baseTick = calcTick(baseValue, diffDim);

               var ticks = new List<float>();
               for (var i = 1; i <10; i++)
                  ticks.Add(baseTick + calcTick(i, diffDim - 2));
               for (var i = 1; i < 10; i++)
               {
                  if (i == 2 || i == 5) continue;
                  ticks.Add(baseTick + calcTick(i, diffDim - 1));
               }
               for (var i = 3; i < 10; i++)
               {
                  if (i == 5) continue;
                  ticks.Add(baseTick + calcTick(i, diffDim));
               }
               return ticks.ToArray();
            }
         if (maxDim - minDim == 1)
         {
            var ticks = new float[15];
            for (var i = 2; i <= 4; i++) ticks[i - 2] = calcTick(i, minDim);
            for (var i = 6; i <= 9; i++) ticks[i - 3] = calcTick(i, minDim);
            ticks[7] = calcTick(15, minDim);
            for (var i = 3; i <= 9; i++) ticks[5 + i] = calcTick(i, maxDim);
            return ticks;
         }

         return null;
      }

      /// <summary>
      /// This function creates automatically ticks for logarithmic axis.
      /// </summary>
      /// <remarks>
      /// In pgfplots there is a problem when the values are to close together on a logarithmic axis.
      /// The ticks are then created by pgfplots on fixed exponent position 10^0.9, 10^0.8, ....
      /// But this not desired. It should be 9, 8, ....
      /// Therefor if the difference of the scale is less than 2 then we define the ticks with this function.
      /// </remarks>
      /// <param name="axisOptions">Axis options to fix.</param>
      /// <param name="plots">Plots for this axis options.</param>
      /// <returns>Fixed axis options</returns>
      private AxisOptions getTicksForLogPlots(AxisOptions axisOptions, IEnumerable<Plot> plots)
      {
         plots = plots.ToArray();

         if (axisOptions.XMode != AxisOptions.AxisMode.log && axisOptions.YMode != AxisOptions.AxisMode.log)
            return axisOptions;
         if (axisOptions.XTicks == null && axisOptions.XMode == AxisOptions.AxisMode.log)
         {
            bool switchFromLogToNormal;
            bool useFixedPoints;
            axisOptions.XTicks = getTicksForPlots(Axis.X, plots, out switchFromLogToNormal, out useFixedPoints);
            if (switchFromLogToNormal)
               axisOptions.XMode = AxisOptions.AxisMode.normal;
            axisOptions.LogTicksWithFixedPoint &= useFixedPoints;

            if (axisOptions.XMinorTicks == null)
               axisOptions.XMinorTicks = getMinorTicksForPlots(Axis.X, plots);

            //if (axisOptions.XMin == null)
            //   axisOptions.XMin = GetMin(Axis.X, plots);
            //if (axisOptions.XMax == null)
            //   axisOptions.XMax = GetMax(Axis.X, plots);

            //if (axisOptions.XMin != null && axisOptions.XMin < Plot.MIN_VALUE) axisOptions.XMin = Plot.MIN_VALUE;
            //if (axisOptions.XMax != null && axisOptions.XMax < Plot.MIN_VALUE) axisOptions.XMax = Plot.MIN_VALUE;
         }
         if (axisOptions.YTicks == null && axisOptions.YMode == AxisOptions.AxisMode.log)
         {
            bool switchFromLogToNormal;
            bool useFixedPoints;
            axisOptions.YTicks = getTicksForPlots(Axis.Y, plots, out switchFromLogToNormal, out useFixedPoints);
            if (switchFromLogToNormal)
               axisOptions.YMode = AxisOptions.AxisMode.normal;
            axisOptions.LogTicksWithFixedPoint &= useFixedPoints;

            if (axisOptions.YMinorTicks == null)
               axisOptions.YMinorTicks = getMinorTicksForPlots(Axis.Y, plots);

            //if (axisOptions.YMin == null)
            //   axisOptions.YMin = GetMin(Axis.Y, plots);
            //if (axisOptions.YMax == null)
            //   axisOptions.YMax = GetMax(Axis.Y, plots);

            //if (axisOptions.YMin != null && axisOptions.YMin < Plot.MIN_VALUE) axisOptions.YMin = Plot.MIN_VALUE;
            //if (axisOptions.YMax != null && axisOptions.YMax < Plot.MIN_VALUE) axisOptions.YMax = Plot.MIN_VALUE;

         }

         return axisOptions;
      }

      /// <summary>
      /// This creates a picture for a group of plots.
      /// </summary>
      /// <param name="groupedPlots">List of plots to be grouped.</param>
      /// <param name="axisOptions">Used for the whole group of plots.</param>
      /// <param name="groupOptions">For defining the structure of the matrix and where labels and ticks are placed.</param>
      /// <param name="options">For sizing the picture.</param>
      /// <returns></returns>
      public string PictureGroupPlot(IEnumerable<IBasePlot> groupedPlots, AxisOptions axisOptions, GroupOptions groupOptions, string options = null)
      {
         var tex = new StringBuilder();

         var groupPlots = groupedPlots as IList<IBasePlot> ?? groupedPlots.ToList();
         foreach (var groupPlot in groupPlots)
         {
            groupPlot.AxisOptions = getTicksForLogPlots(groupPlot.AxisOptions, groupPlot.Plots);
         }

         tex.Append(Helper.Begin(Helper.Environments.tikzpicture));
         tex.Append(Helper.LineFeed());

         if (!String.IsNullOrEmpty(options))
            tex.AppendFormat("\\pgfplotsset{{{0}}}\n", options);

         tex.Append(Helper.LineFeed());
         foreach (var color in _colors)
            tex.Append(defineColor(color));

         tex.Append(Helper.Begin(Helper.Environments.groupplot, $"{groupOptions},{axisOptions}"));
         tex.Append(Helper.LineFeed());
         foreach (var groupPlot in groupPlots)
         {
            tex.AppendFormat((string) "\\nextgroupplot[{0}]\n", (object) groupPlot.AxisOptions);
            addPlots(tex, groupPlot.Plots, groupPlot.AxisOptions);
         }
         tex.Append(Helper.End(Helper.Environments.groupplot));

         if (!string.IsNullOrEmpty(groupOptions.Title))
            tex.Append(groupOptions.PlaceGroupTitle(_converter.StringToTeX(groupOptions.Title)));
         
         tex.Append(groupOptions.PlaceLegend(getSpaceInRowsToReserveForGroupLines(axisOptions, groupedPlots)));

         tex.Append(Helper.End(Helper.Environments.tikzpicture));
         return tex.ToString();
      }

      private int getOffset(AxisOptions axisOptions)
      {
         var titleOffset = (string.IsNullOrEmpty(axisOptions.XLabel) ? 0 : 2);
         return ((axisOptions.XTickLabelsRotateBy % 360 != 0) ? 8 : 1) + titleOffset;
      }

      private int getSpaceInRowsToReserveForGroupLines(AxisOptions axisOptions, IEnumerable<IBasePlot> groupedPlots)
      {
         var offset = getOffset(axisOptions);
         var levels = getLevels(axisOptions);
         foreach (var plot in groupedPlots)
         {
            var plotOffset = getOffset(plot.AxisOptions);
            var plotLevels = getLevels(plot.AxisOptions);
            offset = max(plotOffset, offset);
            levels = max(plotLevels, levels);
         }
         return levels * 2 + offset;
      }

      private int max(int v1, int v2)
      {
         return (v1 > v2 ? v1 : v2);
      }

      private static int getLevels(AxisOptions axisOptions)
      {
         int levels = 0;
         if (axisOptions.GroupLines != null && axisOptions.GroupLines.Any())
            levels = axisOptions.GroupLines.Max(g => g.Level);
         return levels;
      }

      private void addPlots(StringBuilder tex, IEnumerable<Plot> plots, AxisOptions axisOptions, bool markPlots = false)
      {
         foreach (var plot in plots)
         {
            if (axisOptions.XMode == AxisOptions.AxisMode.log) plot.LogarithmicXAxis = true;
            if (axisOptions.YMode == AxisOptions.AxisMode.log) plot.LogarithmicYAxis = true;
            if (markPlots)
               plot.Options.Label = Helper.Marker();
            tex.Append(plot);
            if (!markPlots)
               tex.Append(addLegendEntry(plot));
         }
      }

      /// <summary>
      /// This creates a picture for a plot.
      /// </summary>
      /// <param name="axisOptions"></param>
      /// <param name="plots"></param>
      /// <param name="options">For sizing the picture.</param>
      /// <returns></returns>
      public string Picture(AxisOptions axisOptions, IEnumerable<Plot> plots, string options = null)
      {
         plots = plots.ToArray();
         axisOptions = getTicksForLogPlots(axisOptions, plots);
         return picture(axisOptions, plots, axisOptions.ToString(), options);
      }

      private string picture(AxisOptions axisOptions, IEnumerable<Plot> plots, string axisOptionsForAxis, string options = null)
      {
         plots = plots.ToArray();
         var tex = new StringBuilder();

         tex.Append(Helper.Begin(Helper.Environments.tikzpicture));
         tex.Append(Helper.LineFeed());

         if (!String.IsNullOrEmpty(options))
            tex.AppendFormat("\\pgfplotsset{{{0}}}\n", options);

         tex.Append(Helper.LineFeed());
         foreach (var color in _colors)
            tex.Append(defineColor(color));

         tex.Append(Helper.Begin(Helper.Environments.axis, axisOptionsForAxis));
         tex.Append(Helper.LineFeed());
         addPlots(tex, plots, axisOptions);
         tex.Append(Helper.End(Helper.Environments.axis));

         tex.Append(Helper.End(Helper.Environments.tikzpicture));
         return tex.ToString();
      }

      /// <summary>
      /// This creates a picture for a plot.
      /// </summary>
      /// <param name="barPlotOptions"></param>
      /// <param name="axisOptions"></param>
      /// <param name="plots"></param>
      /// <param name="options">For sizing the picture.</param>
      /// <returns></returns>
      public string Picture(BarPlotOptions barPlotOptions, AxisOptions axisOptions, IEnumerable<Plot> plots, string options = null)
      {
         plots = plots.ToArray();
         axisOptions = getTicksForLogPlots(axisOptions, plots);
         return picture(axisOptions, plots, $"{barPlotOptions},{axisOptions}", options);
      }

      /// <summary>
      /// This creates a picture for a plot with two ordinates (Y, Y2).
      /// </summary>
      /// <remarks>The legend options are used from axisOptionsY2. The x axis is taken from axisOptionsY.</remarks>
      /// <param name="axisOptionsY"></param>
      /// <param name="axisOptionsY2"></param>
      /// <param name="plotsY"></param>
      /// <param name="plotsY2"></param>
      /// <param name="options"></param>
      /// <returns></returns>
      public string PictureTwoOrdinates(AxisOptions axisOptionsY, AxisOptions axisOptionsY2, IEnumerable<Plot> plotsY, IEnumerable<Plot> plotsY2 , string options = null)
      {
         plotsY = plotsY.ToArray();
         plotsY2 = plotsY2.ToArray();

         axisOptionsY = getTicksForLogPlots(axisOptionsY, plotsY);
         axisOptionsY2 = getTicksForLogPlots(axisOptionsY2, plotsY2);

         // The x axis must be the same for all Y axis.
         if (axisOptionsY.XMin == null) axisOptionsY.XMin = axisOptionsY.XTickMin;
         if (axisOptionsY.XMax == null) axisOptionsY.XMax = axisOptionsY.XTickMax;
         axisOptionsY2.XAxisIsHidden = true;
         axisOptionsY2.XMin = axisOptionsY.XMin;
         axisOptionsY2.XMax = axisOptionsY.XMax;
         axisOptionsY2.EnlargeLimits = axisOptionsY.EnlargeLimits;
         // Arrange axis positions for two ordinates plot
         axisOptionsY.YAxisPosition = AxisOptions.AxisYLine.left;
         axisOptionsY2.YAxisPosition = AxisOptions.AxisYLine.right;
         // Arrows make problems in multiple ordinates szenario, therefor they are set to false
         axisOptionsY.YAxisArrow = false;
         axisOptionsY2.YAxisArrow = false;
         // Reset global settings for multiple ordinates.
         axisOptionsY2.BackgroundColor = String.Empty;
         axisOptionsY2.Title = String.Empty;

         var tex = new StringBuilder();
         tex.Append(Helper.Begin(Helper.Environments.tikzpicture));
         tex.Append(Helper.LineFeed());

         if (!String.IsNullOrEmpty(options))   options += ",";
         options += "set layers";
         if (!String.IsNullOrEmpty(options)) options += ",";
         options += "cell picture=true";
         tex.AppendFormat("\\pgfplotsset{{{0}}}\n", options);

         tex.Append(Helper.LineFeed());
         foreach (var color in _colors)
            tex.Append(defineColor(color));

         tex.Append(Helper.Begin(Helper.Environments.axis, axisOptionsY + ",scale only axis"));
         tex.Append(Helper.LineFeed());
         //add curves of Y axis
         addPlots(tex, plotsY, axisOptionsY, true);
         tex.Append(Helper.End(Helper.Environments.axis));

         //define styles for legend ref
         foreach (var plot in plotsY)
            tex.Append(defineStyleForLegendRef(plot.Options.Label, plot.GetStyleForLegend()));

         // Y2 axis

         tex.Append(Helper.Begin(Helper.Environments.axis, axisOptionsY2 + ",scale only axis"));
         tex.Append(Helper.LineFeed());
         //create legend entries for first ordinate
         foreach (var plot in plotsY)
            tex.Append(addLegendEntryWithLabelRef(plot));

         //add curves of Y2 axis
         addPlots(tex, plotsY2, axisOptionsY2);
         tex.Append(Helper.End(Helper.Environments.axis));

         tex.Append(Helper.End(Helper.Environments.tikzpicture));
         return tex.ToString();
      }

      /// <summary>
      /// This creates a picture for a plot with three ordinates (Y, Y2, Y3).
      /// </summary>
      /// <remarks>The legend options are used from axisOptionsY3. The x axis is taken from axisOptionsY.</remarks>
      /// <param name="axisOptionsY"></param>
      /// <param name="axisOptionsY2"></param>
      /// <param name="axisOptionsY3"></param>
      /// <param name="plotsY"></param>
      /// <param name="plotsY2"></param>
      /// <param name="plotsY3"></param>
      /// <param name="options"></param>
      /// <returns></returns>
      public string PictureThreeOrdinates(AxisOptions axisOptionsY, AxisOptions axisOptionsY2, AxisOptions axisOptionsY3, IEnumerable<Plot> plotsY, IEnumerable<Plot> plotsY2, IEnumerable<Plot> plotsY3, string options = null)
      {
         plotsY = plotsY.ToArray();
         plotsY2 = plotsY2.ToArray();
         plotsY3 = plotsY3.ToArray();

         axisOptionsY = getTicksForLogPlots(axisOptionsY, plotsY);
         axisOptionsY2 = getTicksForLogPlots(axisOptionsY2, plotsY2);
         axisOptionsY3 = getTicksForLogPlots(axisOptionsY3, plotsY3);

         // The x axis must be the same for all Y axis.
         if (axisOptionsY.XMin == null) axisOptionsY.XMin = axisOptionsY.XTickMin;
         if (axisOptionsY.XMax == null) axisOptionsY.XMax = axisOptionsY.XTickMax;
         axisOptionsY2.XAxisIsHidden = true;
         axisOptionsY2.XMin = axisOptionsY.XMin;
         axisOptionsY2.XMax = axisOptionsY.XMax;
         axisOptionsY2.EnlargeLimits = axisOptionsY.EnlargeLimits;
         axisOptionsY3.XAxisIsHidden = true;
         axisOptionsY3.XMin = axisOptionsY.XMin;
         axisOptionsY3.XMax = axisOptionsY.XMax;
         axisOptionsY3.EnlargeLimits = axisOptionsY.EnlargeLimits;
         // Arrange axis positions for two ordinates plot
         axisOptionsY.YAxisPosition = AxisOptions.AxisYLine.left;
         axisOptionsY2.YAxisPosition = AxisOptions.AxisYLine.right;
         axisOptionsY3.YAxisPosition = AxisOptions.AxisYLine.right;
         // Arrows make problems in multiple ordinates szenario, therefor they are set to false
         axisOptionsY.YAxisArrow = false;
         axisOptionsY2.YAxisArrow = false;
         axisOptionsY3.YAxisArrow = false;
         // Reset global settings for multiple ordinates.
         axisOptionsY2.BackgroundColor = String.Empty;
         axisOptionsY3.BackgroundColor = String.Empty;
         axisOptionsY2.Title = String.Empty;
         axisOptionsY3.Title = String.Empty;

         var tex = new StringBuilder();
         tex.Append(Helper.Begin(Helper.Environments.tikzpicture));
         tex.Append(Helper.LineFeed());

         if (!String.IsNullOrEmpty(options)) options += ",";
         options += "set layers";
         if (!String.IsNullOrEmpty(options)) options += ",";
         options += "cell picture=true";
         tex.AppendFormat("\\pgfplotsset{{{0}}}\n", options);

         tex.Append(Helper.LineFeed());
         foreach (var color in _colors)
            tex.Append(defineColor(color));

         tex.Append(Helper.Begin(Helper.Environments.axis, axisOptionsY + ",scale only axis"));
         tex.Append(Helper.LineFeed());
         // add curves of Y axis
         addPlots(tex, plotsY, axisOptionsY, true);
         tex.Append(Helper.End(Helper.Environments.axis));

         //define styles for legend ref
         foreach (var plot in plotsY)
            tex.Append(defineStyleForLegendRef(plot.Options.Label, plot.GetStyleForLegend()));

         // Y2 Axis
         tex.Append(Helper.Begin(Helper.Environments.axis, axisOptionsY2 + ",scale only axis"));
         tex.Append(Helper.LineFeed());
         // add curves of Y2 axis
         addPlots(tex, plotsY2, axisOptionsY2, true);
         tex.Append(Helper.End(Helper.Environments.axis));

         //define styles for legend ref
         foreach (var plot in plotsY2)
            tex.Append(defineStyleForLegendRef(plot.Options.Label, plot.GetStyleForLegend()));

         // Y3 Axis
         tex.Append(Helper.Begin(Helper.Environments.axis, axisOptionsY3 + ",scale only axis"));
         tex.Append(Helper.LineFeed());
         if (axisOptionsY3.LegendOptions != null)
            if (axisOptionsY3.LegendOptions.LegendPosition == LegendOptions.LegendPositions.OuterNorthEast)
               tex.Append("\\pgfplotsset{legend style={xshift=2 cm}}\n");
         tex.Append("\\pgfplotsset{every outer y axis line/.style={xshift=2 cm}, every tick/.style={xshift=2cm}, every y tick label/.style={xshift=2cm}}\n");

         //create legend entries for first and second ordinate
         foreach (var plot in plotsY)
            tex.Append(addLegendEntryWithLabelRef(plot));
         foreach (var plot in plotsY2)
            tex.Append(addLegendEntryWithLabelRef(plot));

         // add curves of axis Y3
         addPlots(tex, plotsY3, axisOptionsY3);
         tex.Append(Helper.End(Helper.Environments.axis));

         tex.Append(Helper.End(Helper.Environments.tikzpicture));
         return tex.ToString();
      }

      private static string defineColor(Color color)
      {
         return $"\\definecolor{{{color.Name}}}{{RGB}}{{{color.R},{color.G},{color.B}}}\n";
      }
   }
}
