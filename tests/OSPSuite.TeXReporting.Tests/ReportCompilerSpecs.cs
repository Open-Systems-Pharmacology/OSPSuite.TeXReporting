using System;
using System.IO;
using System.Linq;
using System.Text;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.Tests
{
   internal abstract class concern_for_ReportCompiler : ContextSpecification<ReportCompiler>
   {
      private ITeXCompiler _texCompiler;
      protected string _outputDir = String.Empty;
      protected ReportSettings _settings;
      protected DirectoryInfo _workingDir;

      protected override void Context()
      {
         _outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");

         if (!Directory.Exists(_outputDir))
            Directory.CreateDirectory(_outputDir);

         _workingDir = Directory.CreateDirectory(Path.Combine(_outputDir, Guid.NewGuid().ToString()));

         _texCompiler = new TeXCompiler();
         sut = new ReportCompiler(_texCompiler);

         _settings = new ReportSettings
         {
            Author = "Unit Tests Engine",
            Keywords = new[] {"Tests", "PKReporting", "SBSuite"},
            Software = "SBSuite",
            SoftwareVersion = "2.5",
            TemplateFolder = ConstantsForSpecs.StandardTemplateFolder,
            ContentFileName = "Content",
            DeleteWorkingDir = true
         };
      }

      public override void Cleanup()
      {
         _workingDir.Delete(true);
      }
   }

   internal class When_creating_a_simple_report_with_one_chapter : concern_for_ReportCompiler
   {
      private BuildTracker _tracker;

      [Observation]
      public void should_create_a_pdf_with_only_one_chapter()
      {
         var conv = DefaultConverter.Instance;
         _settings.Title = "Test Case";
         _settings.SubTitle = "Should_create_pdf_with_only_one_chapter";

         // create tex file for content
         var tex = new StringBuilder();
         tex.Append(Helper.CreateStructureElement(Helper.StructureElements.chapter, "Test Chapter 1", conv, true));
         tex.Append(Helper.CreateStructureElement(Helper.StructureElements.paragraph, "This is a little test chapter with no real content.", conv, true));

         var pdfFile = Path.Combine(_outputDir, "Should_create_pdf_with_only_one_chapter.pdf");
         _tracker = new BuildTracker
         {
            WorkingDirectory = _workingDir.FullName,
            ReportFullPath = pdfFile,
            ReportFileName = "Should_create_pdf_with_only_one_chapter",
         };

         //compile
         sut.CompileReport(tex, Enumerable.Empty<Attachement>(), _settings, _tracker).Wait();

         //check
         File.Exists(pdfFile).ShouldBeTrue();
      }
   }
}