using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting
{
   public interface IReportCompiler
   {
      Task CompileReport(StringBuilder texContent, IEnumerable<Attachement> attachements, ReportSettings settings, BuildTracker buildTracker);
   }

   internal class ReportCompiler : IReportCompiler
   {
      private readonly ITeXCompiler _texCompiler;

      public ReportCompiler(ITeXCompiler texCompiler)
      {
         _texCompiler = texCompiler;
      }

      /// <summary>
      ///    This methods brings all information together to compile the generated TEX code with the predefined template to given
      ///    pdf file path.
      /// </summary>
      /// <param name="texContent">The generated TEX Content to be written</param>
      /// <param name="attachements">List of attachements that will be added to the report</param>
      /// <param name="settings">settings used to configure the report</param>
      /// <param name="buildTracker">The current build tracker used to compile the report</param>
      public async Task CompileReport(StringBuilder texContent, IEnumerable<Attachement> attachements, ReportSettings settings, BuildTracker buildTracker)
      {
         checkThatOutputFileIsWritable(buildTracker);

         //copy content of template folder into working directory
         var texFilePath = copyTemplateToOutputDirectory(settings, buildTracker.WorkingDirectory, buildTracker.ReportFileName + ".tex");

         //copy content file to working dir
         var workingContentFile = Path.Combine(buildTracker.WorkingDirectory, settings.ContentFileName + ".tex");
         File.WriteAllText(workingContentFile, texContent.ToString());

         //copy attachments
         attachements.Each(x => x.CopyToWorkingDirectory(buildTracker.WorkingDirectory));

         settings.Implement(texFilePath);

         var createdPDF = await _texCompiler.CompileTex(texFilePath, settings.NumberOfCompilations);

         //Report could not be found. Something went wrong during the creation process. Throw an exception
         if (!FileHelper.FileExists(createdPDF))
            throw new TeXReportCompilerException(buildTracker.WorkingDirectory);

         //creation successful: Copy to final location
         copyCreatedToPdfToDesiredLocation(createdPDF, buildTracker);
      }

      private void copyCreatedToPdfToDesiredLocation(string createdPDF, BuildTracker buildTracker)
      {
         checkThatOutputFileIsWritable(buildTracker);
         FileHelper.Copy(createdPDF, buildTracker.ReportFullPath);
      }

      private void checkThatOutputFileIsWritable(BuildTracker buildTracker)
      {
         var fileFullPath = buildTracker.ReportFullPath;

         FileHelper.TrySaveFile(fileFullPath, () => { });
      }

      private string copyTemplateToOutputDirectory(ReportSettings settings, string workingDir, string fileName)
      {
         //copy all files
         foreach (var templateFile in Directory.GetFiles(settings.TemplateFolder))
         {
            var destName = Path.GetFileName(templateFile);
            if (string.IsNullOrEmpty(destName))
               continue;

            //We rename the actual template to the name of the pdf file.
            if (string.Equals(destName, settings.TemplateName))
               destName = fileName;

            File.Copy(templateFile, Path.Combine(workingDir, destName), true);
         }

         //we return the path of the template file in the working folder
         return Path.Combine(workingDir, fileName);
      }
   }
}