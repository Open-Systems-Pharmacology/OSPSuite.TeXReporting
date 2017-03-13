using System;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Builder
{
   public class BuilderNotFoundException : Exception
   {
      public BuilderNotFoundException(object objectToReport)
         : base("No TEX Builder found for '{0}'".FormatWith(objectToReport.GetType()))
      {
      }
   }
}