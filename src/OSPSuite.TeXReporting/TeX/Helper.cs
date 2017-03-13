using System;
using System.Globalization;
using System.Text;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.TeX
{
   /// <summary>
   ///    This static class encapsulates tex syntax to make creating tex files easier and less error-prone.
   ///    <remarks>The functions of this class should be used to get TEX chunks which can easily be added to TEX streams or string builder objects.</remarks>
   /// </summary>
   public static class Helper
   {
      private const string NO_TOC_SYMBOL = "*";

      /// <summary>
      ///    These are supported measurement units for specifying special length values.
      /// </summary>
      public enum MeasurementUnits
      {
         /// <summary>
         ///    an inch.
         /// </summary>
         @in,

         /// <summary>
         ///    a centimeter.
         /// </summary>
         @cm,

         /// <summary>
         ///    a point is 1/72.27 inch, than means about 0.0138 inch or 0.3515 mm.
         /// </summary>
         @pt,

         /// <summary>
         ///    roughly the height of an 'x' in the current font
         /// </summary>
         @ex,

         /// <summary>
         ///    roughly the width of an 'M' (uppercase) in the current font
         /// </summary>
         @em
      }

      public static string Length(double value, MeasurementUnits unit)
      {
         return $"{value.ToString(CultureInfo.InvariantCulture)} {unit}";
      }

      public static string BaseLineSkip(int numberOfLines)
      {
         return $"{numberOfLines}\\baselineskip";
      }


      public static string GetLengthInPercentageOfTextHeight(double percentage)
      {
         const string TEXTHEIGHT = "\\textheight";
         var factor = (percentage / 100).ToString(CultureInfo.InvariantCulture);
         return factor == "1" ? TEXTHEIGHT : $"{factor}{TEXTHEIGHT}";
      }

      public static string GetLengthInPercentageOfTextWidth(double percentage)
      {
         const string TEXTWIDTH = "\\textwidth";
         var factor = (percentage / 100).ToString(CultureInfo.InvariantCulture);
         return factor == "1" ? TEXTWIDTH : $"{factor}{TEXTWIDTH}";
      }

      /// <summary>
      ///    These are supported environments, which must have a begin and end command.
      /// </summary>
      public enum Environments
      {
         // ReSharper disable InconsistentNaming
         /// <summary>
         ///    This environment centers all included tex objects horizontically.
         /// </summary>
         center,

         /// <summary>
         /// left aligned text.
         /// </summary>
         flushleft,

         /// <summary>
         /// right aligned text.
         /// </summary>
         flushright,

         /// <summary>
         ///    This environment puts all included tex objects into landscaped pages.
         /// </summary>
         landscape,

         /// <summary>
         ///    This environment is usefull to align equations. Use &amp; to mark positions which should used for alignment.
         ///    <example>
         ///       <code>\begin{align}
         /// \frac{\partial f}{\partial \xi} &amp;= - \xi e^{-\frac{\xi^2}{2}} \\
         /// \frac{\partial^2 f}{\partial \xi^2} &amp;= - e^{-\frac{\xi^2}{2}}+ \xi^2 e^{-\frac{\xi^2}{2}} \\
         /// \frac{\partial g}{\partial \xi} &amp;= (l+1) \xi^l \\
         /// \frac{\partial^2 g}{\partial \xi^2} &amp;= l(l+1)\xi^{l-1} 
         /// \end{align}</code>
         ///    </example>
         /// </summary>
         align,

         /// <summary>
         /// </summary>
         array,

         /// <summary>
         ///    This is a floating environment for figures.
         /// </summary>
         figure,

         /// <summary>
         ///    This is a floating environment for tables.
         /// </summary>
         table,

         /// <summary>
         ///    This is the content of a table.
         /// </summary>
         tabular,

         /// <summary>
         ///    This is the content of a table.
         /// </summary>
         tabularx,

         /// <summary>
         ///    This is the content of a table which can be accross multiple pages. Not usable in a floating environment.
         /// </summary>
         longtabu,

         /// <summary>
         ///    This is the content of a table.
         /// </summary>
         tabu,

         /// <summary>
         /// This is an environment which supports table notes. (See package threeparttablex)
         /// </summary>
         ThreePartTable,

         /// <summary>
         /// This is the environment for table notes within a three part table. (table, tablenotes, caption)
         /// </summary>
         tablenotes,

         /// <summary>
         ///    The math environment is used within paragraph mode to place mathematical symbols inline with the paragraph's text.
         /// </summary>
         math,

         /// <summary>
         ///    The displaymath environment is used to display an euquation is a seperated area.
         /// </summary>
         displaymath,

         /// <summary>
         /// The minipage environment can be used to define an area as page within the page.
         /// </summary>
         minipage,

         /// <summary>
         /// This is a plot environment (Using pgfplots). 
         /// </summary>
         tikzpicture,

         /// <summary>
         /// Axis environment (Using pgfplots).
         /// </summary>
         axis,

         /// <summary>
         /// Evironment for grouping several plots in a matrix.
         /// </summary>
         groupplot,
         // ReSharper restore InconsistentNaming
      }

      /// <summary>
      ///    This enumerate contains all structuring elements available.
      /// </summary>
      public enum StructureElements
      {
         // ReSharper disable InconsistentNaming
         /// <summary>
         ///    Parts are not allowed in letters.
         /// </summary>
         part,

         /// <summary>
         ///    Chapters are only allowed for books and reports.
         /// </summary>
         chapter,

         /// <summary>
         ///    Sections are not allowed in letters.
         /// </summary>
         section,

         /// <summary>
         ///    Subsections are not allowed in letters.
         /// </summary>
         subsection,

         /// <summary>
         ///    Subsubsections are not allowed in letters.
         /// </summary>
         subsubsection,

         /// <summary>
         ///    Paragraphs are not allowed in letters.
         /// </summary>
         paragraph,

         /// <summary>
         ///    Subparagraphs are not allowed in letters.
         /// </summary>
         subparagraph
         // ReSharper restore InconsistentNaming
      }

      /// <summary>
      ///    This method creates a tex chunk for a structure element without a special toctitle.
      /// </summary>
      /// <param name="element">Structur element.</param>
      /// <param name="name">Name of structure element.</param>
      /// <param name="converter">TEX converter</param>
      /// <param name="addToToc">Indicates whether the structure should be nummered and added to table of contents.</param>
      /// <returns>TEX chunk.</returns>
      internal static string CreateStructureElement(StructureElements element, string name, ITeXConverter converter, bool addToToc)
      {
         return $"\\{element}{(addToToc ? String.Empty : NO_TOC_SYMBOL)}{{{converter.StringToTeX(name)}}}\n";
      }

      /// <summary>
      ///    This method creates a tex chunk for a structure element with a special toctitle.
      /// </summary>
      /// <param name="element">>Structur element.</param>
      /// <param name="name">Name of structure element.</param>
      /// <param name="converter">TEX converter</param>
      /// <param name="toctitle">Name of the structure element used in table of contents. Shorter version of name.</param>
      /// <returns>TEX chunk.</returns>
      internal static string CreateStructureElement(StructureElements element, string name, ITeXConverter converter, string toctitle)
      {
         return $"\\{element}[{converter.StringToTeX(toctitle)}]{{{converter.StringToTeX(name)}}}\n";
      }

      public static string StartBlock()
      {
         return "{";
      }

      public static string EndBlock()
      {
         return "}\n";
      }

      /// <summary>
      ///    This method just creates a line feed.
      /// </summary>
      /// <returns></returns>
      internal static string LineFeed()
      {
         return "\n";
      }

      /// <summary>
      ///    This method creates a line break in the tex document.
      /// </summary>
      /// <remarks>Identical to a \newline command.</remarks>
      /// <returns>TEX chunk.</returns>
      internal static string LineBreak()
      {
         return "\\\\\n";
      }

      /// <summary>
      ///  This method creates a horizontal line.
      /// </summary>
      /// <param name="indendLength"></param>
      /// <param name="width"></param>
      /// <param name="thickness"></param>
      /// <returns></returns>
      internal static string Rule(string indendLength, string width, string thickness)
      {
         return $"\\rule[{indendLength}]{{{width}}}{{{thickness}}}";
      }

      /// <summary>
      ///    This method creates a page break in the tex document.
      /// </summary>
      /// <returns>TEX chunk.</returns>
      internal static string PageBreak()
      {
         return "\\pagebreak\n";
      }

      /// <summary>
      /// This method creates a space request so that content which is longer will be moved to next page.
      /// </summary>
      /// <param name="length">String representing a tex length.</param>
      /// <returns>TEX chunk.</returns>
      public static string Needspace(string length)
      {
         return $"\\Needspace{{{length}}}";
      }

      /// <summary>
      /// This method creates a nice text box with a title around the text.
      /// </summary>
      /// <param name="title">Title of box.</param>
      /// <param name="text">Text surrounded by box.</param>
      /// <returns>TEX chunk.</returns>
      public static string TextBox(string title, string text)
      {
         return $"\\textbox{{{title}}}{{{text}}}\n";
      }

      public static string SideBySide(string leftSide, string rightSide)
      {
         var tex = new StringBuilder();
         tex.Append(Begin(Environments.minipage, "t"));
         tex.Append("{0.45\\linewidth}\n");
         tex.Append(leftSide);
         tex.Append(End(Environments.minipage));
         tex.Append("\\hfill%\n");
         tex.Append(Begin(Environments.minipage, "t"));
         tex.Append("{0.45\\linewidth}\n");
         tex.Append(rightSide);
         tex.Append(End(Environments.minipage));
         return tex.ToString();
      }

      /// <summary>
      /// This method creates the text in bold font.
      /// </summary>
      /// <param name="text">Text to get in bold.</param>
      /// <returns>TEX chunk.</returns>
      public static string Bold(string text)
      {
         return $"\\textbf{{{text}}}";
      }

      /// <summary>
      /// This method creates the text in italic shape.
      /// </summary>
      /// <param name="text">Text to get in italic shape.</param>
      /// <returns>TEX chunk.</returns>
      public static string Italic(string text)
      {
         return $"\\textit{{{text}}}";
      }

      /// <summary>
      /// This method creates the text in slanted shape.
      /// </summary>
      /// <param name="text">Text to get in slanted shape.</param>
      /// <returns>TEX chunk.</returns>
      public static string Slanted(string text)
      {
         return $"\\textsl{{{text}}}";
      }

      /// <summary>
      ///    This method create three dots within the line.
      /// </summary>
      /// <returns>TEX chunk.</returns>
      public static string Dots()
      {
         return "\\ldots";
      }

      /// <summary>
      ///    This method creates centering command.
      /// </summary>
      /// <remarks>Declares that all text following is to be centered. Usefull in a table or figure environment.</remarks>
      /// <returns></returns>
      public static string Centering()
      {
         return "\\centering\n";
      }

      public static string NoIndent()
      {
         return "\\noindent\n";
      }

      public static string Begin(Environments environment)
      {
         return $"\\begin{{{environment}}}";
      }

      public static string Begin(Environments environment, string options)
      {
         return $"\\begin{{{environment}}}[{options}]";
      }

      public static string End(Environments environment)
      {
         return $"\\end{{{environment}}}\n";
      }

      /// <summary>
      ///    This method creates a new indention with no reflect to table of contents.
      /// </summary>
      /// <returns>TEX chunk.</returns>
      internal static string Par()
      {
         return String.Format("\\par\n");
      }

      /// <summary>
      /// This method creates a caption within the tex document.
      /// </summary>
      /// <remarks>The caption must already been converted to TEX conventions.</remarks>
      /// <param name="caption"></param>
      /// <returns>TEX chunk.</returns>
      internal static string Caption(string caption)
      {
         return $"\\caption{{{caption}}}\n";
      }

      /// <summary>
      ///    This method creates a marker within the tex document, to reference later.
      /// </summary>
      /// <seealso cref="Reference(string)" />
      /// <seealso cref="ReferenceWithPage(string)" />
      /// <remarks>
      ///    The marker must already been converted to TEX conventions.
      /// </remarks>
      /// <param name="marker">Name of the marker.</param>
      /// <returns>TEX chunk.</returns>
      internal static string Label(string marker)
      {
         return $"\\label{{{marker}}}";
      }

      /// <summary>
      /// This method generates a marker for references.
      /// </summary>
      /// <returns>Unique string.</returns>
      internal static string Marker()
      {
         return Guid.NewGuid().ToString().Replace("-", "");
      }

      /// <summary>
      ///    This methods creates a reference within the tex document to the given marker.
      /// </summary>
      /// <remarks>The marker must already been converted to TEX conventions.</remarks>
      /// <seealso cref="Label(string)" />
      /// <seealso cref="ReferenceWithPage(string)" />
      /// <param name="marker">Name of the marker.</param>
      /// <returns>TEX chunk.</returns>
      private static string Reference(string marker)
      {
         return $"\\autoref{{{marker}}}";
      }
      internal static string Reference(IReferenceable referenceable)
      {
         return Reference(referenceable.Label);
      }

      /// <summary>
      /// This methods creates a hypertarget for a hyperlink within the tex document at any place.
      /// </summary>
      /// <remarks>The marker and caption must already been converted to TEX conventions.</remarks>
      /// <seealso cref="HyperLink" />
      /// <param name="marker">Name of the marker.</param>
      /// <param name="caption">Text to appear (normally empty)</param>
      /// <returns>TEX chunk</returns>
      public static string HyperTarget(string marker, string caption = null)
      {
         return $"\\hypertarget{{{marker}}}{{{caption ?? string.Empty}}}\n";
      }

      /// <summary>
      /// This methods creates a hyperlink to a previously defined hypertarget within the tex document.
      /// </summary>
      /// <remarks>The marker and caption must already been converted to TEX conventions.</remarks>
      /// <seealso cref="HyperTarget" />
      /// <param name="marker">Name of the marker.</param>
      /// <param name="caption">Text to appear.</param>
      /// <returns>TEX chunk</returns>
      public static string HyperLink(string marker, string caption)
      {
         return $"\\hyperlink{{{marker}}}{{{caption}}}\n";
      }

      /// <summary>
      ///    This methods creates a reference within the tex document to the given marker and adds page number.
      /// </summary>
      /// <remarks>The marker must already been converted to TEX conventions.</remarks>
      /// <seealso cref="Label(string)" />
      /// <seealso cref="Reference(string)" />
      /// <param name="marker">Name of the marker.</param>
      /// <returns>TEX chunk.</returns>
      private static string ReferenceWithPage(string marker)
      {
         return String.Format("\\autoref{{{0}}} on page~\\pageref{{{0}}}", marker);
      }
      internal static string ReferenceWithPage(IReferenceable referenceable)
      {
         return ReferenceWithPage(referenceable.Label);
      }

      /// <summary>
      ///    This methods creates a footnote.
      /// </summary>
      /// <remarks>Footnotes unfortunately don't work with tables, as it is considered a bad practice.</remarks>
      /// <param name="footnote">Text for the footnote.</param>
      /// <param name="converter">Indicates whether texts should be converted or not.</param>
      /// <returns>TEX chunk.</returns>
      public static string Footnote(string footnote, ITeXConverter converter)
      {
         return $"\\footnote{{{converter.StringToTeX(footnote)}}}";
      }
   }
}