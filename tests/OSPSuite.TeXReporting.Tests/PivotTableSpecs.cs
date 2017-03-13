using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Data;
using OSPSuite.TeXReporting.Items;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Data;
using OSPSuite.Utility.Format;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_report_for_a_pivot_table : ContextForReporting
   {
      private double percentile(IEnumerable<double> values, double percentile)
      {
         // taken without any proof, just for the testing
         var elements = values.ToArray();
         Array.Sort(elements);
         var realIndex = percentile / 100 * (elements.Length - 1);
         var index = (int)realIndex;
         var frac = realIndex - index;
         if (index + 1 < elements.Length)
            return elements[index] * (1 - frac) + elements[index + 1] * frac;
         else
            return elements[index];
      }

      private double standardDeviation(IEnumerable<double> values)
      {
         // taken without any proof, just for the testing
         values = values.ToArray();
         var average = values.Average();
         var sumOfSquaresOfDifferences = values.Select(val => (val - average) * (val - average)).Sum();
         return Math.Sqrt(sumOfSquaresOfDifferences / values.Count()); 
      }

      protected override void Context()
      {
         base.Context();
         _pdfFileName = "Pivot Table";
         NumericFormatterOptions.Instance.DecimalPlace = 5;

         var dataTable = new DataTable();
         var category = dataTable.Columns.Add("Category", typeof(string));
         var category2 = dataTable.Columns.Add("Category2", typeof(string));
         var subgroup = dataTable.Columns.Add("Subgroup", typeof(string));
         var dataValue = dataTable.Columns.Add("Value", typeof(double));
         var keyfigure = dataTable.Columns.Add("KeyFigure", typeof(string));
         var notes = new[] {"The values can be used for grouping the data."};
         category.SetNotes(notes);
         category2.SetNotes(notes);
         keyfigure.SetNotes(notes);
         dataValue.SetUnit("µg/l");
         dataValue.SetNotes(new[] { "This data is used for aggregation.", "Aggregation could be min, max, average, sum, exists, last, first." });

         for (var i = 0; i < 100; i++)
         {
            var row = dataTable.NewRow();
            row[category] = (i < 60) ? "A" : "B";
            row[category2] = (i < 30) ? "C" : "D";
            row[subgroup] = (i % 2 == 0) ? "1" : "2";
            row[dataValue] = (i^2)/Math.Sqrt(i);
            row[keyfigure] = (i % 2 == 0) ? "Figure 1" : "Figure 2";
            dataTable.Rows.Add(row);
         }

         _objectsToReport.Add(new Chapter("Tests"));
         _objectsToReport.Add(new Text("This chapter contains a large table with raw data and some smaller tables with pivoted data. The small tables illustrate the power of pivoting data."));
         _objectsToReport.Add(new Text("This is another indention with more text."));

         var longTable = new Table(data: dataTable.DefaultView,
                                         caption: "These are the raw data.");

         _objectsToReport.Add(new Section("Raw Data"));
         _objectsToReport.Add(longTable);

         var sumAggregation = new Aggregate<double, double> { Aggregation = x => x.Sum(), Name = "Sum" };
         var existsAggregation = new Aggregate<double, bool> { Aggregation = x => x.Any(), Name = "Exists" };
         var minAggregation = new Aggregate<double, double> { Aggregation = x => x.Min(), Name = "Min" };
         var maxAggregation = new Aggregate<double, double> { Aggregation = x => x.Max(), Name = "Max" };
         var averageAggregation = new Aggregate<double, double> { Aggregation = x => x.Average(), Name = "Average" };
         var firstAggregation = new Aggregate<double, double> { Aggregation = x => x.First(), Name = "First" };
         var lastAggregation = new Aggregate<double, double> { Aggregation = x => x.Last(), Name = "Last" };
         var percentile25Aggregation = new Aggregate<double, double> { Aggregation = x => percentile(x, 25), Name = "Percentile25" };
         var percentile75Aggregation = new Aggregate<double, double> { Aggregation = x => percentile(x, 75), Name = "Percentile75" };
         var medianAggregation = new Aggregate<double, double> { Aggregation = x => percentile(x, 50), Name = "Median" };
         var standardDeviationAggregation = new Aggregate<double, double> { Aggregation = x => standardDeviation(x), Name = "StandardDeviation" };

         var pivotTable = new PivotTable(data: dataTable.DefaultView,
                                         pivotInfo: new PivotInfo(
                                            rowFields: new[] {keyfigure.ColumnName},
                                            columnFields: new[] {category.ColumnName, category2.ColumnName, subgroup.ColumnName},
                                            dataFields: new [] {dataValue.ColumnName},
                                            aggregates: new List<Aggregate>
                                                           {
                                                              sumAggregation,
                                                              existsAggregation
                                                           }),
                                         texTranslations: new Cache<Aggregate, string>(),
                                         caption:
                                            new Text(
                                            "The data of {0} pivoted with aggregation functions sum, exists. keyfigure x category, category2, subgroup.",
                                            new Reference(longTable)));


         var pivotTable2 = new PivotTable(data: dataTable.DefaultView,
                                          pivotInfo: new PivotInfo(
                                             rowFields: new[] {category.ColumnName, category2.ColumnName, subgroup.ColumnName},
                                             dataFields: new [] {dataValue.ColumnName},
                                             aggregates: new List<Aggregate>
                                                            {
                                                               minAggregation,
                                                               maxAggregation,
                                                               averageAggregation
                                                            },
                                             columnFields: new[] {keyfigure.ColumnName}),
                                          texTranslations: new Cache<Aggregate, string>(),
                                          caption:
                                             "This is pivoted with aggregation functions min, max, average. category, category2, subgroup x keyfigure.");

         _objectsToReport.Add(new Section("In Landscape"));
         _objectsToReport.Add(new InLandscape(new object[] {pivotTable,pivotTable2}));

         var pivotTable3 = new PivotTable(data: dataTable.DefaultView,
                                          pivotInfo:
                                             new PivotInfo(rowFields: new[] {category.ColumnName, category2.ColumnName},
                                                           dataFields: new [] {dataValue.ColumnName},
                                                           aggregates: new List<Aggregate>()
                                                                          {
                                                                             firstAggregation,
                                                                             lastAggregation
                                                                          },
                                                           columnFields: new[] {keyfigure.ColumnName, subgroup.ColumnName}),
                                          texTranslations: new Cache<Aggregate, string>(),
                                          caption:
                                             "This is pivoted with aggregation functions first and last. category, category2 x keyfigure, subgroup.");

         _objectsToReport.Add(new Section("Multiple Column Fields"));
         _objectsToReport.Add(pivotTable3);

         var pivotTable4 = new PivotTable(data: dataTable.DefaultView,
                                          pivotInfo:
                                             new PivotInfo(rowFields: new[] {category.ColumnName, category2.ColumnName},
                                                           dataFields: new []{dataValue.ColumnName},
                                                           aggregates: new List<Aggregate>
                                                                          {
                                                                             percentile25Aggregation,
                                                                             percentile75Aggregation,
                                                                          },
                                                           columnFields: new[] {keyfigure.ColumnName, subgroup.ColumnName}),
                                          texTranslations: new Cache<Aggregate, string>()
                                                             {
                                                                {percentile25Aggregation, "$25^{th}$ Percentile"},
                                                                {percentile75Aggregation, "$75^{th}$ Percentile"}
                                                             },
                                          caption:
                                             "This is pivoted with aggregation functions 25th and 75th percentile. category, category2 x keyfigure, subgroup.");

         _objectsToReport.Add(new Section("Aggregations With Translations"));
         _objectsToReport.Add(pivotTable4);

         var pivotTable5 = new PivotTable(data: dataTable.DefaultView,
                                          pivotInfo:
                                             new PivotInfo(rowFields: new[] {category.ColumnName, category2.ColumnName},
                                                           dataFields: new []{dataValue.ColumnName},
                                                           aggregates: new List<Aggregate>
                                                                          {
                                                                             standardDeviationAggregation,
                                                                             medianAggregation
                                                                          },
                                                           columnFields: new[] {keyfigure.ColumnName, subgroup.ColumnName}),
                                          texTranslations: new Cache<Aggregate, string> {{standardDeviationAggregation, "$\\sigma$"}},
                                          caption:
                                             "This is pivoted with aggregation functions standard deviation and median. category, category2 x keyfigure, subgroup.");

         _objectsToReport.Add(new Section("Aggregations With Partly Translations"));
         _objectsToReport.Add(pivotTable5);

         var dataValue2 = dataTable.Columns.Add("Value2", typeof (double), string.Format("[{0}]*[{0}]", dataValue.ColumnName));
         dataValue2.SetNotes(new [] {String.Format("Expression: {0}", dataValue2.Expression)});
         var pivotTable6 = new PivotTable(data: dataTable.DefaultView,
                                 pivotInfo:
                                    new PivotInfo(rowFields: new[] { category.ColumnName, category2.ColumnName },
                                                  dataFields: new[] { dataValue.ColumnName, dataValue2.ColumnName },
                                                  aggregates: new List<Aggregate>
                                                                          {
                                                                             standardDeviationAggregation
                                                                          },
                                                  columnFields: new[] { keyfigure.ColumnName, subgroup.ColumnName }),
                                 texTranslations: new Cache<Aggregate, string> { { standardDeviationAggregation, "$\\sigma$" } },
                                 caption:
                                    "An expression column has been added and the standard deviation has been calculated for both value fields. Pivotation over category, category2 x keyfigure, subgroup.");

         _objectsToReport.Add(new Section("Multiple Data Fields"));
         _objectsToReport.Add(pivotTable6);
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}