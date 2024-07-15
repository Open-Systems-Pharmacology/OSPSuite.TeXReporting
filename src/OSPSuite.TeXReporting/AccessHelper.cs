using System;
using System.IO;

namespace OSPSuite.TeXReporting
{
   public static class AccessHelper
   {
      /// <summary>
      ///    Returns <c>false</c> if the path is read only or do not have access to view the permissions otherwise <c>true</c>
      /// </summary>
      public static bool HasWriteAccessToFolder(string folderPath)
      {
         try
         {
            // Attempt to get a list of security permissions from the folder. 
            // This will raise an exception if the path is read only or do not have access to view the permissions. 
            new DirectoryInfo(folderPath).GetAccessControl();
            return true;
         }
         catch (UnauthorizedAccessException)
         {
            return false;
         }
      }

      /// <summary>
      ///    Returns <c>false</c> if the path is read only or do not have access to view the permissions otherwise <c>true</c>
      /// </summary>
      public static bool HasWriteAccessToFolder(DirectoryInfo directoryInfo) =>
         HasWriteAccessToFolder(directoryInfo.FullName);
   }
}