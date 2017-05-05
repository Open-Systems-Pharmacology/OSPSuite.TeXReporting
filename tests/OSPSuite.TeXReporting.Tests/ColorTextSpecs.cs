using System.Drawing;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Tests
{
   public class ColorTextSpecs : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "ColorText";
         var chapter = new Chapter("Test"); 

         _objectsToReport.Add(chapter);
         _objectsToReport.Add(new Section("Font Styles"));
         _objectsToReport.Add(new ColorText("Aqua", Color.Aqua));
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new ColorText("Red", Color.Red));
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new ColorText("Blue", Color.Blue));
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}