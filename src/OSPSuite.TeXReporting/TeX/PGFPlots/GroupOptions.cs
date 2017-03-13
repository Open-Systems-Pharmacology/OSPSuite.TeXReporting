using System;
using System.Text;
using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.TeX.PGFPlots
{

   public class GroupLegendOptions
   {
      private readonly ITeXConverter _converter = DefaultConverter.Instance;

      public GroupLegendOptions()
      {
         
      }

      public GroupLegendOptions(ITeXConverter converter)
      {
         _converter = converter;
      }

      private string _legendName;
    
      public string[] LegendEntries;

      /// <summary>
      /// These are the legend options for the group legend.
      /// </summary>
      public LegendOptions LegendOptions;

      public override string ToString()
      {
         if (LegendEntries.Length == 0) 
            return String.Empty;
         var text = new StringBuilder();
         text.Append(", ");

         _legendName = Helper.Marker();
         text.AppendFormat("legend to name={0}", _legendName);
         
         addSeparator(text);
         text.AppendFormat("legend entries={{{0}}}", string.Join(", ", _converter.StringToTeX(LegendEntries)));
         
         addSeparator(text);
         text.Append(LegendOptions);
         
         return text.ToString();
      }

      private void addSeparator(StringBuilder text)
      {
         if (text.Length > 0) text.Append(", ");
      }

      /// <summary>
      /// This method is needed for placing the legend around the grouped plots.
      /// </summary>
      /// <param name="columns">Columns used in group.</param>
      /// <param name="rows">Rows used in group.</param>
      /// <param name="verticalSpaceinRows">Amount of rows vertically skipped. Needed for group lines.</param>
      /// <returns>TEX string which creates the node with the referenced legend.</returns>
      internal string PlaceLegendNode(int columns, int rows, int verticalSpaceinRows = 1)
      {
         var text = new StringBuilder();
         var positionText = new StringBuilder();

         const string everyAxisTitleShift = "\\pgfkeysvalueof{/pgfplots/every axis title shift}";

         string optionsText;

         switch (LegendOptions.LegendPosition)
         {
            case LegendOptions.LegendPositions.OuterSouthWest:
               positionText.AppendFormat("(group c1r{0}.south west)", rows);
               positionText.Append(getVerticalSpace(-verticalSpaceinRows));
               optionsText = string.Format("yshift=-{0}, anchor=north west", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.OuterSouthEast:
               positionText.AppendFormat("(group c{0}r{1}.south east)", columns, rows);
               positionText.Append(getVerticalSpace(-verticalSpaceinRows));
               optionsText = string.Format("yshift=-{0}, anchor=north east", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.NorthWest:
               positionText.AppendFormat("(group c1r1.north west)");
               optionsText = string.Format("xshift=-{0}, yshift=-{0}, anchor=north west", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.OuterNorthWest:
               positionText.AppendFormat("(group c1r1.north west)");
               optionsText = string.Format("left, xshift=-{0}, anchor=north east", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.OuterNorthEast:
               positionText.AppendFormat("(group c{0}r1.north east)", columns);
               optionsText = string.Format("right, xshift={0}, anchor=north west", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.NorthEast:
               positionText.AppendFormat("(group c{0}r1.north east)", columns);
               optionsText = string.Format("xshift=-{0}, yshift=-{0}, anchor=north east", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.OuterNorth:
               positionText.Append(getPositionCenter(columns, "north"));
               positionText.Append(getVerticalSpace(4));
               optionsText = string.Format("yshift=-{0}, anchor=south", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.SouthEast:
               positionText.AppendFormat("(group c{0}r{1}.south east)", columns, rows);
               optionsText = string.Format("xshift=-{0}, yshift={0}, anchor=south east", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.OuterSouth:
               positionText.Append(getPositionCenter(columns, rows, "south"));
               positionText.Append(getVerticalSpace(-verticalSpaceinRows));
               optionsText = string.Format("below, yshift=-{0}", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.SouthWest:
               positionText.AppendFormat("(group c1r{0}.south west)", rows);
               optionsText = string.Format("xshift={0}, yshift={0}, anchor=south west", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.South:
               positionText.Append(getPositionCenter(columns, rows, "south"));
               optionsText = string.Format("yshift={0}, anchor=south", everyAxisTitleShift);
               break;
            case LegendOptions.LegendPositions.North:
               positionText.Append(getPositionCenter(columns, "north"));
               optionsText = string.Format("yshift=-{0}, anchor=north", everyAxisTitleShift);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }

         text.AppendFormat("\\node (legend) at (${0}$) [{1}] {{\\pgfplotslegendfromname{{{2}}}}};\n", positionText, optionsText, _legendName);
         return text.ToString();
      }

      private string getVerticalSpace(int verticalSpaceinRows)
      {
         const string addVerticalSpaceFormat = "+ (0,{0})";
         return string.Format(addVerticalSpaceFormat, Helper.BaseLineSkip(verticalSpaceinRows));
      }

      private static string getPositionCenter(int columns, string position)
      {
         if (columns > 1)
            return string.Format("(group c1r1.{1})!0.5!(group c{0}r1.{1})", columns, position);
         return string.Format("(group c1r1.{0})", position);
      }

      private static string getPositionCenter(int columns, int rows, string position)
      {
         if (columns > 1)
            return string.Format("(group c1r{1}.{2})!0.5!(group c{0}r{1}.{2})", columns, rows, position);
         return string.Format("(group c1r{0}.{1})", rows, position);
      }

   }

   /// <summary>
   /// For defining the structure of the matrix and where labels and ticks are placed.
   /// </summary>
   public class GroupOptions
   {
      private readonly ITeXConverter _converter = DefaultConverter.Instance;

      public GroupOptions(ITeXConverter converter)
      {
         _converter = converter;
      }

      public enum GroupXPositions
      {
         All,
         EdgeTop,
         EdgeBottom
      }

      public enum GroupYPositions
      {
         All,
         EdgeLeft,
         EdgeRight
      }

      public GroupLegendOptions GroupLegendOptions;

      public GroupOptions()
      {
         GroupLegendOptions = new GroupLegendOptions(_converter);
      }

      public string PlaceLegend(int verticalSpaceinRows)
      {
         if (GroupLegendOptions == null) 
            return string.Empty;

         return GroupLegendOptions.PlaceLegendNode(Columns, Rows, verticalSpaceinRows);
      }

      public GroupXPositions XLabelsAt = GroupXPositions.All;
      public GroupYPositions YLabelsAt = GroupYPositions.All;

      public GroupXPositions XTickLabelsAt = GroupXPositions.All;
      public GroupYPositions YTickLabelsAt = GroupYPositions.All;

      public int Columns = 1;
      public int Rows = 1;

      public string HorizontalSep = Helper.Length(2, Helper.MeasurementUnits.pt);
      public string VerticalSep = Helper.Length(2, Helper.MeasurementUnits.pt);

      public string Title;

      private string getPositionCenterNorth()
      {
         if (Columns > 1)
            return string.Format("(group c1r1.north)!0.5!(group c{0}r1.north)", Columns);
         return string.Format("(group c1r1.north)");
      }

      private string addVerticalSpace(int verticalSpaceinRows)
      {
         const string addVerticalSpaceFormat = " + (0,{0})";
         return string.Format(addVerticalSpaceFormat, Helper.BaseLineSkip(verticalSpaceinRows));
      }

      /// <summary>
      /// This method places a title on top of grouped plots.
      /// </summary>
      /// <param name="title"></param>
      /// <returns></returns>
      internal string PlaceGroupTitle(string title)
      {
         var position = getPositionCenterNorth();
         position += addVerticalSpace(1);

         return String.Format("\\node ({0}) at (${1}$) [above, yshift=\\pgfkeysvalueof{{/pgfplots/every axis title shift}}] {{{2}}};\n", Helper.Marker(), position, title);
      }

      public override string ToString()
      {
         var options = new StringBuilder();
         const string FORMAT_STRING = "{0}={1}";

         addSeparator(options);
         options.AppendFormat(FORMAT_STRING, "columns", Columns);
         addSeparator(options);
         options.AppendFormat(FORMAT_STRING, "rows", Rows);

         addSeparator(options);
         options.AppendFormat(FORMAT_STRING, "horizontal sep", HorizontalSep);
         addSeparator(options);
         options.AppendFormat(FORMAT_STRING, "vertical sep", VerticalSep);

         if (XLabelsAt != GroupXPositions.All)
         {
            addSeparator(options);
            options.AppendFormat(FORMAT_STRING, "xlabels at", getGroupXPositionsText(XLabelsAt));
         }
         if (YLabelsAt != GroupYPositions.All)
         {
            addSeparator(options);
            options.AppendFormat(FORMAT_STRING, "ylabels at", getGroupYPositionsText(YLabelsAt));
         }

         if (XTickLabelsAt != GroupXPositions.All)
         {
            addSeparator(options);
            options.AppendFormat(FORMAT_STRING, "xticklabels at", getGroupXPositionsText(XTickLabelsAt));
         }
         if (YTickLabelsAt != GroupYPositions.All)
         {
            addSeparator(options);
            options.AppendFormat(FORMAT_STRING, "yticklabels at", getGroupYPositionsText(YTickLabelsAt));
         }

         return String.Format("group style={{{0}}}{1}", options, GroupLegendOptions);
      }

      private void addSeparator(StringBuilder text)
      {
         if (text.Length > 0) text.Append(", ");
      }

      private string getGroupXPositionsText(GroupXPositions position)
      {
         switch (position)
         {
            case GroupXPositions.All:
               return "all";
            case GroupXPositions.EdgeBottom:
               return "edge bottom";
            case GroupXPositions.EdgeTop:
               return "edge top";
            default:
               return "all";
         }
      }

      private string getGroupYPositionsText(GroupYPositions position)
      {
         switch (position)
         {
            case GroupYPositions.All:
               return "all";
            case GroupYPositions.EdgeLeft:
               return "edge left";
            case GroupYPositions.EdgeRight:
               return "edge right";
            default:
               return "all";
         }
      }

   }
}
