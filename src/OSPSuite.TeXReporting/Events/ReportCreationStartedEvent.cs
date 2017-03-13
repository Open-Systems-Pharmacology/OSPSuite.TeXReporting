namespace OSPSuite.TeXReporting.Events
{
   public class ReportCreationStartedEvent
   {
      public string ReportFullPath { get; private set; }

      public ReportCreationStartedEvent(string reportFullPath)
      {
         ReportFullPath = reportFullPath;
      }
   }
}