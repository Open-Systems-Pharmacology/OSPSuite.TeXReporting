using System;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_report_for_a_TableWithTextSpecs : ContextForReporting
   {
      private Section _refSection;

      protected override void Context()
      {
         base.Context();
         _pdfFileName = "TableWithText";

         _refSection = new Section("Reference Section");

         var table1 = new SimpleTable(CreateLongTestTable().DefaultView);
         _objectsToReport.Add(new Chapter("Test 1"));
         _objectsToReport.Add(table1);
         _objectsToReport.Add(new TextBox("Simple Table With Text", new Text("This is a simple table with text:{0}{1}", new LineBreak(), table1)));

         var table2 = new Table(CreateTestTable().DefaultView, "Table");
         _objectsToReport.Add(new Chapter("Test 2"));
         _objectsToReport.Add(table2);
         _objectsToReport.Add(new TextBox("Table With Text", new Text("This is a table with text:{0}{1}", new LineBreak(), table2)));

         _objectsToReport.Add(_refSection);
         _objectsToReport.Add("This is a dummy section with some text which is tested for refencing within tables.");
      }

      private DataTable CreateLongTestTable()
      {
         var dataTable = new DataTable();
         dataTable.Columns.Add("Name", typeof(string));
         dataTable.Columns.Add("Value", typeof(double));
         dataTable.Columns.Add("Text", typeof(Text));

         dataTable.BeginLoadData();
         for (int i = 0; i < 20; i++)
         {
            var row = dataTable.NewRow();
            row["Name"] = "Value {0}".FormatWith(i);
            row["Value"] = i;
            row["Text"] = new Text(row["Name"].ToString());
            dataTable.Rows.Add(row);
         }
         dataTable.Rows.Add(new object[]
                               {
                                  "Reference to section", null,
                                  new Text("see {0}", new Reference(_refSection))
                                     {
                                        Converter =
                                           NoConverter.Instance
                                     }
                               });
         dataTable.EndLoadData();

         return dataTable;
      }

      private DataTable CreateTestTable()
      {
         var dataTable = new DataTable();
         dataTable.Columns.Add("Column1", typeof(string));
         dataTable.Columns.Add("Column2", typeof(string));
         dataTable.Columns.Add("Text", typeof(Text));

         dataTable.BeginLoadData();
         var row = dataTable.NewRow();
         row["Column1"] = "A string";
         row["Column2"] = "Some text with a lot of words.";
         row["Text"] = new Text("This is just a test of a simple string.");
         dataTable.Rows.Add(row);
         dataTable.EndLoadData();

         dataTable.Rows.Add(new object[]
                               {
                                  "Test", DBNull.Value,
                                  new Text("please take a look at {0}", new Reference(_refSection))
                                     {
                                        Converter =
                                           NoConverter.Instance
                                     }
                               });
         return dataTable;
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}	
