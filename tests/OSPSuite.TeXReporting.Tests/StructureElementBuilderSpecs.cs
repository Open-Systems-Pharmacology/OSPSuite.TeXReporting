using System;
using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Tests
{
   class When_creating_a_pdf_report_for_structure_elements : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "StructureElements";
         createReport();
      }

      private void createReport()
      {
         var someText = "This is some text, just to fill some words between structure elements";
         var figure = new Figure("Figure", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "figure.png"));
         var inlineImage = new InlineImage(figure.FullPath);

         _objectsToReport.Add(new Part("This is the first part using simple strings"));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new Chapter("Simple Chapter"));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(figure);
         _objectsToReport.Add(new Section("Simple Section"));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new SubSection("Simple SubSection"));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new SubSubSection("Simple SubSubSection"));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new Paragraph("Simple Paragraph"));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new SubParagraph("Simple SubParagraph"));
         _objectsToReport.Add(someText);

         _objectsToReport.Add(new Part("This is the first part using text objects with inline image", new Text("This is the first part using text objects with inline image {0}.", inlineImage)));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new Chapter("Text Chapter", new Text("Text Chapter with {0} image.", inlineImage)));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(figure);
         _objectsToReport.Add(new Section("Text Section", new Text("Text Section with {0} image.", inlineImage)));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new SubSection("Text SubSection", new Text("Text SubSection with {0} image.", inlineImage)));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new SubSubSection("Text SubSubSection", new Text("Text SubSubSection with {0} image.", inlineImage)));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new Paragraph("Text Paragraph", new Text("Text Paragraph with {0} image.", inlineImage)));
         _objectsToReport.Add(someText);
         _objectsToReport.Add(new SubParagraph("Text SubParagraph", new Text("Text SubParagraph with {0} image.", inlineImage)));
         _objectsToReport.Add(someText);
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}
