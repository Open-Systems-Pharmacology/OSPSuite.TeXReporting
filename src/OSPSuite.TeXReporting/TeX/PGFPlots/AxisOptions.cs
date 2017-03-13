using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.TeX.PGFPlots
{
   /// <summary>
   /// This class provides options which can be set on axis.</summary>
   /// <remarks>
   /// </remarks>
   public class AxisOptions
   {
      private readonly ITeXConverter _converter = DefaultConverter.Instance;

      public AxisOptions(ITeXConverter converter)
      {
         _converter = converter;
      }

      public enum AxisMode
      {
         log,
         normal
      }

      public enum BoxPlotDrawDirections
      {
         x,
         y
      }

      /// <summary>
      /// Group lines are used to make multiple level tick labels.
      /// </summary>
      /// <remarks>
      /// An own comment has been defined to allow to show multiple level x axis labels.
      /// </remarks>
      public class GroupLine
      {
         private readonly float _fromTick;
         private readonly float _toTick;
         private readonly string _label;
         public readonly int Level;

         /// <summary>
         /// Creates a group line.
         /// </summary>
         /// <param name="fromTick"></param>
         /// <param name="toTick"></param>
         /// <param name="label"></param>
         /// <param name="level"></param>
         public GroupLine(float fromTick, float toTick, string label, int level)
         {
            _fromTick = fromTick;
            _toTick = toTick;
            _label = label;
            Level = level;
         }

         private const float TICKOFFSET = 0.4F;

         public override string ToString()
         {
            var text = new StringBuilder();
            text.AppendFormat("draw group line={{{0}}}{{{1}}}{{{2}}}{{{3}}}",
                              (_fromTick - TICKOFFSET).ToString(CultureInfo.InvariantCulture),
                              (_toTick + TICKOFFSET).ToString(CultureInfo.InvariantCulture),
                              _label,
                              Level);
            return text.ToString();
         }
      }

      /// <summary>
      /// List of group lines.
      /// </summary>
      public IList<GroupLine> GroupLines = new List<GroupLine>();

      public BoxPlotDrawDirections BoxPlotDrawDirection = BoxPlotDrawDirections.y;

      public bool IsUsedForBoxPlots = false;

      public string BackgroundColor;

      public AxisMode XMode = AxisMode.normal;
      public AxisMode YMode = AxisMode.normal;

      public enum AxisXLine
      {
         box,
         top,
         middle,
         center,
         bottom,
         none
      }

      public bool XAxisIsHidden;
      public bool YAxisIsHidden;

      public LegendOptions LegendOptions;

      public enum AxisYLine
      {
         box, 
         left,
         middle,
         center,
         right,
         none
      }

      public AxisXLine XAxisPosition = AxisXLine.box;
      public AxisYLine YAxisPosition = AxisYLine.box;
      public bool XAxisArrow = false;
      public bool YAxisArrow = false;

      public string XLabel;
      public string YLabel;
      public string Title;

      public float? XMin;
      public float? XMax;
      public float? YMin;
      public float? YMax;

      /// <summary>
      /// Specifies the major ticks used for x axis.
      /// </summary>
      public float[] XTicks;
      /// <summary>
      /// Specifies the list of strings for x axis tick labels.
      /// </summary>
      public string[] XTickLabels;
      /// <summary>
      /// Specifies the distance between group lines.
      /// </summary>
      public string XGroupLineOffset;
      /// <summary>
      /// Specifies the distance between first group line and x tick labels.
      /// </summary>
      /// <remarks>This reserves the space needed for x tick labels. If there are long labels you have to increase it.</remarks>
      public string XGroupLineTextOffset;
      /// <summary>
      /// Specifies the rotation angle used for x tick label rotation.
      /// </summary>
      public int XTickLabelsRotateBy = 0;
      /// <summary>
      /// Specifies the minor ticks used for x axis.
      /// </summary>
      public float[] XMinorTicks;
      /// <summary>
      /// Specifies the minimum tick which should be displayed.
      /// </summary>
      public float? XTickMin;
      /// <summary>
      /// Specifies the maximum tick which should be displayed.
      /// </summary>
      public float? XTickMax;
      /// <summary>
      /// Specifies the major ticks used for y axis.
      /// </summary>
      public float[] YTicks;
      /// <summary>
      /// Specifies the list of strings for y axis tick labels.
      /// </summary>
      public string[] YTickLabels;
      /// <summary>
      /// Specifies the rotation angle used for y tick label rotation.
      /// </summary>
      public int YTickLabelsRotateBy = 0;
      /// <summary>
      /// Specifies the minor ticks used for y axis.
      /// </summary>
      public float[] YMinorTicks;
      /// <summary>
      /// Specifies the minimum tick which should be displayed.
      /// </summary>
      public float? YTickMin;
      /// <summary>
      /// Specifies the maximum tick which should be displayed.
      /// </summary>
      public float? YTickMax;

      /// <summary>
      /// Specifies the minor of minor ticks between major ticks.
      /// </summary>
      public int? MinorXTickNum;
      /// <summary>
      /// Specifies the minor of minor ticks between major ticks.
      /// </summary>
      public int? MinorYTickNum;

      /// <summary>
      /// Should the tick labels for logarithmic axis be oriented on real numbers. Default is true.
      /// </summary>      
      public bool LogTicksWithFixedPoint = true;

      /// <summary>
      /// Should the tick labels be shorten by a factor if possible? Default is true.
      /// </summary>
      public bool XScaledTicks = true;
      /// <summary>
      /// Should the tick labels be shorten by a factor if possible? Default is true.
      /// </summary>
      public bool YScaledTicks = true;

      public enum Discontinuities
      {
         crunch,
         parallel,
         none
      }

      /// <summary>
      /// How should scale breaks be shown.
      /// </summary>
      public Discontinuities XDiscontinuity = Discontinuities.none;
      /// <summary>
      /// How should scale breaks be shown.
      /// </summary>
      public Discontinuities YDiscontinuity = Discontinuities.none;

      /// <summary>
      /// Should there be a grid for major ticks on x axis.
      /// </summary>
      public bool XMajorGrid;
      /// <summary>
      /// Should there be a grid for major ticks on y axis.
      /// </summary>
      public bool YMajorGrid;
      /// <summary>
      /// Should there be a grid for minor ticks on x axis.
      /// </summary>
      public bool XMinorGrid;
      /// <summary>
      /// Should there be a grid for minor ticks on y axis.
      /// </summary>
      public bool YMinorGrid;
      /// <summary>
      /// Should all axis be enlarged (~10%) so that there is an empty area around values.
      /// </summary>
      public bool EnlargeLimits;

      /// <summary>
      /// Sets the width of the axis.
      /// </summary>
      /// <remarks>
      /// It is setted automatically. Use it only if you really need it.
      /// </remarks>
      public string Width;
      /// <summary>
      /// Sets the height of the axis.
      /// </summary>
      /// <remarks>
      /// It is setted automatically. Use it only if you really need it.
      /// </remarks>
      public string Height;

      /// <summary>
      /// Is that an empty plot within a groupedplot environment.
      /// </summary>
      public bool IsEmptyGroupPlot;

      public override string ToString()
      {
         var optionsString = new StringBuilder();
         const string FORMAT_STRING = "{0}={1}\n";
         const string FORMAT_STRING_LABEL = "{0}={{{1}}}\n";

         if (IsEmptyGroupPlot)
         {
            return "group/empty plot";
         }

         optionsString.Append("ylabel near ticks");
         optionsString.Append(Helper.LineFeed());

         if (IsUsedForBoxPlots)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "boxplot/draw direction", BoxPlotDrawDirection.ToString());
         }

         foreach (var groupLineEntry in GroupLines)
         {
            addSeparator(optionsString);
            optionsString.Append(groupLineEntry);
         }

         addSeparator(optionsString);
         optionsString.AppendFormat(FORMAT_STRING, "axis x line" + (XAxisArrow ? String.Empty : "*"), XAxisPosition);

         addSeparator(optionsString);
         optionsString.AppendFormat(FORMAT_STRING, "axis y line" + (YAxisArrow ? String.Empty : "*"), YAxisPosition);

         if (XAxisIsHidden)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "hide x axis", "true");
         }
         if (YAxisIsHidden)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "hide y axis", "true");
         }

         if (!String.IsNullOrEmpty(BackgroundColor))
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "axis background/.style", "{" + String.Format(FORMAT_STRING, "fill", BackgroundColor) + "}");            
         }

         if (EnlargeLimits)
         {
            addSeparator(optionsString);
            optionsString.Append("enlargelimits");
            optionsString.Append(Helper.LineFeed());
         }

         if (LegendOptions != null)
         {
            addSeparator(optionsString);
            optionsString.Append(LegendOptions);
            optionsString.Append(Helper.LineFeed());
         }

         addSeparator(optionsString);
         optionsString.AppendFormat(FORMAT_STRING, "xmode", XMode.ToString());
         addSeparator(optionsString);
         optionsString.AppendFormat(FORMAT_STRING, "ymode", YMode.ToString());

         if (!string.IsNullOrEmpty(XLabel))
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING_LABEL, "xlabel", _converter.StringToTeX(XLabel));
         }
         if (!string.IsNullOrEmpty(YLabel))
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING_LABEL, "ylabel", _converter.StringToTeX(YLabel));
         }
         if (!string.IsNullOrEmpty(Title))
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING_LABEL, "title", _converter.StringToTeX(Title));
         }

         //ranges
         if (XMax != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "xmax", ((float)XMax).ToString(CultureInfo.InvariantCulture));
         }
         if (XMin != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "xmin", ((float)XMin).ToString(CultureInfo.InvariantCulture));
         }
         if (YMin != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "ymin", ((float)YMin).ToString(CultureInfo.InvariantCulture));
         }
         if (YMax != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "ymax", ((float)YMax).ToString(CultureInfo.InvariantCulture));
         }

         if (XTicks != null && XTicks.Length > 0)
         {
            addSeparator(optionsString);
            var ticks = string.Join(",", Array.ConvertAll(XTicks, f => f.ToString(CultureInfo.InvariantCulture)));
            optionsString.AppendFormat(FORMAT_STRING_LABEL, "xtick", ticks);            
         }
         if (XTickLabels != null && XTickLabels.Length > 0)
         {
            addSeparator(optionsString);
            var tickLabels = string.Format("{{{0}}}", string.Join("}, {", _converter.StringToTeX(XTickLabels)));
            optionsString.AppendFormat(FORMAT_STRING_LABEL, "xticklabels", tickLabels);
         }
         if (!string.IsNullOrEmpty(XGroupLineOffset))
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "group line offset", XGroupLineOffset);
         }
         if (!string.IsNullOrEmpty(XGroupLineTextOffset))
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "text offset", XGroupLineTextOffset);
         }
         if (XTickLabelsRotateBy % 360 != 0)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat("xticklabel style = {{rotate={0}, anchor=east, yshift={1}}}", XTickLabelsRotateBy % 360, Helper.Length(-6, Helper.MeasurementUnits.pt));
         }
         if (XMinorTicks != null && XMinorTicks.Length > 0)
         {
            addSeparator(optionsString);
            var ticks = string.Join(",", Array.ConvertAll(XMinorTicks, f => f.ToString(CultureInfo.InvariantCulture)));
            optionsString.AppendFormat(FORMAT_STRING_LABEL, "minor xtick", ticks);
         }
         if (YTicks != null && YTicks.Length > 0)
         {
            addSeparator(optionsString);
            var ticks = string.Join(",", Array.ConvertAll(YTicks, f => f.ToString(CultureInfo.InvariantCulture)));
            optionsString.AppendFormat(FORMAT_STRING_LABEL, "ytick", ticks);
         }
         if (YTickLabels != null && YTickLabels.Length > 0)
         {
            addSeparator(optionsString);
            var tickLabels = string.Format("{{{0}}}", string.Join("}, {", _converter.StringToTeX(YTickLabels)));
            optionsString.AppendFormat(FORMAT_STRING_LABEL, "yticklabels", tickLabels);
         }
         if (YTickLabelsRotateBy % 360 != 0)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat("yticklabel style = {{rotate={0}, anchor=east, xshift={1}}}", XTickLabelsRotateBy % 360, Helper.Length(-6, Helper.MeasurementUnits.pt));
         }
         if (YMinorTicks != null && YMinorTicks.Length > 0)
         {
            addSeparator(optionsString);
            var ticks = string.Join(",", Array.ConvertAll(YMinorTicks, f => f.ToString(CultureInfo.InvariantCulture)));
            optionsString.AppendFormat(FORMAT_STRING, "minor ytick", String.Format("{{{0}}}", ticks));
         }
         //tick ranges
         if (XTickMax != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "xtickmax", ((float)XTickMax).ToString(CultureInfo.InvariantCulture));
         }
         if (XTickMin != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "xtickmin", ((float)XTickMin).ToString(CultureInfo.InvariantCulture));
         }
         if (YTickMin != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "ytickmin", ((float)YTickMin).ToString(CultureInfo.InvariantCulture));
         }
         if (YTickMax != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "ytickmax", ((float)YTickMax).ToString(CultureInfo.InvariantCulture));
         }

         // minor tick num
         if (MinorXTickNum != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "minor x tick num", MinorXTickNum);
         }
         if (MinorYTickNum != null)
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "minor y tick num", MinorYTickNum);
         }

         if (XMode == AxisMode.log || YMode == AxisMode.log)
         {
            if (LogTicksWithFixedPoint)
            {
               addSeparator(optionsString);
               optionsString.Append("log ticks with fixed point");
            }
         }

         // scaled ticks
         addSeparator(optionsString);
         optionsString.AppendFormat(FORMAT_STRING, "scaled x ticks", XScaledTicks ? "true" : "false");

         addSeparator(optionsString);
         optionsString.AppendFormat(FORMAT_STRING, "scaled y ticks", YScaledTicks ? "true" : "false");

         //discontinuities
         addSeparator(optionsString);
         optionsString.AppendFormat(FORMAT_STRING, "axis x discontinuity", XDiscontinuity);
         addSeparator(optionsString);
         optionsString.AppendFormat(FORMAT_STRING, "axis y discontinuity", YDiscontinuity);

         //grid
         if (XMajorGrid)
         {
            addSeparator(optionsString);
            optionsString.Append("xmajorgrids");
         }
         if (YMajorGrid)
         {
            addSeparator(optionsString);
            optionsString.Append("ymajorgrids");
         }
         if (XMinorGrid)
         {
            addSeparator(optionsString);
            optionsString.Append("xminorgrids");
         }
         if (YMinorGrid)
         {
            addSeparator(optionsString);
            optionsString.Append("yminorgrids");
         }

         //dimensions
         if (!String.IsNullOrEmpty(Width))
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "width", Width);
         }
         if (!String.IsNullOrEmpty(Height))
         {
            addSeparator(optionsString);
            optionsString.AppendFormat(FORMAT_STRING, "height", Height);
         }

         return optionsString.ToString();
      }

      private void addSeparator(StringBuilder text)
      {
         if (text.Length > 0) text.Append(", ");
      }
   }

}
