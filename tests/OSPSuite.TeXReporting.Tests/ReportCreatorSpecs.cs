using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Events;
using OSPSuite.Utility.Events;

namespace OSPSuite.TeXReporting.Tests
{
   public abstract class concern_for_Reporter : ContextSpecification<IReportCreator>
   {
      protected ITeXBuilderRepository _builderRepository;
      protected string _pdfReportFullPath;
      protected ReportSettings _reportSettings;
      protected IReportCompiler _reportCompiler;
      private IBuildTrackerFactory _buildTrackerFactory;
      protected BuildSettings _buildSettings;
      protected IArtifactsManager _artifactsManager;
      protected IEventPublisher _eventPublisher;

      protected override void Context()
      {
         _pdfReportFullPath = string.Empty;
         _reportSettings = A.Fake<ReportSettings>();
         _buildSettings = new BuildSettings();
         _builderRepository = A.Fake<ITeXBuilderRepository>();
         _reportCompiler = A.Fake<IReportCompiler>();
         _buildTrackerFactory = A.Fake<IBuildTrackerFactory>();
         _artifactsManager = A.Fake<IArtifactsManager>();
         _eventPublisher = A.Fake<IEventPublisher>();
         var task = Task.Run(() => { });
         A.CallTo(_reportCompiler).WithReturnType<Task>().Returns(task);
         sut = new ReportCreator(_builderRepository, _reportCompiler, _buildTrackerFactory, _artifactsManager, _eventPublisher);
      }
   }

   public class When_the_reported_is_asked_to_create_a_report_for_an_object_for_which_no_builder_was_found : concern_for_Reporter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(_builderRepository).Throws(new BuilderNotFoundException("TOTO"));
      }

      [Observation]
      public async Task should_throw_an_exception_containing_the_type_of_the_object_for_which_the_builder_could_not_be_found()
      {
         try
         {
            await sut.ReportToPDF(_pdfReportFullPath, _reportSettings, new object());
            false.ShouldBeTrue();
         }
         catch (BuilderNotFoundException)
         {
         }
      }
   }

   public class When_the_reported_is_asked_to_create_a_report_for_some_objects_for_which_a_builder_was_found : concern_for_Reporter
   {
      private Parameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = new Parameter();
      }

      protected override void Because()
      {
         sut.ReportToPDF(_pdfReportFullPath, _reportSettings, new[] {_parameter});
      }

      [Observation]
      public void should_validate_the_settings()
      {
         A.CallTo(() => _reportSettings.Validate()).MustHaveHappened();
      }

      [Observation]
      public void should_send_the_report_started_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ReportCreationStartedEvent>._)).MustHaveHappened();
      }

      [Observation]
      public void should_send_the_report_finished_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ReportCreationFinishedEvent>._)).MustHaveHappened();
      }

      [Observation]
      public void should_call_the_build_method_on_the_builder_for_the_given_object()
      {
         A.CallTo(() => _builderRepository.Report(_parameter, A<BuildTracker>._)).MustHaveHappened();
      }
   }

   public class When_the_reported_is_asked_to_create_a_report_and_save_the_artifacts : concern_for_Reporter
   {
      private Parameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = new Parameter();
         _reportSettings.SaveArtifacts = true;
      }

      protected override void Because()
      {
         sut.ReportToPDF(_pdfReportFullPath, _reportSettings, new[] {_parameter});
      }

      [Observation]
      public void should_export_the_artifacts()
      {
         A.CallTo(() => _artifactsManager.SaveArtifacts(A<BuildTracker>._)).MustHaveHappened();
      }
   }
}