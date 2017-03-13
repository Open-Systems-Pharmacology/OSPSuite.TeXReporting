using System.Text;

namespace OSPSuite.TeXReporting.TeX.PGFPlots
{
   public class BarPlotOptions
   {
      /// <summary>
      /// Supported directions for bar plots.
      /// </summary>
      public enum BarDirections
      {
         x,
         y
      }

      /// <summary>
      /// Specifies the coordinate value to use for bars and the direction of bars.
      /// </summary>
      public BarDirections BarDirection = BarDirections.y;

      /// <summary>
      /// Supported plot types for bar plots.
      /// </summary>
      public enum BarPlotTypes
      {
         /// <summary>
         /// Bars are ploted side by side.
         /// </summary>
         SideBySide,
         /// <summary>
         /// Bars are stacked on each other.
         /// </summary>
         Stacked,
         /// <summary>
         /// The width of bars is calculated by the size of the interval.
         /// </summary>
         /// <remarks>For getting the last interval, you have to add an additional coordinate. For the last coordinate no bar will be plotted.</remarks>
         Interval,
         /// <summary>
         /// The width of bars is calculated by the size of the interval. Bars are stacked on each other.
         /// </summary>
         /// <remarks>For getting the last interval, you have to add an additional coordinate. For the last coordinate no bar will be plotted.</remarks>
         IntervalStacked
      }

      private string getTextFor(BarPlotTypes plotType)
      {
         switch (plotType)
         {
            case BarPlotTypes.SideBySide:
               return string.Empty;
            case BarPlotTypes.Stacked:
               return " stacked";
            case BarPlotTypes.Interval:
               return " interval";
            case BarPlotTypes.IntervalStacked:
               return " interval stacked";
         }
         return string.Empty;
      }

      /// <summary>
      /// Specifies the bar plot type.
      /// </summary>
      public BarPlotTypes BarPlotType = BarPlotTypes.SideBySide;

      /// <summary>
      /// Specifies whether the bar value is displayed on top of the bar.
      /// </summary>
      public bool NodesNearCoords = false;

      /// <summary>
      /// Specifies the width of the bars.
      /// </summary>
      /// <remarks>Can be a length or if unit is missing, axis unit is taken.</remarks>
      public string Width;

      /// <summary>
      /// Specifies the distance between bars arranged side by side.
      /// </summary>
      /// <remarks>Can be a length or if unit is missing, axis unit is taken.</remarks>
      public string Shift;

      /// <summary>
      /// Specifies whether the labels should be automatically set to the interval boundaries.
      /// </summary>
      public bool TickLabelIntervalBoundaries;

      public override string ToString()
      {
         const string FORMAT_STRING = "{0}={1}";

         var text = new StringBuilder();

         switch (BarDirection)
         {
            case BarDirections.y:
               if (!string.IsNullOrEmpty(Shift))
                  text.AppendFormat(FORMAT_STRING, "ybar", Shift);
               else
                  text.Append("ybar");
               break;
            case BarDirections.x:
               if (!string.IsNullOrEmpty(Shift))
                  text.AppendFormat(FORMAT_STRING, "xbar", Shift);
               else
                  text.Append("xbar");
               break;
         }

         text.Append(getTextFor(BarPlotType));

         if (!string.IsNullOrEmpty(Width))
         {
            addSeparator(text);
            text.AppendFormat(FORMAT_STRING, "bar width", Width);
         }

         if (NodesNearCoords)
         {
            addSeparator(text);
            text.Append("nodes near coords");
         }

         if ((BarPlotType == BarPlotTypes.Interval || BarPlotType == BarPlotTypes.IntervalStacked) && TickLabelIntervalBoundaries)
         {
            addSeparator(text);
            switch (BarDirection)
            {
               case BarDirections.x:
                  text.Append("y");
                  break;
               case BarDirections.y:
                  text.Append("x");
                  break;
            }
            text.Append("ticklabel interval boundaries");
         }

         return text.ToString();      
      }

      private void addSeparator(StringBuilder text)
      {
         if (text.Length > 0) text.Append(", ");
      }

   }
}
