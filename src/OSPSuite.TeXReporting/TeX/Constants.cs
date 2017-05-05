using System.Collections.Generic;

namespace OSPSuite.TeXReporting.TeX
{
   internal class Constants
   {
      public const string ArtifactFolderForPlots = "plots";
      public const string ArtifactFolderForFigures = "figures";

      public static IReadOnlyCollection<string> AllArtifactFolders = new List<string>
      {
         ArtifactFolderForPlots,
         ArtifactFolderForFigures
      };

      /// <summary>
      ///    These are the types of artifacts which should be saved when SaveArtifacts is enabled.
      /// </summary>
      public static IReadOnlyCollection<string> ArtifactExtensions = new List<string> {".pdf", ".png"};

      /// <summary>
      ///    Name of folder that will be created for a generated report
      ///    For example, if the report is report.pdf, the output folder will be report_Files
      /// </summary>
      public static string ArtifactOutputFolder = "Files";

      public static class Error
      {
         public static string CouldNotCreateReport(string workingFolder)
         {
            return $"Report could not be created. Please see log for details located under '{workingFolder}'";
         }
      }
   }
}