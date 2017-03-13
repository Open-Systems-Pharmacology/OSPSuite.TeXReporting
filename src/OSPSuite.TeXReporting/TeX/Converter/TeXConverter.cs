using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OSPSuite.TeXReporting.TeX.Converter
{
   public interface ITeXConverter
   {
      string StringToTeX(string text);
      string[] StringToTeX(string[] text);
      string FilePathToTeX(string filePath);
   }

   public abstract class TeXConverter : ITeXConverter
   {
      /// <summary>
      /// Dictionary of char and their replacement.
      /// </summary>
      protected readonly IDictionary<char, string> _textCriticals = new Dictionary<char, string>
                                                            {
                                                                {'#', "\\#"},
                                                                {'$', "\\$"},
                                                                {'%', "\\%"},
                                                                {'&', "\\&"},
                                                                {'{', "\\{"},
                                                                {'}', "\\}"},
                                                                {'[', "{[}"},
                                                                {']', "{]}"},
                                                                {'~', "{\\raise.17ex\\hbox{$\\scriptstyle\\sim$}}"},
                                                                {'^', "\\textasciicircum "},
                                                                {'\\', "\\textbackslash "},
                                                                {'\n', "\\newline "},
                                                                {'<', "\\textless "},
                                                                {'>', "\\textgreater "},
                                                                {'|', "\\textbar \\-"}, //with \- it is allowed to make a word wrap
                                                                {'ä', "\\\"a"},
                                                                {'ö', "\\\"o"},
                                                                {'ü', "\\\"u"},
                                                                {'Ä', "\\\"A"},
                                                                {'Ö', "\\\"O"},
                                                                {'Ü', "\\\"U"},
                                                                {'ß', "\\ss"},
                                                                {'®', "\\textsuperscript{\\textregistered}"},
                                                                {'µ', "\\textmu "},
                                                                {'²', "$^2$"},
                                                                {'³', "$^3$"},
                                                                {'α', "$\\alpha$"},
                                                                {'β', "$\\beta$"},
                                                                {'∞', "$\\infty$"}
                                                            };

      /// <summary>
      /// Dictionary of char and their replacement.
      /// </summary>
      protected readonly IDictionary<char, string> _filePathCriticals = new Dictionary<char, string>
                                                                                {
                                                                                   {'\\', "/"},
                                                                                   {' ', "\\space "}
                                                                                };

      /// <summary>
      /// Purifies a given string and replaces critical symbols.
      /// </summary>
      /// <param name="text">To be converted.</param>
      /// <param name="criticals">Dictionary with predefined criticals and their replacement.</param>
      /// <returns>Purified string.</returns>
      protected string StringConverter(string text, IDictionary<char, string> criticals)
      {
         var purifiedText = new StringBuilder();
         var ia = 0;
         for (var i = 0; i < text.Length; i++)
         {
            var c = text[i];
            if (!criticals.ContainsKey(c)) continue;
            purifiedText.Append(text.Substring(ia, i - ia));
            purifiedText.Append(criticals[c]);
            ia = i + 1;
         }
         purifiedText.Append(text.Substring(ia));
         return purifiedText.ToString();
      }

      /// <summary>
      /// Purifies a given string and replaces critical symbols.
      /// </summary>
      /// <param name="text">To be converted.</param>
      /// <returns>Converted string.</returns>
      public virtual string StringToTeX(string text)
      {
         if (String.IsNullOrEmpty(text)) 
            return String.Empty;
         text = text.Replace("&&", "&");
         return StringConverter(text, _textCriticals);
      }

      public string[] StringToTeX(string[] texts)
      {
         return texts.Select(StringToTeX).ToArray();
      }

      /// <summary>
      /// Purifies a given file path and replaces critical symbols.
      /// </summary>
      /// <param name="filePath">To be converted.</param>
      /// <returns>Converted string.</returns>
      public virtual string FilePathToTeX(string filePath)
      {
         return StringConverter(filePath, _filePathCriticals);
      }
   }
}