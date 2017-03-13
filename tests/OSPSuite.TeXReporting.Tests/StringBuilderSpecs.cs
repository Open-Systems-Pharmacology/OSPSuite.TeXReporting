using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_pdf_report_for_a_string : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "StringReport";

         _objectsToReport.Add("Some strings added by range: ");
         _objectsToReport.Add(Helper.LineFeed());
         _objectsToReport.AddRange(new string[]
            {
               "Part1",
               "Part2"
            });

         _objectsToReport.Add(Helper.LineFeed());
         _objectsToReport.Add("Now added as single string array (should be translated to itemized list):");
         _objectsToReport.Add(new string[]
            {
               "Part1",
               "Part2"
            });

      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}