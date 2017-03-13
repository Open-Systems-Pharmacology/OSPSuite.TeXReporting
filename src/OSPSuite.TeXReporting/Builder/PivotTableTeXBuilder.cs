using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.Utility.Data;

namespace OSPSuite.TeXReporting.Builder
{
   internal class PivotTableTeXBuilder : TeXChunkBuilder<PivotTable>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public PivotTableTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(PivotTable table, BuildTracker tracker)
      {
         tracker.Track(table.Caption.Items);
         tracker.Track(table.Caption);

         base.Build(table, tracker);
      }

      public override string TeXChunk(PivotTable table)
      {
         var captionText = _builderRepository.ChunkFor(table.Caption);
         var pivoter = new Pivoter();
         var pivotData = pivoter.PivotData(table.Data, table.PivotInfo);

         return (TableWriter.PivotTable(captionText, table.Label, pivotData.DefaultView, table.Data.Table, table.PivotInfo, table.TexTranslations, table.Converter, _builderRepository));
      }

   }
}