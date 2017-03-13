using OSPSuite.TeXReporting.TeX;
using OSPSuite.Utility.Exceptions;

namespace OSPSuite.TeXReporting
{
   public class TeXReportCompilerException : OSPSuiteException
   {
      public TeXReportCompilerException(string workingDirectory) : base(Constants.Error.CouldNotCreateReport(workingDirectory))
      {
      }
   }
}