using System;
using System.IO;
using Microsoft.Win32;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;

namespace OSPSuite.TeXReporting.TeX
{
   public class MikTexInstallationException : OSPSuiteException
   {
      private const string ERROR_MESSAGE = "There is something wrong with the Open Systems Pharmacology - MiKTeX installation.";

      public MikTexInstallationException(string message) : base($"{ERROR_MESSAGE}\n{message}")
      {
      }
   }

   public static class CompilerConfiguration
   {
      private const string MIK_TEX_REGISTRY_PATH_X_86 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Open Systems Pharmacology\MikTex";
      private const string MIK_TEX_REGISTRY_PATH_X_64 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Open Systems Pharmacology\MikTex";
      private const string MIK_TEX_INSTALL_DIR = "MIK_TEX_INSTALL_DIR";
      private const string MIK_TEX_INSTALLATION_NOT_FOUND = "MiKTex installation not found";

      /// <summary>
      ///    This is the complete path to the mixtek texify executable.
      /// </summary>
      public static string Texify => retrieveFileOrThrow("texify.exe");

      /// <summary>
      ///    This is the complete path to the miktex ghostview executable.
      /// </summary>
      public static string MGS => retrieveFileOrThrow("mgs.exe");

      private static string retrieveFileOrThrow(string fileName)
      {
         var file = Path.Combine(MikTexExecutablePath, fileName);
         if (FileHelper.FileExists(file))
            return file;

         throw new MikTexInstallationException(MIK_TEX_INSTALLATION_NOT_FOUND);
      }

      public static string MikTexExecutablePath => Path.Combine(MikTexPortablePath, "miktex", "bin");

      private static string MikTexPortablePath
      {
         get
         {
            //32 bit install with 32 bit version of MikTex (or 64 bit install with 64bit version of MikTex)
            var path = (string) Registry.GetValue(MIK_TEX_REGISTRY_PATH_X_86, "InstallDir", null);
            if (!string.IsNullOrEmpty(path))
               return path;

            //64 bit install with 32 bit version of MikTex 
            path = (string)Registry.GetValue(MIK_TEX_REGISTRY_PATH_X_64, "InstallDir", null);
            if (!string.IsNullOrEmpty(path))
               return path;

            //Maybe environment variable was provided?
            path = Environment.GetEnvironmentVariable(MIK_TEX_INSTALL_DIR);
            if (!string.IsNullOrEmpty(path))
               return path;

            throw new MikTexInstallationException(MIK_TEX_INSTALLATION_NOT_FOUND);
         }
      }
   }
}