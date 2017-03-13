using System;
using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.Utility;

namespace OSPSuite.TeXReporting.Tests
{
   public abstract class concern_for_ArtifactsManager : ContextSpecification<IArtifactsManager>
   {
      private BuildTracker _tracker;
      protected string _outputDir;
      protected string _artifact1;
      private IImageConverter _imageConverter;

      protected override void Context()
      {
         var factory = new BuildTrackerFactory();
         _outputDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults", "Artifacts");
         var workDir = Path.Combine(_outputDir, "WORK");
         var plotDir = Path.Combine(workDir, Constants.ArtifactFolderForPlots);
         if (Directory.Exists(plotDir))
            Directory.Delete(plotDir, true);

         Directory.CreateDirectory(plotDir);
         _artifact1 = Path.Combine(plotDir, "oneArtifact.pdf");
         using (var writer = new StreamWriter(_artifact1))
         {
            writer.Write("BLAH");
         }

         var pdfReportFullPath = Path.Combine(_outputDir, string.Concat("Report", ".pdf"));
         _tracker = factory.CreateFor<BuildTracker>(pdfReportFullPath, "WORK");

         _imageConverter = new ImageConverter();
         sut = new ArtifactsManager(_imageConverter);
      }

      protected override void Because()
      {
         sut.SaveArtifacts(_tracker);
      }
   }

   public class When_the_artifacts_manager_is_copying_all_the_artifacts : concern_for_ArtifactsManager
   {
      [Observation]
      public void should_have_created_a_folder_named_after_the_report()
      {
         var folder = Path.Combine(_outputDir, "Report_" + Constants.ArtifactOutputFolder, Constants.ArtifactFolderForPlots);
         Directory.Exists(folder).ShouldBeTrue();

         var file = Path.Combine(folder, new FileInfo(_artifact1).Name);
         FileHelper.FileExists(file).ShouldBeTrue();
      }
   }
}