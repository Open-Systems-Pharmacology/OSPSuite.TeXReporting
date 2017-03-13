using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class LineBreakTeXBuilder : TeXChunkBuilder<LineBreak>
   {
      public override string TeXChunk(LineBreak lineBreak)
      {
         return Helper.LineBreak();
      }
   }
}
