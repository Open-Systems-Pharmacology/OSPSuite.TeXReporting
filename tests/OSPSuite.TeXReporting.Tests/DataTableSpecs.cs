using System.Data;
using NUnit.Framework;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_report_for_a_data_table : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "DataTable";
         var dataTable = new DataTable();
         dataTable.Columns.Add("Name", typeof (string));
         dataTable.Columns.Add("Value", typeof (double));

         for (int i = 0; i < 40; i++)
         {
            var row = dataTable.NewRow();
            row["Name"] = $"Value {i}";
            row["Value"] = i;
            dataTable.Rows.Add(row);
         }
         
         _objectsToReport.Add(dataTable);
      }

      [Test]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}