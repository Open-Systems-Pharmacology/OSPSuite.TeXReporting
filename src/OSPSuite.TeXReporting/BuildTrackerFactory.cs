using System;
using System.IO;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;

namespace OSPSuite.TeXReporting
{
   public interface IBuildTrackerFactory
   {
      /// <summary>
      ///    Creates a new build tracker working directory based on the <paramref name="pdfFullPath" /> location.
      /// </summary>
      /// <param name="pdfFullPath">Full path of the pdf to create</param>
      /// <param name="workingDirName">Name of working directory in the folder where the pdf file will be generate, where all the bitmaps etc will be generated </param>
      /// <returns>the working directory</returns>
      T CreateFor<T>(string pdfFullPath, string workingDirName = null) where T : BuildTracker, new();
   }

   internal class BuildTrackerFactory : IBuildTrackerFactory
   {
      /// <summary>
      ///    This function is used to inject the way the subworking folder is created. Useful for testing
      /// </summary>
      public Func<string> WorkingDirName = () => Guid.NewGuid().ToString();

      public T CreateFor<T>(string pdfFullPath, string workingDirName = null) where T : BuildTracker, new()
      {
         var usedWorkingDir = workingDirName ?? WorkingDirName();

         var outputDir = Path.GetDirectoryName(pdfFullPath);
         if (string.IsNullOrEmpty(outputDir))
            throw new OSPSuiteException("The pdfFileName must be specified with full path.");

         if (!Directory.Exists(outputDir))
            throw new OSPSuiteException("The specified output directory does not exists.");

         var pdfFileName = Path.GetFileNameWithoutExtension((pdfFullPath));
         if (string.IsNullOrEmpty(pdfFileName))
            throw new OSPSuiteException("The pdf file name is empty. A pdf file must be specified.");

         if (!AccessHelper.HasWriteAccessToFolder(outputDir))
            throw new OSPSuiteException("The output directory is write protected or does not exist.");

         //create a unique working dir
         var workingDir = Directory.CreateDirectory(Path.Combine(outputDir, usedWorkingDir));

         return new T
            {
               WorkingDirectory = workingDir.FullName,
               ReportFullPath = pdfFullPath,
               ReportFileName = pdfFileName,
               ReportFolder =  outputDir
            };
      }
   }
}