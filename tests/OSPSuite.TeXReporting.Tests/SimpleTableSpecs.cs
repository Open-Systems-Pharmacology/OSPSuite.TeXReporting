using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_report_for_a_simple_table : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "SimpleTable";

         var table1 = new SimpleTable(CreateLongTestTable().DefaultView);
         _objectsToReport.Add(new Chapter("Test 1"));
         _objectsToReport.Add(table1);
         _objectsToReport.Add(new TextBox("Simple Table", new Text("This is a simple table test table:{0}{1}", new LineBreak(), table1)));

         var table2 = new SimpleTable(CreateTestTable().DefaultView);
         _objectsToReport.Add(new Chapter("Test 2"));
         _objectsToReport.Add(table2);
         _objectsToReport.Add(new TextBox("Simple Table", new Text("This is a simple table test table:{0}{1}", new LineBreak(), table2)));
      }

      private DataTable CreateLongTestTable()
      {
         var dataTable = new DataTable();
         dataTable.Columns.Add("Name", typeof(string));
         dataTable.Columns.Add("Value", typeof(double));

         dataTable.BeginLoadData();
         for (int i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row["Name"] = "Value {0}".FormatWith(i);
            row["Value"] = i;
            dataTable.Rows.Add(row);
         }
         dataTable.EndLoadData();

         return dataTable;
      }

      private DataTable CreateTestTable()
      {
         var dataTable = new DataTable();
         dataTable.Columns.Add("Alias", typeof(string));
         dataTable.Columns.Add("Path", typeof(string));
         dataTable.Columns.Add("Dimension", typeof(string));
//         Alias Path Dimension
//MassTransferIntestineToMucosaBCRate NEIGHBORHOODjMOLECULEjMass transfer
//rate (intestine to mucosa blood cells)
//Rate
         dataTable.BeginLoadData();
         var row = dataTable.NewRow();
         row["Alias"] = "MassTransferIntestineToMucosaBCRate";
         row["Path"] = "NEIGHBORHOOD|MOLECULE|Mass transfer rate (intestine to mucosa blood cells)";
         row["Dimension"] = "Rate";
         dataTable.Rows.Add(row);
         dataTable.EndLoadData();

         return dataTable;
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}	