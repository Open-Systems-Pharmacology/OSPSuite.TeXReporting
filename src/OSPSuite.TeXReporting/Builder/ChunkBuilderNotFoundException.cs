using System;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Builder
{
   class ChunkBuilderNotFoundException : Exception
   {
      public ChunkBuilderNotFoundException(object objectToReport)
         : base("No TEX Chunk Builder found for '{0}'".FormatWith(objectToReport.GetType()))
      {
      }
   }
}
