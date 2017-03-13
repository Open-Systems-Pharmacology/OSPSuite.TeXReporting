using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class ParTeXBuilder : TeXChunkBuilder<Par>
   {
      public override string TeXChunk(Par par)
      {
         return Helper.Par();
      }
   }
}