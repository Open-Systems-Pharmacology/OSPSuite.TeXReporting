using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Events;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting
{
   public interface IReportCreator
   {
      /// <summary>
      ///    Reports the objects given to the pdf file using the <paramref name="settings" />
      /// </summary>
      /// <param name="pdfReportFullPath">Full pdf file path where the report will be generated</param>
      /// <param name="settings">Settings used to generate the report</param>
      /// <param name="objectsToReport">objects to report</param>
      /// <returns>A task that will create the report to pdf file</returns>
      Task ReportToPDF(string pdfReportFullPath, ReportSettings settings, IReadOnlyCollection<object> objectsToReport);

      /// <summary>
      ///    Reports the objects given to the pdf file using the <paramref name="settings" />
      /// </summary>
      /// <param name="pdfReportFullPath">Full pdf file path where the report will be generated</param>
      /// <param name="settings">Settings used to generate the report</param>
      /// <param name="objectsToReport">objects to report</param>
      /// <returns>A task that will create the report to pdf file</returns>
      Task ReportToPDF(string pdfReportFullPath, ReportSettings settings, params object[] objectsToReport);

      /// <summary>
      ///    Reports the objects using the the <paramref name="tracker" /> and the <paramref name="settings" />.
      ///    This is especially usefull when referenced need to be passed along between builder
      /// </summary>
      /// <param name="tracker">Tracker containing the required info to build the report</param>
      /// <param name="settings">Settings used to generate the report</param>
      /// <param name="objectsToReport">objects to report</param>
      /// <returns>A task that will create the report to pdf file</returns>
      Task ReportToPDF<T>(T tracker, ReportSettings settings, IReadOnlyCollection<object> objectsToReport) where T : BuildTracker;
   }

   internal class ReportCreator : IReportCreator
   {
      private readonly ITeXBuilderRepository _builderRepository;
      private readonly IReportCompiler _reportCompiler;
      private readonly IBuildTrackerFactory _buildTrackerFactory;
      private readonly IArtifactsManager _artifactsManager;
      private readonly IEventPublisher _eventPublisher;

      public ReportCreator(ITeXBuilderRepository builderRepository, IReportCompiler reportCompiler, IBuildTrackerFactory buildTrackerFactory,
         IArtifactsManager artifactsManager, IEventPublisher eventPublisher)
      {
         _builderRepository = builderRepository;
         _reportCompiler = reportCompiler;
         _buildTrackerFactory = buildTrackerFactory;
         _artifactsManager = artifactsManager;
         _eventPublisher = eventPublisher;
      }

      public async Task ReportToPDF<T>(T tracker, ReportSettings settings, IReadOnlyCollection<object> objectsToReport) where T : BuildTracker
      {
         settings.Validate();

         try
         {
            _eventPublisher.PublishEvent(new ReportCreationStartedEvent(tracker.ReportFullPath));

            objectsToReport.Each(item => _builderRepository.Report(item, tracker));

            var allAttachements = tracker.TrackedObjects.OfType<Attachement>();

            await _reportCompiler.CompileReport(tracker.TeX, allAttachements, settings, tracker);

            if (settings.SaveArtifacts)
               await _artifactsManager.SaveArtifacts(tracker);

            //delete the temporary working directory
            if (settings.DeleteWorkingDir)
               tracker.DeleteWorkingDirectory();
         }
         finally
         {
            _eventPublisher.PublishEvent(new ReportCreationFinishedEvent(tracker.ReportFullPath));
         }
      }

      public Task ReportToPDF(string pdfReportFullPath, ReportSettings settings, IReadOnlyCollection<object> objectsToReport)
      {
         var tracker = _buildTrackerFactory.CreateFor<BuildTracker>(pdfReportFullPath);
         return ReportToPDF(tracker, settings, objectsToReport);
      }

      public Task ReportToPDF(string pdfReportFullPath, ReportSettings settings, params object[] objectsToReport)
      {
         return ReportToPDF(pdfReportFullPath, settings, new ReadOnlyCollection<object>(objectsToReport));
      }
   }
}