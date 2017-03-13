using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Container;

namespace OSPSuite.TeXReporting.Tests
{
   public abstract class concern_for_TEXBuilderRepository : ContextSpecification<ITeXBuilderRepository>
   {
      private IContainer _container;
      protected List<ITeXBuilder> _allTEXBuilder;

      protected override void Context()
      {
         _allTEXBuilder = new List<ITeXBuilder>();
         _container = A.Fake<IContainer>();
         A.CallTo(() => _container.ResolveAll<ITeXBuilder>()).Returns(_allTEXBuilder);
         sut = new TeXBuilderRepository(_container);
      }
   }

   public class When_resolving_a_builder_for_a_type_that_was_not_registered : concern_for_TEXBuilderRepository
   {
      [Observation]
      public void should_return_null()
      {
         sut.BuilderFor(new Parameter()).ShouldBeNull();
      }
   }

   public class When_resolving_a_builder_for_a_type_that_was_registered : concern_for_TEXBuilderRepository
   {
      protected override void Context()
      {
         base.Context();
         _allTEXBuilder.Add(new StructureElementTeXBuilder(sut));
      }

      [Observation]
      public void should_return_null()
      {
         sut.BuilderFor(new Chapter("Chap")).ShouldBeAnInstanceOf<StructureElementTeXBuilder>();
      }
   }

   public class When_resolving_a_builder_for_a_type_that_could_use_many_possible_builder : concern_for_TEXBuilderRepository
   {
      protected override void Context()
      {
         base.Context();
         _allTEXBuilder.Add(new ParameterBuilder());
         _allTEXBuilder.Add(new TeXMySuperBuilder());
      }

      [Observation]
      public void should_use_the_most_accurate_builder()
      {
         sut.BuilderFor(new MySuperParameter()).ShouldBeAnInstanceOf<TeXMySuperBuilder>();
      }
   }

   public class When_reporting_an_object_for_which_a_report_was_registered : concern_for_TEXBuilderRepository
   {
      private Parameter _parameter;
      private MyBuildTracker _tracker;
      private ParameterBuilder _parameterBuilder;

      protected override void Context()
      {
         base.Context();
         _parameter = new Parameter();
         _tracker = new MyBuildTracker();
         _parameterBuilder = new ParameterBuilder();
         _allTEXBuilder.Add(_parameterBuilder);
         sut.Start();
      }

      protected override void Because()
      {
         sut.Report(_parameter, _tracker);
      }

      [Observation]
      public void should_call_the_build_method_on_the_expected_builder()
      {
         _parameterBuilder.Parameter.ShouldBeEqualTo(_parameter);
      }

      [Observation]
      public void should_add_the_reported_object_to_the_tracker()
      {
         _tracker.TrackedObjects.ShouldContain(_parameter);
      }
   }

   public class When_reporting_an_object_for_which_a_report_was_not_registered : concern_for_TEXBuilderRepository
   {
      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.Report(new Parameter(), A.Fake<BuildTracker>())).ShouldThrowAn<BuilderNotFoundException>();
      }
   }

   internal class Parameter
   {
   }

   internal class MySuperParameter : Parameter
   {
   }

   internal class TeXMySuperBuilder : TeXBuilder<MySuperParameter, MyBuildTracker>
   {
      public override void Build(MySuperParameter objectToReport, MyBuildTracker buildTracker)
      {
      }
   }

   internal class ParameterBuilder : TeXBuilder<Parameter, MyBuildTracker>
   {
      public override void Build(Parameter objectToReport, MyBuildTracker buildTracker)
      {
         Parameter = objectToReport;
      }

      public Parameter Parameter { get; set; }
   }

   internal class MyBuildTracker : BuildTracker
   {
   }
}