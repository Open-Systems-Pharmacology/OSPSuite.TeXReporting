using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.Builder
{
   public class StringArrayTeXBuilder : TeXChunkBuilder<string[]>
   {
      public override string TeXChunk(string[] stringsToReport)
      {
         return ListWriter.ItemizedList(DefaultConverter.Instance.StringToTeX(stringsToReport));
      }
   }
}