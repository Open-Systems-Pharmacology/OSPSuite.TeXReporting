using System.Data;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Builder
{
   public class DataTableTeXBuilder : TeXBuilder<DataTable>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public DataTableTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(DataTable dataTable, BuildTracker tracker)
      {
         _builderRepository.Report(new Table(dataTable.DefaultView, new Text(dataTable.TableName)), tracker);
      }
   }
}