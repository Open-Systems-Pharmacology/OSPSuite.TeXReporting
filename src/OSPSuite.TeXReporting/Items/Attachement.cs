using System.IO;
using OSPSuite.Utility;

namespace OSPSuite.TeXReporting.Items
{
   public abstract class Attachement
   {
      private readonly string _subfolder;

      /// <summary>
      ///    Full path of attachement
      /// </summary>
      public string FullPath { get; protected set; }

      protected Attachement(string fullPath, string subfolder)
      {
         _subfolder = subfolder;
         FullPath = fullPath;
      }

      public string RelativeFilePath
      {
         get { return Path.Combine(".", _subfolder, FileName); }
      }

      public string FileName
      {
         get { return Path.GetFileName(FullPath); }
      }

      public void CopyToWorkingDirectory(string workingDir)
      {
         var subFolder = Directory.CreateDirectory(Path.Combine(workingDir, _subfolder));
         var target = Path.Combine(subFolder.FullName, FileName);
         if (!FileHelper.FileExists(target))
            File.Copy(FullPath, target);
      }
   }
}