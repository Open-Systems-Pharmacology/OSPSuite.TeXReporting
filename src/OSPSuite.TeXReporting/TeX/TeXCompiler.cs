using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.TeX
{
   public interface ITeXCompiler
   {
      /// <summary>
      ///    This method compiles a given tex file to pdf. Returns the full path of the created pdf
      /// </summary>
      /// <param name="texFileFullPath">Full path of the tex file that should be compiled.</param>
      /// <param name="numberOfCompilations">Number of compilations that will be started</param>
      /// <returns>the full path of the compiled tex file</returns>
      Task<string> CompileTex(string texFileFullPath, int numberOfCompilations);
   }

   internal class TeXCompiler : ITeXCompiler
   {
      public async Task<string> CompileTex(string texFileFullPath, int numberOfCompilations)
      {
         if (!File.Exists(texFileFullPath))
            throw new OSPSuiteException($"The tex file '{texFileFullPath}' does not exist!");

         if (Path.GetExtension(texFileFullPath) != ".tex")
            throw new OSPSuiteException("The file must be a tex file and have the extension .TeX.");

         var texifyExe = CompilerConfiguration.Texify;
         if (!File.Exists(texifyExe))
            throw new OSPSuiteException("The miktex installation is corrupted or not present. Texify could not be found.");

         var workingDir = Path.GetDirectoryName(texFileFullPath);
         if (string.IsNullOrEmpty(workingDir))
            throw new OSPSuiteException($"The fileName '{texFileFullPath}' was spefified without full path which is needed.");

         var texFile = Path.GetFileName((texFileFullPath));
         if (string.IsNullOrEmpty(texFile))
            throw new OSPSuiteException($"The fileName '{texFileFullPath}' is empty. A tex file must be specified.");

         var texFileName = FileHelper.FileNameFromFileFullPath(texFileFullPath);

         //check write access to working directory
         if (!FileHelper.HasWriteAccessToFolder(workingDir))
            throw new OSPSuiteException($"The working directory '{workingDir}' is write protected or does not exist.");

         var pathVariable = Environment.GetEnvironmentVariable("Path");
         if (!string.IsNullOrEmpty(pathVariable))
         {
            if (!pathVariable.ToUpper().Contains(CompilerConfiguration.MikTexExecutablePath.ToUpper()))
            {
               Environment.SetEnvironmentVariable("Path", $"{pathVariable}{(pathVariable.EndsWith(";") ? string.Empty : ";")}{CompilerConfiguration.MikTexExecutablePath}");
            }
         }

         await runCompiler(texFile, workingDir, numberOfCompilations);

         //return created pdf path 
         return Path.Combine(workingDir, string.Concat(texFileName, ".pdf"));
      }

      private Task runCompiler(string texFile, string workingDirectory, int numberOfCompilations)
      {
         return Task.Run(() =>
         {
            // Use ProcessStartInfo class
            var startInfo = new ProcessStartInfo
            {
               CreateNoWindow = true,
               UseShellExecute = true,
               FileName = CompilerConfiguration.Texify,
               WindowStyle = ProcessWindowStyle.Hidden,
               Arguments = $"--batch --pdf --quiet --tex-option=-shell-escape \"{texFile}\"",
               WorkingDirectory = workingDirectory
            };

            try
            {
               // Start the process with the info we specified.
               // Call WaitForExit and then the using statement will close.
               // start it twice so that everything is really done.
               // Although texify should ensure that everything is done, sometimes it fails. So give it a second chance.
               // Sometime the page number was incorrect so I decided to start it threetimes now.
               for (int i = 0; i < numberOfCompilations; i++)
               {
                  using (var exeProcess = Process.Start(startInfo))
                  {
                     exeProcess.WaitForExit();
                  }
               }
            }
            catch
            {
               /*should we really do nothing here ?*/
            }
         });
      }
   }
}