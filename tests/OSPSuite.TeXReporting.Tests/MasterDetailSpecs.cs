using System;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Data;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_report_for_a_master_detail_table : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "MasterDetail";

         _objectsToReport.Add(new Chapter("Tests"));

         _objectsToReport.Add(new Section("Master Detail Table"));
         _objectsToReport.Add(CreateMasterDetailTable());

         _objectsToReport.Add(new Section("Master Detail Detail Table"));
         _objectsToReport.Add(CreateMasterDetailDetailTable());

      }

      private DataTable CreateMasterDetailTable()
      {
         var masterTable = new DataTable("Master Detail Table");
         var idColumn = masterTable.Columns.Add("ID", typeof(int));
         idColumn.SetHidden(true);
         masterTable.Columns.Add("Name", typeof(string));
         masterTable.Columns.Add("Value", typeof(double));
         masterTable.Columns.Add("Category", typeof(string));

         var detailTable = new DataTable("DetailTable");
         detailTable.Columns.Add("ID", typeof(int));
         detailTable.Columns.Add("Text", typeof(string));

         for (int i = 0; i < 100; i++)
         {
            var masterRow = masterTable.NewRow();
            masterRow["ID"] = i;
            masterRow["Name"] = $"Value {i}";
            masterRow["Value"] = i / (2 + i);
            masterRow["Category"] = (i % 2 == 0) ? "Category A" : "Category B";
            masterTable.Rows.Add(masterRow);

            var detailRow = detailTable.NewRow();
            detailRow["ID"] = i;
            detailRow["Text"] = "This is a longer text which could make trouble if it occurs in a table, because if there is a longer text this text might need to get broken into multiple lines.";
            detailTable.Rows.Add(detailRow);
         }
         var dataSet = new DataSet("DataSet");
         dataSet.Tables.Add(masterTable);
         dataSet.Tables.Add(detailTable);

         dataSet.Relations.Add(masterTable.Columns["ID"], detailTable.Columns["ID"]);
         return masterTable;
      }

      private DataTable CreateMasterDetailDetailTable()
      {
         var masterTable = new DataTable("Master Detail Detail Table");
         var masterPK1 = masterTable.Columns.Add("ID", typeof(int));
         masterPK1.SetHidden(true);
         masterTable.Columns.Add("Start Time", typeof(string));
         masterTable.Columns.Add("Number of Repetitions", typeof(double));
         masterTable.Columns.Add("Time Between Repetitions", typeof(string));

         var detailTable = new DataTable("DetailTable");
         var detailPK1 = detailTable.Columns.Add("ID", typeof(int));
         detailTable.Columns.Add("Start Time", typeof(string));
         detailTable.Columns.Add("Dose", typeof(string));
         var col = detailTable.Columns.Add("Administration type", typeof(string));
         col.SetNotes(new [] {"This is a footnote for Administration type"});
         detailTable.Columns.Add("Placeholder for formulation", typeof(string));

         var detailDetailTable = new DataTable("DetailDetailTable");
         var detailDetailPK1 = detailDetailTable.Columns.Add("ID", typeof(int));
         detailDetailTable.Columns.Add("Parameter", typeof(string));
         detailDetailTable.Columns.Add("Value", typeof(string));

         //fill data
         masterTable.BeginLoadData();
         detailTable.BeginLoadData();
         detailDetailTable.BeginLoadData();
         var newMasterRow = masterTable.NewRow();
         newMasterRow.ItemArray = new object[] {1, "0 h", 1.0, "0 h"};
         masterTable.Rows.Add(newMasterRow);
         var newDetailRow = detailTable.NewRow();
         newDetailRow.ItemArray = new object[] {1, "0 h", "1.00 mg/kg", "Intravenous Bolus", DBNull.Value};
         detailTable.Rows.Add(newDetailRow);
         newMasterRow = masterTable.NewRow();
         newMasterRow.ItemArray = new object[] {2, "2.00 h", 5.0, "10 h" };
         masterTable.Rows.Add(newMasterRow);
         newDetailRow = detailTable.NewRow();
         newDetailRow.ItemArray = new object[] {2, "0 h", "1.00 mg/kg", "Oral", "Formulation" };
         detailTable.Rows.Add(newDetailRow);
         var newDetailDetailRow = detailDetailTable.NewRow();
         newDetailDetailRow.ItemArray = new object[] {2, "Volume of water/body weight", "22.00 ml/kg"};
         detailDetailTable.Rows.Add(newDetailDetailRow);

         detailDetailTable.EndLoadData();
         detailTable.EndLoadData();
         masterTable.EndLoadData();

         var dataSet = new DataSet("DataSet");
         dataSet.Tables.Add(masterTable);
         dataSet.Tables.Add(detailTable);
         dataSet.Tables.Add(detailDetailTable);

         dataSet.Relations.Add(masterPK1, detailPK1);
         dataSet.Relations.Add(detailPK1, detailDetailPK1);
         return masterTable;
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}