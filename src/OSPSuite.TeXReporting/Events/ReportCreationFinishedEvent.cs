namespace OSPSuite.TeXReporting.Events
{
   public class ReportCreationFinishedEvent
   {
      public string ReportFullPath { get; private set; }

      public ReportCreationFinishedEvent(string reportFullPath)
      {
         ReportFullPath = reportFullPath;
      }
   }
}