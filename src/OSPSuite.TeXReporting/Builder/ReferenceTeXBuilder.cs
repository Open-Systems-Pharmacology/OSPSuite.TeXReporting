using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class ReferenceTeXBuilder : TeXChunkBuilder<Reference>
   {
      public override string TeXChunk(Reference reference)
      {
         return Helper.Reference(reference.Referenceable);
      }
   }
}