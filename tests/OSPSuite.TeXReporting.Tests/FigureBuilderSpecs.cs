using System;
using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_pdf_report_for_a_figure : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "Figure";
         var figure = new Figure("Figure", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "figure.png"));
         _objectsToReport.Add(figure);

         _objectsToReport.Add(new Text("The image shown in {0} can also be included inline in a text: {1}.", new Reference(figure), new InlineImage(figure.FullPath)));
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}