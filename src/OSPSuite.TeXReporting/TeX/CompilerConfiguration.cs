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
      public static string Texify
      {
         get
         {
            var texify = Path.Combine(MikTexExecutablePath, "texify.exe");
            if (!FileHelper.FileExists(texify))
               throw new MikTexInstallationException();
            return texify;
         }
      }

      /// <summary>
      ///    This is the complete path to the miktex ghostview executable.
      /// </summary>
      public static string MGS
      {
         get
         {
            var texify = Path.Combine(MikTexExecutablePath, "mgs.exe");
            if (!FileHelper.FileExists(texify))
               throw new MikTexInstallationException();
            return texify;
         }
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

               return Environment.GetEnvironmentVariable(MIK_TEX_INSTALL_DIR);
            }
            catch (Exception e)
            {
               throw new MikTexInstallationException(e);
            }
         }
      }
   }
}