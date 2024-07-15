using System;
using System.IO;

namespace OSPSuite.TeXReporting.Tests
{
   public static class ConstantsForSpecs
   {
      public static string TemplatesFolder
      {
         get
         {
            var rootDir = AppDomain.CurrentDomain.BaseDirectory;
            return Path.GetFullPath(rootDir + @"\..\..\..\..\..\src\Templates");
         }
      }

      public static string StandardTemplateFolder
      {
         get { return Path.Combine(TemplatesFolder, "StandardTemplate"); }
      }
   }
}