using System;

namespace OSPSuite.TeXReporting.Builder
{
   class ChunkBuilderNotFoundException : Exception
   {
      public ChunkBuilderNotFoundException(object objectToReport)
         : base($"No TEX Chunk Builder found for '{objectToReport.GetType()}'")
      {
      }
   }
}