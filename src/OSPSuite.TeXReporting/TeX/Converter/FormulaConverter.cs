using System.Linq;

namespace OSPSuite.TeXReporting.TeX.Converter
{
   public class FormulaConverter : ITeXConverter
   {

      /// <summary>
      ///    The singleton instance of <see cref="FormulaConverter" />.
      /// </summary>
      /// <value>
      ///    The singleton instance of <see cref="FormulaConverter" />.
      /// </value>
      public static readonly FormulaConverter Instance = new FormulaConverter();

      public string StringToTeX(string text)
      {
         var newText = text;

         newText = newText.Replace("+", " + ");
         newText = newText.Replace("-", " - ");
         newText = newText.Replace("*", " * ");
         newText = newText.Replace("/", " / ");
         while (newText.Contains("  "))
            newText = newText.Replace("  ", " ");

         newText = DefaultConverter.Instance.StringToTeX(newText);
         return newText;
      }

      public string[] StringToTeX(string[] texts)
      {
         return texts.Select(StringToTeX).ToArray();
      }

      public string FilePathToTeX(string filePath)
      {
         return DefaultConverter.Instance.FilePathToTeX(filePath);
      }
   }
}
