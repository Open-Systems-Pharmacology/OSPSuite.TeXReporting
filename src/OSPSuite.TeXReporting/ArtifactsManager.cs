using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting
{
   public interface IArtifactsManager
   {
      Task SaveArtifacts(BuildTracker tracker);
   }

   internal class ArtifactsManager : IArtifactsManager
   {
      private readonly IImageConverter _imageConverter;

      public ArtifactsManager(IImageConverter imageConverter)
      {
         _imageConverter = imageConverter;
      }

      private void deleteDirectory(DirectoryInfo directory)
      {
         try
         {
            directory.GetFiles().ToList().ForEach(f => f.Delete());
            directory.Delete(true);
         }
         catch
         {
            //Catch all is bad, but in this situation it makes sense to avoid a show stopper
         }
      }

      public async Task SaveArtifacts(BuildTracker tracker)
      {
         var outputFolderPath = Path.Combine(tracker.ReportFolder, $"{tracker.ReportFileName}_{Constants.ArtifactOutputFolder}");
         var outputFolder = new DirectoryInfo(outputFolderPath);
         if (outputFolder.Exists)
            deleteDirectory(outputFolder);

         outputFolder.Create();

         foreach (var artifactFolderName in Constants.AllArtifactFolders)
         {
            var artifactWorkingFolder = new DirectoryInfo(Path.Combine(tracker.WorkingDirectory, artifactFolderName));
            if (!artifactWorkingFolder.Exists)
               continue;

            var artifactOutputFolder = Path.Combine(outputFolder.FullName, artifactFolderName);
            createDirectory(artifactOutputFolder);

            foreach (var artifact in allArtifactsFrom(artifactWorkingFolder))
            {
               var outputArtifactFullPath = Path.Combine(artifactOutputFolder, artifact.Name);
               artifact.CopyTo(outputArtifactFullPath, true);

               //convert the pdf plots to png format
               if (isAnPDFFile(artifact.Name))
                  await _imageConverter.ConvertPDF(outputArtifactFullPath, ImageTypes.PNG, 600);
            }
         }
      }

      private bool isAnPDFFile(string path)
      {
         var extension = Path.GetExtension(path);
         if (string.IsNullOrEmpty(extension)) return false;
         return (extension.ToUpper() == ".PDF");
      }

      private static IEnumerable<FileInfo> allArtifactsFrom(DirectoryInfo artifactWorkingFolder)
      {
         return artifactWorkingFolder.EnumerateFiles().Where(f => Constants.ArtifactExtensions.Contains(f.Extension.ToLower()));
      }

      private static void createDirectory(string directory)
      {
         if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
      }
   }
}