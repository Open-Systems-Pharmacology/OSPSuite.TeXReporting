﻿using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_pdf_report_for_a_draft_report : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "DraftReport";
         _settings.Draft = true;
         _objectsToReport.AddRange(new object[]
            {
               new Part("Part1"),
               new Chapter("Chapter1.1"),
               new Chapter("Chapter1.2"),
               new Part("Part2"),
               new Chapter("Chapter2.1"),
               new Section("First Section"),
               new SubSection("First Subsection"),
               new SubSubSection("First SubSubSection"),
               new Paragraph("First Paragraph"),
               new SubParagraph("First SubParagraph"),
               new Paragraph("Second Paragraph"),
               new SubSection("Second Subsection"),
               new Chapter("Chapter2.2"),
               new Chapter("Chapter2.3") {CreateTableOfContentsEntry = false},
               new Chapter("Chapter2.4") {TableOfContentsTitle = "Amazing 2.4"},
            });
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}