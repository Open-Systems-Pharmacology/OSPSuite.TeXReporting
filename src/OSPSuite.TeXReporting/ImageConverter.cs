using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting
{
   internal enum ImageTypes
   {
      JPG,
      PNG,
      BMP
   }

   internal interface IImageConverter
   {
      /// <summary>
      ///    This method converts a given pdf file to an image format. Returns the full path of the created image file.
      /// </summary>
      /// <param name="pdfFileFullPath">Full path of the pdf file that should be converted.</param>
      /// <param name="imageType">Type of the output image.</param>
      /// <param name="resolution">Pixel/inch resolution of output.</param>
      /// <returns>the full path of the converted image file</returns>
      Task<string> ConvertPDF(string pdfFileFullPath, ImageTypes imageType, int resolution);
   }

   internal class ImageConverter : IImageConverter
   {
      public async Task<string> ConvertPDF(string pdfFileFullPath, ImageTypes imageType, int resolution = 144)
      {
         if (!File.Exists(pdfFileFullPath))
            throw new OSPSuiteException($"The pdf file '{pdfFileFullPath}' does not exist!");

         if (Path.GetExtension(pdfFileFullPath) != ".pdf")
            throw new OSPSuiteException("The file must be a pdf file and have the extension .pdf.");

         if (!File.Exists(CompilerConfiguration.MGS))
            throw new OSPSuiteException("The miktex installation is corrupted or not present. MGS could not be found.");

         var workingDir = Path.GetDirectoryName(pdfFileFullPath);
         if (String.IsNullOrEmpty(workingDir))
            throw new OSPSuiteException($"The fileName '{pdfFileFullPath}' was spefified without full path which is needed.");

         var pdfFile = Path.GetFileName((pdfFileFullPath));
         if (String.IsNullOrEmpty(pdfFile))
            throw new OSPSuiteException($"The fileName '{pdfFileFullPath}' is empty. A pdf file must be specified.");


         //check write access to working directory
         if (!AccessHelper.HasWriteAccessToFolder(workingDir))
            throw new OSPSuiteException($"The working directory '{workingDir}' is write protected or does not exist.");

         await runConverter(pdfFile, workingDir, resolution, imageType);

         //return created image path 
         var pdfFileName = FileHelper.FileNameFromFileFullPath(pdfFileFullPath);
         return Path.Combine(workingDir, string.Concat(pdfFileName, getOutputExtension(imageType)));
      }

      private static string getOutputType(ImageTypes imageType)
      {
         switch (imageType)
         {
            case ImageTypes.JPG:
               return "jpeg";
            case ImageTypes.PNG:
               return "pngalpha";
            case ImageTypes.BMP:
               return "bmp256";
            default:
               return "pngalpha";
         }
      }

      private static string getOutputExtension(ImageTypes imageType)
      {
         switch (imageType)
         {
            case ImageTypes.JPG:
               return ".jpg";
            case ImageTypes.PNG:
               return ".png";
            case ImageTypes.BMP:
               return ".bmp";
            default:
               return ".png";
         }
      }

      private Task runConverter(string pdfFile, string workingDirectory, int resolution, ImageTypes imageType)
      {
         return Task.Run(() =>
            {
               var outputFile = String.Concat(Path.GetFileNameWithoutExtension(pdfFile), getOutputExtension(imageType));
               // Use ProcessStartInfo class
               var startInfo = new ProcessStartInfo
                  {
                     CreateNoWindow = true,
                     UseShellExecute = true,
                     FileName = CompilerConfiguration.MGS,
                     WindowStyle = ProcessWindowStyle.Hidden,
                     Arguments = $"-sDEVICE={getOutputType(imageType)} -sOutputFile=\"{outputFile}\" -r{resolution} -q -dBATCH -dNOPAUSE \"{pdfFile}\"",
                     WorkingDirectory = workingDirectory
                  };

               try
               {
                  using (var exeProcess = Process.Start(startInfo))
                  {
                     exeProcess.WaitForExit();
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