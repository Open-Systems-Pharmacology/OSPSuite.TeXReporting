using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace OSPSuite.TeXReporting.TeX.PGFPlots
{
   public class Plot
   {
      public Plot(IList<Coordinate> coordinates, PlotOptions options)
      {
         Coordinates = coordinates;
         Options = options;
      }

      public IList<Coordinate> Coordinates;
      public PlotOptions Options;
      public string LegendEntry;
      internal bool LogarithmicXAxis;

      internal bool LogarithmicYAxis;

      // some properties for error shading
      // opacity of area and limit lines
      // width of limit line
      const string LINE_WIDTH = "1pt";
      public const float MIN_VALUE = 1e-24F;

      private const string NAN = "nan";
      private const string INFINITY = "inf";
      private const string NEG_INFINITY = "-inf";

      public string GetStyleForLegend()
      {
         if (Options.ShadedErrorBars)
            return String.Format("smooth, {0}, draw=none, area legend, stack plots=y, fill={0}, opacity={1}",
               Options.Color, Options.Opacity);
         return Options.ToString();
      }

      public override string ToString()
      {
         var text = new StringBuilder();

         if (Options.IsLegendPlot)
         {
            plotLegendPlot(text);
         }
         else if (Options.BoxPlotPrepared != null)
         {
            plotBoxPlot(text);
         }
         else if (Options.ShadedErrorBars)
         {
            plotShadedErrorBars(text);
         }
         else if (Options.ErrorBars & Options.ErrorType == PlotOptions.ErrorTypes.geometric)
         {
            plotGeometricErrorBars(text);
         }
         else
         {
            plotCurve(text);
         }

         return text.ToString();
      }

      private void plotLegendPlot(StringBuilder text)
      {
         if (!Options.ShowInLegend)
            return;
         var options = Options.ToString();

         if (Options.BoxPlotPrepared != null)
         {
            var boxOptions = Options.Clone();
            boxOptions.BoxPlotPrepared = null;
            options = $"{boxOptions}, area legend";
         }
         else if (Options.ShadedErrorBars)
            options = $"{Options}, draw=none, area legend, fill={Options.Color}, opacity={Options.Opacity}";


         text.AppendFormat("\\addlegendimage{{{0}}};\n", options);
      }

      private void plotBoxPlot(StringBuilder text)
      {
         text.AppendFormat("\\addplot+[area legend, {0}] coordinates {{\n", Options);
         foreach (var point in Coordinates)
         {
            addValue(text, point.X, point.Y);
         }

         text.Append("};\n");
      }

      private void plotCurve(StringBuilder text)
      {
         text.AppendFormat("\\addplot [{0}] coordinates {{\n", Options);
         foreach (var point in Coordinates)
         {
            if (point.errX != null || point.errY != null)
               text.AppendFormat("({0}, {1}) +- ({2},{3})\n",
                  convert(point.X), convert(point.Y),
                  convert(point.errX), convert(point.errY));
            else
               addValue(text, point.X, point.Y);
         }

         text.Append("};\n");
      }

      private void plotGeometricErrorBars(StringBuilder text)
      {
         text.AppendFormat("\\addplot [{0},{1}] coordinates {{\n", Options,
            "forget plot, error bars/.cd, error bar style={solid},y dir=plus, y explicit");
         foreach (var point in Coordinates)
         {
            if (point.errX != null || point.errY != null)
               text.AppendFormat("({0}, {1}) +- ({2},{3})\n",
                  convert(point.X), convert(point.Y),
                  convert(point.errX), convert(point.Y * errorFor(point.errY) - point.Y));
            else
               addValue(text, point.X, point.Y);
         }

         text.Append("};\n");
         text.AppendFormat("\\addplot [{0},{1}] coordinates {{\n", Options,
            "error bars/.cd, error bar style={solid},y dir=minus, y explicit");
         foreach (var point in Coordinates)
         {
            if (point.errX != null || point.errY != null)
               text.AppendFormat("({0}, {1}) +- ({2},{3})\n",
                  convert(point.X), convert(point.Y),
                  convert(point.errX), convert(point.Y - point.Y / errorFor(point.errY)));
            else
               addValue(text, point.X, point.Y);
         }

         text.Append("};\n");
      }

      private void plotShadedErrorBarsSmoothUsingStacking(StringBuilder text)
      {
         var lowerCoordinates = getLowerCoordinates().ToList();
         var upperCoordinates = getUpperCoordinates().ToList();

         //draw lower limit line
         text.AppendFormat(
            "\\addplot [smooth, {0}, stack plots=y, line width={1}, opacity={2}, forget plot] \n", Options.Color,
            LINE_WIDTH, Options.Opacity);
         plotCoordinates(text, lowerCoordinates);
         text.Append(";\n");

         //draw upper limit line
         text.AppendFormat(
            "\\addplot [smooth, {0}, line width={1}, opacity={2}, forget plot] \n", Options.Color, LINE_WIDTH,
            Options.Opacity);
         plotCoordinates(text, upperCoordinates);
         text.Append(";\n");

         //stack upper area limit
         text.AppendFormat(
            "\\addplot [{0}, smooth, draw=none, area legend, stack plots=y, fill={0}, opacity={1}] \n",
            Options.Color, Options.Opacity);
         plotAreaCoordinates(text);

         //reset stack to zero by drawing upper limit again with stack dir=minus
         text.AppendFormat(
            "\\addplot [draw=none, stack plots=y, stack dir=minus, forget plot] \n");
         plotCoordinates(text, upperCoordinates);
         text.Append(";\n");
      }

      private void plotShadedErrorBars(StringBuilder text)
      {
         var lowerCoordinates = getLowerCoordinates().ToList();
         var upperCoordinates = getUpperCoordinates().ToList();

         //draw lower limit line
         text.AppendFormat(
            "\\addplot [{0}, line width={1}, opacity={2}, forget plot] \n", Options.Color,
            LINE_WIDTH, Options.Opacity);
         plotCoordinates(text, lowerCoordinates);
         text.Append(";\n");


         //draw upper limit line
         text.AppendFormat(
            "\\addplot [{0}, line width={1}, opacity={2}, forget plot] \n", Options.Color, LINE_WIDTH,
            Options.Opacity);
         plotCoordinates(text, upperCoordinates);
         text.Append(";\n");


         //stack upper area limit

         text.AppendFormat(
            "\\addplot [{0}, draw=none, area legend, fill={0}, opacity={1}{2}] \n",
            Options.Color, Options.Opacity, Options.ShowInLegend ? string.Empty : ", forget plot");
         var areaCoordinates = new List<Coordinate>(lowerCoordinates);
         areaCoordinates.AddRange(upperCoordinates.OrderByDescending(x => x.X));

         if (lowerCoordinates.Any())
            areaCoordinates.Add(lowerCoordinates.First());

         plotCoordinates(text, areaCoordinates);
         text.Append(" \\closedcycle;\n");
      }

      private void plotCoordinates(StringBuilder text, IEnumerable<Coordinate> coordinates)
      {
         text.Append("coordinates {");
         foreach (var point in coordinates)
            addValue(text, point.X, point.Y);
         text.Append("}");
      }

      private IEnumerable<Coordinate> getLowerCoordinates()
      {
         var coordinates = new List<Coordinate>();
         foreach (var point in Coordinates)
         {
            float yValue;

            var yError = errorFor(point.errY);
            switch (Options.ErrorType)
            {
               case PlotOptions.ErrorTypes.arithmetic:
                  yValue = point.Y - yError;
                  break;
               case PlotOptions.ErrorTypes.geometric:
                  yValue = point.Y / yError;
                  break;
               case PlotOptions.ErrorTypes.relative:
                  yValue = point.Y - yError * point.Y;
                  break;
               default:
                  throw new ArgumentOutOfRangeException();
            }

            coordinates.Add(new Coordinate(point.X, yValue));
         }

         return coordinates;
      }

      private void plotAreaCoordinates(StringBuilder text)
      {
         text.Append("coordinates {");
         plotCoordinates(text, getDifferenceCoordinates());
         text.Append(" \\closedcycle;\n");
      }

      private IEnumerable<Coordinate> getDifferenceCoordinates()
      {
         var coordinates = new List<Coordinate>();
         foreach (var point in Coordinates)
         {
            float yValue;
            if (point.errY2 == null)
            {
               var yError = errorFor(point.errY);
               switch (Options.ErrorType)
               {
                  case PlotOptions.ErrorTypes.arithmetic:
                     yValue = 2 * (yError < MIN_VALUE ? MIN_VALUE : yError);
                     break;
                  case PlotOptions.ErrorTypes.geometric:
                     yValue = point.Y * (yError - 1 / yError);
                     break;
                  case PlotOptions.ErrorTypes.relative:
                     yValue = 2 * point.Y * yError;
                     break;
                  default:
                     throw new ArgumentOutOfRangeException();
               }
            }
            else
            {
               yValue = (float) point.errY2 - point.errY ?? 0F;
            }

            coordinates.Add(new Coordinate(point.X, yValue));
         }

         return coordinates;
      }

      private IEnumerable<Coordinate> getUpperCoordinates()
      {
         var coordinates = new List<Coordinate>();
         foreach (var point in Coordinates)
         {
            float yValue;
            var yError = point.errY2 == null ? errorFor(point.errY) : errorFor(point.errY2);
            switch (Options.ErrorType)
            {
               case PlotOptions.ErrorTypes.arithmetic:
                  yValue = point.Y + yError;
                  break;
               case PlotOptions.ErrorTypes.geometric:
                  yValue = point.Y * yError;
                  break;
               case PlotOptions.ErrorTypes.relative:
                  yValue = point.Y + yError * point.Y;
                  break;
               default:
                  throw new ArgumentOutOfRangeException();
            }

            coordinates.Add(new Coordinate(point.X, yValue));
         }

         return coordinates;
      }

      private float errorFor(float? errorValue)
      {
         return errorValue ?? (Options.ErrorArithmetic ? 0F : 1F);
      }

      private void addValue(StringBuilder text, float xValue, float yValue)
      {
         if (LogarithmicXAxis && xValue <= 0F)
            xValue = float.NaN;
         if (LogarithmicYAxis && yValue <= 0F)
            yValue = float.NaN;

         text.AppendFormat("({0}, {1})\n", convert(xValue), convert(yValue));
      }

      private static string convert(float value)
      {
         if (float.IsNaN(value)) return NAN;
         if (float.IsInfinity(value)) return INFINITY;
         if (float.IsNegativeInfinity(value)) return NEG_INFINITY;
         return value.ToString(CultureInfo.InvariantCulture);
      }

      private static string convert(float? value)
      {
         return value.HasValue ? convert(value.Value) : NAN;
      }
   }
}