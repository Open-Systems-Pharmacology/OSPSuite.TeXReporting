using System;

namespace OSPSuite.TeXReporting.Builder
{
   public class BuilderNotFoundException : Exception
   {
      public BuilderNotFoundException(object objectToReport)
         : base($"No TEX Builder found for '{objectToReport.GetType()}'")
      {
      }
   }
}