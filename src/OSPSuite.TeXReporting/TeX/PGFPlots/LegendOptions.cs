using System;
using System.Text;

namespace OSPSuite.TeXReporting.TeX.PGFPlots
{
   /// <summary>
   /// This class provides legend options. 
   /// </summary>
   public class LegendOptions
   {
      /// <summary>
      /// Specifies the width of border line.
      /// </summary>
      public string LineWidth;

      /// <summary>
      /// Number of columns used in legend.
      /// </summary>
      /// <remarks>Use -1 to place lengend entries horizontally.</remarks>
      public int Columns = 1;

      /// <summary>
      /// Supported values for font sizes of legend entries.
      /// </summary>
      public enum FontSizes
      {
         large,
         normalsize,
         small,
         footnotesize,
         scriptsize,
         tiny
      }
      /// <summary>
      /// Specifies the font size of legend entries.
      /// </summary>
      public FontSizes FontSize = FontSizes.normalsize;

      /// <summary>
      /// This specifies the horizontal space for legend entries. If the space is not enough the entries gets wrapped.  
      /// </summary>
      /// <remarks>This can be used to directly influence the width of legends. 
      /// Use this only if you know you have long texts, because it can not shrink.</remarks>
      public string TextWidth;

      /// <summary>
      /// If set to true rounded corners are created with a radius of 3 pt.
      /// </summary>
      public bool RoundedCorners;

      /// <summary>
      /// Removes the border.
      /// </summary>
      public bool NoBorder;

      /// <summary>
      /// Alignments for legend entries.
      /// </summary>
      public enum LegendAlignments
      {
         /// <summary>
         /// Align entries left.
         /// </summary>
         left,
         /// <summary>
         /// Align entries right.
         /// </summary>
         right,
         /// <summary>
         /// Align entries in center position.
         /// </summary>
         center
      }

      /// <summary>
      /// Specify how legend entries should be aligned.
      /// </summary>
      public LegendAlignments LegendAlignment;
      internal static string TextForLegendAlignment(LegendAlignments legendAlignment)
      {
         string anchor;
         switch (legendAlignment)
         {
            case LegendAlignments.right:
               anchor = "east";
               break;
            case LegendAlignments.center:
               anchor = "center";
               break;
            default:
               anchor = "west";
               break;
         }
         return String.Format("{{anchor={0}}}", anchor);
      }

      /// <summary>
      /// Predefined positions of legend.
      /// </summary>
      public enum LegendPositions
      {
         /// <summary>
         /// Inner lower left corner.
         /// </summary>
         SouthWest,
         /// <summary>
         /// Inner lower right corner.
         /// </summary>
         SouthEast,
         /// <summary>
         /// Inner upper left corner.
         /// </summary>
         NorthWest,
         /// <summary>
         /// Inner upper right corner.
         /// </summary>
         NorthEast,
         /// <summary>
         /// On the right of the plot.
         /// </summary>
         OuterNorthEast,
         /// <summary>
         /// On the left of the plot.
         /// </summary>
         OuterNorth,
         /// <summary>
         /// Above the plot.
         /// </summary>         
         OuterNorthWest,
         /// <summary>
         /// On the outer right corner of the plot.
         /// </summary>
         OuterSouthEast,
         /// <summary>
         /// Under the plot.
         /// </summary>
         OuterSouth,
         /// <summary>
         /// On the outer left corner of the plot.
         /// </summary>
         OuterSouthWest,
         /// <summary>
         /// Inner lower center.
         /// </summary>
         South,
         /// <summary>
         /// Inner upper center.
         /// </summary>
         North
      }

      /// <summary>
      /// Specifies where the legend is placed.
      /// </summary>
      public LegendPositions LegendPosition;

      private string getLegendPositionText()
      {
         switch (LegendPosition)
         {
            case LegendPositions.SouthWest:
               return "at={(0.03, 0.03)}, anchor=south west";
            case LegendPositions.SouthEast:
               return "at={(0.97, 0.03)}, anchor=south east";
            case LegendPositions.South:
               return "at={(0.5, 0.03)}, anchor=south";
            case LegendPositions.NorthWest:
               return "at={(0.03, 0.97)}, anchor=north west";
            case LegendPositions.NorthEast:
               return "at={(0.97, 0.97)}, anchor=north east";
            case LegendPositions.North:
               return "at={(0.5, 0.97)}, anchor=north";
            case LegendPositions.OuterNorthEast:
               return "at={(1.2, 1)}, anchor=north west";
            case LegendPositions.OuterNorth:
               return "at={(0.5, 1.1)}, anchor=south";
            case LegendPositions.OuterNorthWest:
               return "at={(-0.2, 1)}, anchor=north east";
            case LegendPositions.OuterSouthEast:
               return "at={(1, -0.1)}, anchor=north east";
            case LegendPositions.OuterSouth:
               return "at={(0.5, -0.1)}, anchor=north";
            case LegendPositions.OuterSouthWest:
               return "at={(0, -0.1)}, anchor=north west";
            default:
               return "at={(0.97, 0.97)}, anchor=north east";
         }
      }

      public override string ToString()
      {
         const string FORMAT_STRING = "{0}={1}";

         var options = new StringBuilder();

         if (!String.IsNullOrEmpty(TextWidth))
         {
            addSeparator(options);
            options.AppendFormat(FORMAT_STRING, "nodes", "{text depth=," + String.Format(FORMAT_STRING, "text width", TextWidth) + "}");
         }

         addSeparator(options);
         options.AppendFormat(FORMAT_STRING, "cells", TextForLegendAlignment(LegendAlignment));

         if (!String.IsNullOrEmpty(LineWidth))
         {
            addSeparator(options);
            options.AppendFormat(FORMAT_STRING, "line width", LineWidth);
         }

         addSeparator(options);
         options.AppendFormat(FORMAT_STRING, "font", "\\" + FontSize);

         addSeparator(options);
         options.AppendFormat(FORMAT_STRING, "legend columns", Columns);

         if (NoBorder)
         {
            addSeparator(options);
            options.AppendFormat(FORMAT_STRING, "draw", "none");
         }

         if (RoundedCorners)
         {
            addSeparator(options);
            options.AppendFormat(FORMAT_STRING, "rounded corners", "3pt");
         }

         addSeparator(options);
         options.Append(getLegendPositionText());

         return  String.Format("legend style={{{0}}}", options);
      }

      private void addSeparator(StringBuilder text)
      {
         if (text.Length > 0) text.Append(", ");
      }
   }
}
