using System;
using System.IO;
using Microsoft.Win32;
using OSPSuite.Utility;

namespace OSPSuite.TeXReporting.TeX
{
   public static class CompilerConfiguration
   {
      private const string MIK_TEX_REGISTRY_PATH = @"HKEY_LOCAL_MACHINE\SOFTWARE\Open Systems Pharmacology\MikTex";
      private const string MIK_TEX_INSTALL_DIR = "MIK_TEX_INSTALL_DIR";
      private const string ERROR_MESSAGE = "There is something wrong with the Open Systems Pharmacology - MiKTeX installation.";

      public class MikTexInstallationException : Exception
      {
         public MikTexInstallationException() : base(ERROR_MESSAGE)
         {
         }

         public MikTexInstallationException(Exception inner) : base(ERROR_MESSAGE, inner)
         {
         }
      }

      /// <summary>
      ///    This is the complete path to the mixtek texify executable.
      /// </summary>
      public static string Texify=> retrieveFileOrThrow("texify.exe");

      /// <summary>
      ///    This is the complete path to the miktex ghostview executable.
      /// </summary>
      public static string MGS => retrieveFileOrThrow("mgs.exe");

      private static string retrieveFileOrThrow(string fileName)
      {
         var file = Path.Combine(MikTexExecutablePath, fileName);
         if (FileHelper.FileExists(file))
            return file;

          throw new MikTexInstallationException();
      } 

      public static string MikTexExecutablePath => Path.Combine(MikTEXPortablePath, "miktex", "bin");

      public static string MikTEXPortablePath
      {
         get
         {
            try
            {
               var path = (string) Registry.GetValue(MIK_TEX_REGISTRY_PATH, "InstallDir", null);
               if (!string.IsNullOrEmpty(path))
                  return path;

               path = Environment.GetEnvironmentVariable(MIK_TEX_INSTALL_DIR);
               if (!string.IsNullOrEmpty(path))
                  return path;

               throw new MikTexInstallationException();
            }
            catch (Exception e)
            {
               throw new MikTexInstallationException(e);
            }
         }
      }
   }
}