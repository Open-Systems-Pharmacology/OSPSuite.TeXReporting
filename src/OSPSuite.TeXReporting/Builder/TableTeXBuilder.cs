using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class TableTeXBuilder : TeXChunkBuilder<Table>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public TableTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(Table table, BuildTracker tracker)
      {
         tracker.Track(table.Caption.Items);
         tracker.Track(table.Caption);

         base.Build(table,tracker);
      }

      public override string TeXChunk(Table table)
      {
         var captionText = _builderRepository.ChunkFor(table.Caption);

         return(TableWriter.Table(captionText, table.Label, table.Data, table.Converter, _builderRepository));
      }
   }
}