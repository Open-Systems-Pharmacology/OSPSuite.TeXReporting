using System;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Builder
{
   /// <summary>
   ///    Represents a builder capable of building an object into its TEX representation.
   /// </summary>
   public interface ITeXBuilder : ISpecification<Type>
   {
      /// <summary>
      ///    Builds the <paramref name="objectToReport" />
      ///    Each object actually build by the builder should be added to the <paramref name="tracker" />
      ///    <para>
      ///       For example, an InLandscape object should add all its items in the tracker.
      ///    </para>
      /// </summary>
      void Build(object objectToReport, BuildTracker tracker);
   }

   public interface ITeXBuilder<TObject> : ITeXBuilder
   {
      void Build(TObject objectToReport, BuildTracker tracker);
   }

   public interface ITeXBuilder<TObject, TTracker> : ITeXBuilder<TObject> where TTracker : BuildTracker
   {
      void Build(TObject objectToReport, TTracker buildTracker);
   }

   public abstract class TeXBuilder<TObject> : ITeXBuilder<TObject>
   {
      public virtual void Build(object objectToReport, BuildTracker tracker)
      {
         Build(objectToReport.DowncastTo<TObject>(), tracker);
      }

      public abstract void Build(TObject objectToReport, BuildTracker tracker);

      public virtual bool IsSatisfiedBy(Type type)
      {
         return type.IsAnImplementationOf<TObject>();
      }
   }

   public abstract class TeXBuilder<TObject, TTracker> : TeXBuilder<TObject>, ITeXBuilder<TObject, TTracker> where TTracker : BuildTracker
   {
      public override void Build(TObject objectToReport, BuildTracker tracker)
      {
         Build(objectToReport, tracker.DowncastTo<TTracker>());
      }

      public abstract void Build(TObject objectToReport, TTracker buildTracker);
   }
}