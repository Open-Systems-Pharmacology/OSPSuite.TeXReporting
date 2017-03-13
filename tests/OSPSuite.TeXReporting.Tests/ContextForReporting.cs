using System;
using System.Collections.Generic;
using System.IO;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;

namespace OSPSuite.TeXReporting.Tests
{
   [IntegrationTests]
   public abstract class ContextForReporting : ContextSpecification<IReportCreator>
   {
      protected ReportSettings _settings;
      protected string _outputDir;
      protected string _pdfFileName;
      protected string _pdfFile;
      protected List<object> _objectsToReport;

      public override void GlobalContext()
      {
         base.GlobalContext();
         IoC.InitializeWith(new CastleWindsorContainer());
         IoC.RegisterImplementationOf(IoC.Container);
         IoC.RegisterImplementationOf(A.Fake<IEventPublisher>());
         IoC.Container.AddRegister(x => x.FromType<ReportingRegister>());
         sut = IoC.Resolve<IReportCreator>();
         _objectsToReport = new List<object>();

         _outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");
         if (!Directory.Exists(_outputDir))
            Directory.CreateDirectory(_outputDir);

         _settings = new ReportSettings
         {
            Author = "Unit Tests Engine",
            Keywords = new[] {"Tests", "PKReporting", "OSPSuite"},
            Software = "OSPSuite",
            SoftwareVersion = "5.2",
            SubTitle = "SubTitle",
            TemplateFolder = ConstantsForSpecs.StandardTemplateFolder,
            ContentFileName = "Content",
            DeleteWorkingDir = true
         };
      }

      protected override void Because()
      {
         _settings.Title = _pdfFileName;
         _pdfFile = Path.Combine(_outputDir, string.Concat(_pdfFileName, ".pdf"));
      }

      protected void VerifyThatFileExists()
      {
         sut.ReportToPDF(_pdfFile, _settings, _objectsToReport).Wait();
         File.Exists(_pdfFile).ShouldBeTrue();
      }
   }
}