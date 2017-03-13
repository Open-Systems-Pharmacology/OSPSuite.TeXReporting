using System;
using System.Text;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.TeX.PGFPlots
{
   public class PlotOptions
   {
      public PlotOptions Clone()
      {
         var dummy = BoxPlotPrepared;
         BoxPlotPrepared = null;
         var clone= MemberwiseClone().DowncastTo<PlotOptions>();
         BoxPlotPrepared = dummy;
         if (dummy != null)
            clone.BoxPlotPrepared = dummy.Clone();
         return clone;
      }

      /// <summary>
      /// Should the plot be shown in the legend?
      /// </summary>
      /// <remarks>Tex takes all legend entries an associates it to the plots added by occurance. To skip a plot set this property to true.</remarks>
      public bool ShowInLegend = true;

      /// <summary>
      /// Should just a legend entry be created.
      /// </summary>
      public bool IsLegendPlot = false;

      public enum PlotTypes
      {
         /// <summary>
         /// All coordinates are combined by lines.
         /// </summary>
         LinearPlot,
         /// <summary>
         /// All coordinates are combined by spline curves.
         /// </summary>
         SmoothPlot,
         /// <summary>
         /// Marks, colors and other settings are changed depending on data.
         /// </summary>
         ScatterPlot,
         /// <summary>
         /// Specifies the plot as a bar plot.
         /// </summary>
         BarPlot
      }

      /// <summary>
      /// Specifies the used plot type.
      /// </summary>
      public PlotTypes PlotType;

      private string getPlotTypeText()
      {
         switch (PlotType)
         {
            case PlotTypes.LinearPlot:
               return "sharp plot";
            case PlotTypes.SmoothPlot:
               return "smooth";
            case PlotTypes.ScatterPlot:
               return "scatter";
            default:
               return string.Empty;
         }
      }

      /// <summary>
      /// Specifies the limits used for plotting the boxes and whiskers.
      /// </summary>
      /// <remarks>The coordinates y values are used as outliers.</remarks>
      public BoxPlotPrepared BoxPlotPrepared;

      public enum LineStyles
      {
         None,
         Solid,
         Dotted,
         DenselyDotted,
         LooselyDotted,
         Dashed,
         DenselyDashed,
         LooselyDashed,
         DashDotted,
         DenselyDashDotted,
         LooselyDashDotted,
         DashDotDotted,
         DenselyDashDotDotted,
         LooselyDashDotDotted
      }

      /// <summary>
      /// Specifies the style of the line.
      /// </summary>
      /// <remarks>If set to none, only marks are plotted (Scatter Plot).</remarks>
      public LineStyles LineStyle = LineStyles.None;

      private string getLineStyleText(LineStyles lineStyle)
      {
         switch (lineStyle)
         {
            case LineStyles.Solid:
               return "solid";
            case LineStyles.Dotted:
               return "dotted";
            case LineStyles.DenselyDotted:
               return "densely dotted";
            case LineStyles.LooselyDotted:
               return "loosely dotted";
            case LineStyles.Dashed:
               return "dashed";
            case LineStyles.DenselyDashed:
               return "densely dashed";
            case LineStyles.LooselyDashed:
               return "loosely dashed";
            case LineStyles.DashDotted:
               return "dashdotted";
            case LineStyles.DenselyDashDotted:
               return "densely dashdotted";
            case LineStyles.LooselyDashDotted:
               return "loosely dashdotted";
            case LineStyles.DashDotDotted:
               return "dashdotdotted";
            case LineStyles.DenselyDashDotDotted:
               return "densely dashdotdotted";
            case LineStyles.LooselyDashDotDotted:
               return "loosely dashdotdotted";
            default:
               return "only marks";
         }
      }

      public enum Markers
      {
         //more plot could be defined. See pgfplots manual 4.6 on page 134.
         None,
         Circle,
         Diamond,
         Triangle,
         Square
      }

      public Markers Marker = Markers.None;

      private string getMarkerText()
      {
         switch (Marker)
         {
            case Markers.Circle:
               return "*";
            case Markers.Diamond:
               return "diamond*";
            case Markers.Triangle:
               return "triangle*";
            case Markers.Square:
               return "square*";
            case Markers.None:
               return ErrorBars ? "-" : "none";
            default:
               return "none";
         }
      }

      /// <summary>
      /// This specifies the size of the marks. 
      /// </summary>
      /// <remarks>Use <see cref="Helper.Length"/> to specify the string.</remarks>
      public string MarkSize;

      public string MarkColor;
      public string MarkFillColor;

      public enum Thicknesses
      {
         Thin,
         UltraThin,
         VeryThin,
         SemiThick,
         Thick,
         VeryThick,
         UltraThick
      }

      public LineStyles MarkStyle = LineStyles.Solid;

      public Thicknesses Thickness;

      public string GetThicknessText()
      {
         switch (Thickness)
         {
            case Thicknesses.Thin:
               return "thin";
            case Thicknesses.UltraThin:
               return "ultra thin";
            case Thicknesses.VeryThin:
               return "very thin";
            case Thicknesses.SemiThick:
               return "semithick";
            case Thicknesses.Thick:
               return "thick";
            case Thicknesses.VeryThick:
               return "very thick";
            case Thicknesses.UltraThick:
               return "ultra thick";
            default:
               return "thick";
         }
      }

      /// <summary>
      /// This specifies the size of the marks. 
      /// </summary>
      /// <remarks>Use <see cref="Helper.Length"/> to specify the string.</remarks>
      public string ThicknessSize;

      public bool ErrorBars;

      public bool ShadedErrorBars;

      /// <summary>
      /// This is the opacity used for shaded error bars.
      /// </summary>
      /// <remarks>Range is from 0 to 1.</remarks>
      public string Opacity = "0.25";

      public enum ErrorTypes
      {
         arithmetic,
         geometric,
         relative
      }

      public ErrorTypes ErrorType = ErrorTypes.arithmetic;

      public bool ErrorRelative {
         get { return (ErrorType == ErrorTypes.relative); }
      }

      public bool ErrorGeometric
      {
         get { return (ErrorType == ErrorTypes.geometric); }
      }

      public bool ErrorArithmetic
      {
         get { return (ErrorType == ErrorTypes.arithmetic); }
      }

      /// <summary>
      /// Specifies the color of the plot.
      /// </summary>
      public string Color = "blue";

      /// <summary>
      /// Specifies the color for fillings.
      /// </summary>
      public string FillColor;

      internal string Label;

      public override string ToString()
      {
         var text = new StringBuilder();
         const string FORMAT_STRING = "{0}={1} ";
         const string FORMAT_STRING2 = "{0}={{{1}}} ";

         text.Append(getPlotTypeText());

         if (!ShowInLegend)
         {
            addSeparator(text);
            text.Append("forget plot");
         }

         addSeparator(text);
         text.Append(Color);

         if (!string.IsNullOrEmpty(FillColor))
         {
            addSeparator(text);
            text.AppendFormat(FORMAT_STRING, "fill", FillColor);
         }

         addSeparator(text);
         text.Append(getLineStyleText(LineStyle));

         addSeparator(text);
         if (!string.IsNullOrEmpty(ThicknessSize))
            text.AppendFormat(FORMAT_STRING, "line width", ThicknessSize);
         else
            text.Append(GetThicknessText());

         addSeparator(text);
         text.AppendFormat(FORMAT_STRING, "mark", getMarkerText());

         if (!string.IsNullOrEmpty(MarkSize))
         {
            addSeparator(text);
            text.AppendFormat(FORMAT_STRING, "mark size", MarkSize);            
         }

         addSeparator(text);
         text.AppendFormat(FORMAT_STRING2, "mark options",
                           string.Format("{0}, {1}, {2}", getLineStyleText(MarkStyle),
                                         String.IsNullOrEmpty(MarkColor) ? Color : MarkColor,
                                         String.Format(FORMAT_STRING, "fill",
                                                       String.IsNullOrEmpty(MarkFillColor)
                                                          ? String.IsNullOrEmpty(MarkColor) ? Color : MarkColor
                                                          : MarkFillColor)));          

         if (ErrorBars & !ErrorGeometric)
         {
            addSeparator(text);
            text.Append("error bars/.cd, error bar style={solid},y dir=both, y explicit");
            if (ErrorRelative)
               text.Append(" relative");
         }

         if (BoxPlotPrepared != null)
         {
            addSeparator(text);
            text.Append(BoxPlotPrepared);
         }

         return text.ToString();
      }

      private void addSeparator(StringBuilder text)
      {
         if (text.Length > 0) text.Append(", ");
      }
   }
}
