using System;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_report_for_a_long_table : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "LongTable";
         var dataTable = new DataTable();
         dataTable.Columns.Add("Name", typeof (string));
         dataTable.Columns.Add("Value", typeof (double));
         dataTable.Columns.Add("Text", typeof(string));

         for (int i = 0; i < 100; i++)
         {
            var row = dataTable.NewRow();
            row["Name"] = "Value {0}".FormatWith(i);
            row["Value"] = i;
            row["Text"] = "This is a longer text which could make trouble if it occurs in a table, because if there is a longer text this text might need to get broken into multiple lines.";
            dataTable.Rows.Add(row);
         }

         for (int i = 0; i < 10; i++)
         {
            var row = dataTable.NewRow();
            row["Name"] = "Value 1e-{0}".FormatWith(i);
            row["Value"] = Math.Pow(10, (-1) * i);
            row["Text"] = "This is a longer text which could make trouble if it occurs in a table, because if there is a longer text this text might need to get broken into multiple lines.";
            dataTable.Rows.Add(row);
         }

         _objectsToReport.Add(new Chapter("Tests"));

         var longTable = new Table(data: dataTable.DefaultView,
                                   caption: "My Title");

         _objectsToReport.Add(longTable);
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}