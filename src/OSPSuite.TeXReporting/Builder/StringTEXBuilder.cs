using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Builder
{
   internal class StringTeXBuilder : TeXChunkBuilder<string>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public StringTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(string stringToReport, BuildTracker tracker)
      {
         _builderRepository.Report(new Text(stringToReport), tracker);
      }

      public override string TeXChunk(string stringToReport)
      {
         return _builderRepository.ChunkFor(new Text(stringToReport));
      }
   }
}