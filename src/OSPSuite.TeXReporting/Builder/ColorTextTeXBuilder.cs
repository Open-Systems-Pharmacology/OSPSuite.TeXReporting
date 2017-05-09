using System.Drawing;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Builder
{
   public class ColorTextTeXBuilder : TextTeXBuilder<ColorText>
   {
      public ColorTextTeXBuilder(ITeXBuilderRepository builderRepository) : base(builderRepository)
      {
      }

      public override string TeXChunk(ColorText text)
      {
         var baseText = base.TeXChunk(text);
         return colorizedText(baseText, text.Color);
      }

      private string colorizedText(string text, Color color)
      {
         return color.IsEmpty ? text : $"\\definecolor{{theColor}}{{RGB}}{{{color.R},{color.G},{color.B}}}\\textcolor{{theColor}}{{{text}}}";
      }
   }
}