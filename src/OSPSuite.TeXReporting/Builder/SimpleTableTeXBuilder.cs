using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class SimpleTableTeXBuilder : TeXChunkBuilder<SimpleTable>
   {

      private readonly ITeXBuilderRepository _builderRepository;

      public SimpleTableTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override string TeXChunk(SimpleTable simpleTable)
      {
         return TableWriter.SimpleTable(simpleTable.Data, simpleTable.Converter, _builderRepository);
      }
   }
}