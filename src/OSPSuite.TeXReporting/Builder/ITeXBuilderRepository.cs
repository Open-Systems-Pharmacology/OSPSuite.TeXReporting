using System.Collections.Generic;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Builder
{
   public interface ITeXBuilderRepository : IBuilderRepository<ITeXBuilder>
   {
      /// <summary>
      ///    Retrieves a builder for the <paramref name="objectToReport" />.
      ///    If the builder is not available, throws an <exception cref="BuilderNotFoundException" />. Otherwise
      ///    the build method will be called
      /// </summary>
      /// <param name="objectToReport">Object that should be reported</param>
      /// <param name="tracker">Build tracker for the give report process</param>
      /// <exception cref="BuilderNotFoundException"> is thrown if no builder for the given object was found</exception>
      void Report(object objectToReport, BuildTracker tracker);

      /// <summary>
      /// Returns a chunk builder defined fro the given <paramref name="objectToReport" />.
      /// </summary>
      /// <param name="objectToReport">Object for which a ChunkBuilder should be found</param>
      /// <exception cref="ChunkBuilderNotFoundException"> is thrown if no chunk for the given object was found</exception>
      ITeXChunkBuilder ChunkBuilderFor(object objectToReport);

      string ChunkFor(object objectToReport);

      void Report(IEnumerable<object> objectsToReport, BuildTracker tracker);
   }

   internal class TeXBuilderRepository : BuilderRepository<ITeXBuilder>, ITeXBuilderRepository
   {
      public TeXBuilderRepository(IContainer container) : base(container, typeof (ITeXBuilder<>))
      {
      }

      public void Report(object objectToReport, BuildTracker tracker)
      {
         if (objectToReport == null) return;

         var builder = BuilderFor(objectToReport);
         if (builder == null)
            throw new BuilderNotFoundException(objectToReport);

         builder.Build(objectToReport, tracker);
         tracker.Track(objectToReport);
      }

      public ITeXChunkBuilder ChunkBuilderFor(object objectToReport)
      {
         return BuilderFor(objectToReport) as ITeXChunkBuilder;
      }

      public string ChunkFor(object objectToReport)
      {
         var builder = ChunkBuilderFor(objectToReport);
         if (builder == null)
            throw new ChunkBuilderNotFoundException(objectToReport);

         return builder.TeXChunk(objectToReport);
      }

      public void Report(IEnumerable<object> objectsToReport, BuildTracker tracker)
      {
         objectsToReport.Each(o => Report(o, tracker));
      }
   }
}