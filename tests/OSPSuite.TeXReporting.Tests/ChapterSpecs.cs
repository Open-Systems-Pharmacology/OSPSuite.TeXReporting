using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_pdf_report_for_a_chapter : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "Chapter";
         var chapter = new Chapter("One chapter");
         _objectsToReport.Add(chapter);
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}