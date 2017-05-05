using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.Data;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Data;
using OSPSuite.Utility.Format;

namespace OSPSuite.TeXReporting.TeX
{
   public static class TableWriter
   {
      private const char VALUE_SEPARATOR = '&';
      private const char VERTICAL_LINE = '|';
      private const double ARRAY_STRETCH = 1.2;

      /// <summary>
      /// Within a longtable we can adjust at the beginning of a row how much a pagebreak is desirable.
      /// This can be used to tweak page breaking.
      /// </summary>
      private enum PageBreakDesirability
      {
         VeryLow = 0,
         Low = 1,
         Medium = 2,
         High = 3,
         VeryHigh = 4,
      }

      private static readonly NumericFormatter<double> _numericFormatter = new NumericFormatter<double>(NumericFormatterOptions.Instance);

      /// <summary>
      /// This the factor for linewidth to specify dynamically the table width.
      /// </summary>
      private const double TABLE_WIDTH = 0.95;

      /// <summary>
      ///    These are alignments usable for column alignment specification.
      /// </summary>
      /// <remarks>
      ///    <see cref="TableWriter.FixedColumnWidth(double,OSPSuite.TeXReporting.TeX.Helper.MeasurementUnits)" /> as an alingment where the column width is fixed.
      /// </remarks>
      public enum ColumnAlignments
      {
         /// <summary>
         ///    Left alignment.
         /// </summary>
         l,

         /// <summary>
         ///    Center alignment.
         /// </summary>
         c,

         /// <summary>
         ///    Right alignment.
         /// </summary>
         r,

      }

      private const string X_COLUMN = "X";

      /// <summary>
      ///    This method specifies a fixed width column. This is a special column alignment.
      /// </summary>
      /// <param name="value">Value for width.</param>
      /// <param name="unit">Length unit.</param>
      /// <returns>TEX chunk.</returns>
      public static string FixedColumnWidth(double value, Helper.MeasurementUnits unit)
      {
         return FixedColumnWidth(Helper.Length(value, unit));
      }

      public static string FixedColumnWidth(string length)
      {
         return $"p{{{length}}}";
      }

      /// <summary>
      ///    This methods creates a horizontal line within a table.
      /// </summary>
      /// <returns>TEX chunk.</returns>
      private static string HLine()
      {
         return "\\hline\n";
      }

      /// <summary>
      ///    This function creates a TEX chunk to draw a horizontal line within a table from startCol to endCol.
      /// </summary>
      /// <param name="startCol">Number of column to start with.</param>
      /// <param name="endCol">Number of column to end with.</param>
      /// <returns>TEX chunk.</returns>
      private static string CLine(int startCol, int endCol)
      {
         return $"\\cline{{{startCol.ToString(CultureInfo.InvariantCulture)}-{endCol.ToString(CultureInfo.InvariantCulture)}}}";
      }

      /// <summary>
      ///    This method determines the maximum number of characters used in a string column of a data view.
      /// </summary>
      /// <param name="data">Data view to analyze.</param>
      /// <param name="column">Column to analyze.</param>
      /// <returns>Maximum number of characters used.</returns>
      private static int GetMaxLength(DataView data, DataColumn column)
      {
         if (column.DataType != typeof (string)) return 0;
         var maxLength = 0;
         foreach (DataRowView row in data)
         {
            var objValue = row[column.ColumnName];
            if (objValue == null || objValue == DBNull.Value) continue;
            var length = objValue.ToString().Length;
            if (length > maxLength) maxLength = length;
         }
         return maxLength;
      }

      /// <summary>
      ///    This method generates the a generic column design for data view columns.
      /// </summary>
      /// <remarks>
      ///    Standard alignment is right. Dates are centered aligned. Strings are left
      ///    aligned or if the have lot of text a fixed column with is set.
      /// </remarks>
      /// <param name="data">Data view to analyze.</param>
      /// <param name="relation">Relation associated to the design</param>
      /// <returns>TEX chunk.</returns>
      private static string createColumnDesign(DataView data, DataRelation relation = null)
      {
         //column design
         var columnDesign = new StringBuilder();
         columnDesign.Append(Helper.StartBlock());
         int factor = -1;

         foreach (DataColumn col in data.Table.Columns)
         {
            if (col.IsHidden()) continue;
            if (relation != null)
               if (relation.ChildColumns.Contains(col)) continue;

            columnDesign.AppendFormat(" {0} ", VERTICAL_LINE);

            var aligment = col.GetAlignment();

            if (col.DataType == typeof(string) || col.DataType == typeof(Text))
               if (aligment == null)
                  columnDesign.AppendFormat("{0}[{1}]", X_COLUMN, factor);
               else
                  columnDesign.AppendFormat("{0}[{1}, {2}]", X_COLUMN, factor, aligment);
            else if (col.DataType == typeof(DateTime))
               columnDesign.AppendFormat("{0}[{1},{2}]", X_COLUMN, factor, aligment ?? ColumnAlignments.c);
            else if (col.DataType == typeof(double) || col.DataType == typeof(float))
               if (aligment == null)
                  columnDesign.AppendFormat(">{{\\tabudecimal \\alignnumbers}}{0}[{1}]", X_COLUMN, 2 * factor);
               else
                  columnDesign.AppendFormat("{0}[{1},{2}]", X_COLUMN, factor, aligment);
            else
               columnDesign.AppendFormat("{0}[{1},{2}]", X_COLUMN, factor, aligment ?? ColumnAlignments.r);
         }
         columnDesign.AppendFormat(" {0} ", VERTICAL_LINE);
         columnDesign.Append(Helper.EndBlock());
         return columnDesign.ToString();
      }

      /// <summary>
      ///    This method streches the cells of a table.
      /// </summary>
      /// <param name="value">Factor to stretch. 1 means no strech.</param>
      /// <returns>TEX chunk.</returns>
      private static string arrayStretch(double value)
      {
         return String.Format(@"{1}\renewcommand{{\arraystretch}}{{{0}}}{1}", value.ToString(CultureInfo.InvariantCulture), Helper.LineFeed());
      }

      /// <summary>
      ///    This method stacks values separted by \\.
      /// </summary>
      /// <param name="alignment">Alignment of stacked values.</param>
      /// <param name="values">String containing values to stack separated by \\.</param>
      /// <returns>TEX chunk.</returns>
      private static string shortStack(ColumnAlignments alignment, string values)
      {
         return $"\\shortstack[{alignment}]{{\\\\{values}}}";
      }
    
      /// <summary>
      ///    This method creates a header for tables.
      /// </summary>
      /// <param name="data">Data view containing data.</param>
      /// <param name="tableNotes">Notes of the table.</param>
      /// <param name="converter">Tex converter.</param>
      /// <param name="relation"></param>
      /// <returns>TEX chunk.</returns>
      private static string header(DataView data, Dictionary<string, char> tableNotes , ITeXConverter converter, DataRelation relation = null)
      {
         // create standard header
         var header = new StringBuilder();
         header.Append("\\rowfont[c]{\\bfseries}\n");
         var newHeader = true;
         foreach (DataColumn col in data.Table.Columns)
         {
            if (col.IsHidden()) continue;
            if (relation != null)
               if (relation.ChildColumns.Contains(col)) continue;
            if (newHeader)
            {
               header.AppendFormat("{{{0}}}",converter.StringToTeX(col.Caption));
               newHeader = false;
            }
            else
               header.AppendFormat(" {0} {{{1}}}", VALUE_SEPARATOR, converter.StringToTeX(col.Caption));

            header.Append(TNote(col, tableNotes));
            var unit = col.GetUnit();
            if (!String.IsNullOrEmpty(unit)) header.AppendFormat(" [{0}]", converter.StringToTeX(unit));
         }
         header.Append(Helper.LineBreak());
         return header.ToString();
      }

      /// <summary>
      ///    This method creates a nicer header for pivot tables.
      /// </summary>
      /// <param name="pivotData">Data view containing data to pivot.</param>
      /// <param name="tableNotes">Notes of table.</param>
      /// <param name="pivotInfo">Information specifying the pivotation.</param>
      /// <param name="texTranslations">Use defined tex string for naming aggregations within the tex header.</param>
      /// <param name="converter">Tex converter.</param>
      /// <returns>TEX chunk.</returns>
      private static string headerForPivot(DataView pivotData, Dictionary<string, char> tableNotes, PivotInfo pivotInfo, Cache<Aggregate, string> texTranslations , ITeXConverter converter)
      {
         var colFieldNames = pivotInfo.ColumnFields;
         var colValueList = pivotData.ToTable(true, colFieldNames.ToArray()).AsEnumerable().ToList();
         
         var header = new StringBuilder();
         var headerRow = new StringBuilder();
         var multiplied = pivotInfo.ColumnFields.Count;

         header.Append(HLine());
         for (int i = 0; i < colFieldNames.Count; i++)
         {
            var fieldName = colFieldNames[i];
            var columnName = $"{{{converter.StringToTeX(fieldName)}}}";
            if (pivotData.Table.Columns.Contains(fieldName))
            {
               columnName += TNote(pivotData.Table.Columns[fieldName], tableNotes);
               var unit = pivotData.Table.Columns[fieldName].GetUnit();
               if (!String.IsNullOrEmpty(unit)) columnName = $"{columnName} [{converter.StringToTeX(unit)}]";
            }
            if (headerRow.Length > 0)
               headerRow.Append(multiColumn(pivotInfo.RowFields.Count, ColumnAlignments.r,
                                             Helper.Bold(columnName)));
            else
               headerRow.Append(multiFirstColumn(pivotInfo.RowFields.Count, ColumnAlignments.r,
                                                   Helper.Bold(columnName)));
            for (int index = 0; index < colValueList.Count; index++)
            {
               var colValue = colValueList[index];
               var value = colValue[fieldName].ToString();
               var multiples = 0;
               while (multiples < multiplied && (index + multiples + 1) < colValueList.Count &&
                        value == colValueList[index + multiples + 1][fieldName].ToString())
               {
                  multiples++;
               }
               multiplied = multiples;
               index += multiples;
               headerRow.Append(VALUE_SEPARATOR);
               headerRow.Append(multiColumn(pivotInfo.Aggregates.Count() * pivotInfo.DataFields.Count * (multiples + 1), ColumnAlignments.c,
                                             converter.StringToTeX(colValue[fieldName].ToString())));
            }

            headerRow.Append(Helper.LineBreak());
            headerRow.Append(HLine());
            header.Append(headerRow);
            headerRow = new StringBuilder();
         }

         headerRow = new StringBuilder();
         foreach (var rowField in pivotInfo.RowFields)
         {
            if (headerRow.Length > 0) headerRow.Append(VALUE_SEPARATOR);
            headerRow.Append(Helper.Bold(converter.StringToTeX(rowField)));
            if (pivotData.Table.Columns.Contains(rowField))
            {
               headerRow.Append(Helper.Bold(TNote(pivotData.Table.Columns[rowField], tableNotes)));
               var unit = pivotData.Table.Columns[rowField].GetUnit();
               if (!String.IsNullOrEmpty(unit)) headerRow.AppendFormat(" [{0}]", converter.StringToTeX(unit));
            }
         }
         foreach (var colValue in colValueList)
            foreach (var aggregate in pivotInfo.Aggregates)
               foreach (var datafield in pivotInfo.DataFields)
               {
                  headerRow.Append(VALUE_SEPARATOR);
                  if (aggregate != null)
                  {
                     var funcName = texTranslations.Contains(aggregate) ? texTranslations[aggregate] : aggregate.Name;
                     headerRow.Append(Helper.Bold(funcName));
                     if (aggregate.DataType == typeof(bool) || aggregate.DataType == typeof(int)) continue;
                  }
                  if (pivotInfo.DataFields.Count > 1)
                  {
                     var datafieldName = converter.StringToTeX(datafield);
                     datafieldName += TNote(pivotData.Table.Columns[datafield], tableNotes);
                     headerRow.Append(Helper.Bold($"({datafieldName})"));
                  }
                  if (pivotData.Table.Columns.Contains(datafield))
                  {
                     var unit = pivotData.Table.Columns[datafield].GetUnit();
                     if (!String.IsNullOrEmpty(unit)) headerRow.Append(Helper.Bold($" [{converter.StringToTeX(unit)}]"));
                  }
               }
         headerRow.Append(Helper.LineBreak());
         header.Append(headerRow);
         header.Append(HLine());

         return header.ToString();
      }

      private static string longTableHeader(DataView data, Dictionary<string, char> tableNotes, ITeXConverter converter)
      {
         const string END_HEAD = "\\endhead\n";
         const string END_FIRST_HEAD = "\\endfirsthead\n";
         const string END_FOOT = "\\endfoot\n";
         const string END_LAST_FOOT = "\\endlastfoot\n";

         var header = TableWriter.header(data, tableNotes, converter);
         var shownColumns = data.Table.Columns.Cast<DataColumn>().Count(col => !col.IsHidden());

         var tex = new StringBuilder();
         //table header for all continued pages
         tex.Append(HLine());
         tex.Append(multiFirstColumn(shownColumns, ColumnAlignments.l, "\\small\\sl continued from previous page"));
         tex.Append(Helper.LineBreak());
         tex.Append(HLine());
         tex.Append(header);
         tex.Append(HLine());
         tex.Append(END_HEAD);

         //table header
         tex.Append(HLine());
         tex.Append(header);
         tex.Append(HLine());
         tex.Append(END_FIRST_HEAD);

         //table tail
         tex.Append(HLine());
         tex.Append(multiFirstColumn(shownColumns, ColumnAlignments.r, "\\small\\sl continued on next page"));
         tex.Append(Helper.LineBreak());
         tex.Append(HLine());
         tex.Append(END_FOOT);

         //table last tail
         tex.Append(END_LAST_FOOT);

         return tex.ToString();
      }

      private static string multiFirstColumn(int numberOfColumns, string alignment, string content)
      {
         return String.Format("\\multicolumn{{{0}}}{{{1}{2}{1}}}{{{3}}}", numberOfColumns, VERTICAL_LINE, alignment, content);
      }

      private static string multiFirstColumn(int numberOfColumns, ColumnAlignments alignment, string content)
      {
         return multiFirstColumn(numberOfColumns, alignment.ToString(), content);
      }

      public static string MultiColumn(int numberOfColumns, string alignment, string content)
      {
         return String.Format("\\multicolumn{{{0}}}{{{2}{1}}}{{{3}}}", numberOfColumns, VERTICAL_LINE, alignment, content);
      }

      private static string multiColumn(int numberOfColumns, ColumnAlignments alignment, string content)
      {
         return MultiColumn(numberOfColumns, alignment.ToString(), content);
      }

      private static string createTable(DataView data, ITeXConverter converter, ITeXBuilderRepository builderRepository, bool hasNotes, string header, Dictionary<string, char> tableNotes, int spaceNeeded = 6, string caption = null, string label = null)
      {
         var tex = new StringBuilder();
         //if the available space is less than 6 lines the table starts on next page to avoid page breaks for with no lines.
         tex.Append(Helper.Needspace(Helper.BaseLineSkip(spaceNeeded)));
         tex.Append(Helper.Begin(Helper.Environments.center));
         tex.Append(arrayStretch(ARRAY_STRETCH));

         if (hasNotes)
            tex.Append(Helper.Begin(Helper.Environments.ThreePartTable));

         tex.Append(Helper.Begin(Helper.Environments.longtabu));
         tex.AppendFormat(" to {0}\\linewidth", TABLE_WIDTH);
         tex.Append(createColumnDesign(data));

         //Create header
         tex.Append(header);

         // rows
         tex.Append(createRows(data, tableNotes, converter, builderRepository));
         tex.Append(HLine());

         if (caption != null)
         {
            tex.Append(Helper.Caption(caption));
            tex.Append(Helper.Label(label));
            tex.Append(Helper.LineBreak());
         }

         tex.Append(Helper.End(Helper.Environments.longtabu));

         if (caption == null)
         {
            //the longtable package always increases the table counter. If no caption is set that is not wanted.
            tex.Append(decreaseTableCounter());
         }

         if (hasNotes)
         {
            tex.Append(TableWriter.tableNotes(tableNotes, converter));
            tex.Append(Helper.End(Helper.Environments.ThreePartTable));
         }

         tex.Append(Helper.End(Helper.Environments.center));
         return tex.ToString();
      }

      /// <summary>
      ///    This method generates a simple table without caption and label from a given data view object.
      /// </summary>
      /// <param name="data">Data to be shown in table.</param>
      /// <param name="converter">Tex converter.</param>
      /// <param name="builderRepository">Repository builder needed to get chunks of tex objects.</param>
      /// <returns>TEX Chunk.</returns>
      public static string SimpleTable(DataView data, ITeXConverter converter, ITeXBuilderRepository builderRepository)
      {
         if (data.Count == 0) return String.Empty;

         var hasNotes = HasNotes(data.Table);
         var tableNotes = getTableNotes(data);
         var header = longTableHeader(data, tableNotes, converter);

         return createTable(data, converter, builderRepository, hasNotes, header, tableNotes);
      }

      /// <summary>
      ///    This method generates a tabular from a given data view object.
      /// </summary>
      /// <seealso cref="SimpleTable" />
      /// <remarks>
      /// </remarks>
      /// <param name="caption">>Text for table caption.</param>
      /// <param name="label">Text for table lable. Should start with tab:.</param>
      /// <param name="data">Data to be shown in table.</param>
      /// <param name="pivotInfo">Information about pivotation.</param>
      /// <param name="texTranslations">For used aggregations a tex conform string can be set.</param>
      /// <param name="converter">Converter.</param>
      /// <param name="builderRepository">Repository builder needed to get chunks of tex objects.</param>
      /// <param name="rawData">Data which has been pivoted.</param>
      public static string PivotTable(string caption, string label, DataView data, DataTable rawData, PivotInfo pivotInfo, Cache<Aggregate, string> texTranslations, ITeXConverter converter, ITeXBuilderRepository builderRepository)
      {
         if (data.Count == 0) return String.Empty;

         var tableNotes = getTableNotes(rawData, pivotInfo);
         var hasNotes = tableNotes.Any();
         var header = headerForPivot(rawData.DefaultView, tableNotes, pivotInfo, texTranslations, converter);

         return createTable(data, converter, builderRepository, hasNotes, header, tableNotes, spaceNeeded: 12, caption: caption, label: label);
      }

      /// <summary>
      ///    This method generates a tabular from a given data view object.
      /// </summary>
      /// <seealso cref="SimpleTable" />
      /// <remarks>
      /// </remarks>
      /// <param name="caption">>Text for table caption.</param>
      /// <param name="label">Text for table lable. Should start with tab:.</param>
      /// <param name="data">Data to be shown in table.</param>
      /// <param name="converter">Converter.</param>
      /// <param name="builderRepository">Repository builder needed to get chunks of tex objects.</param>
      public static string Table(string caption, string label, DataView data, ITeXConverter converter, ITeXBuilderRepository builderRepository)
      {
         if (data.Count == 0) return String.Empty;

         var hasNotes = HasNotes(data.Table);
         var tableNotes = getTableNotes(data);
         var header = longTableHeader(data, tableNotes, converter);

         return createTable(data, converter, builderRepository, hasNotes, header, tableNotes, caption: caption, label: label);
      }

      private static string decreaseTableCounter()
      {
         return "\\addtocounter{table}{-1}\n";
      }

      /// <summary>
      /// This creates all table notes.
      /// </summary>
      /// <param name="tableNotes"></param>
      /// <param name="converter"></param>
      /// <returns></returns>
      private static string tableNotes(Dictionary<string, char> tableNotes, ITeXConverter converter)
      {
         var tex = new StringBuilder();
         tex.Append(Helper.Begin(Helper.Environments.flushleft));
         tex.Append(notesRule());
         tex.Append(Helper.Begin(Helper.Environments.tablenotes));

         foreach (var note in tableNotes)
            tex.Append(tableNote(note.Value, converter.StringToTeX(note.Key)));

         tex.Append(Helper.End(Helper.Environments.tablenotes));
         tex.Append(Helper.End(Helper.Environments.flushleft));
         return tex.ToString();
      }

      /// <summary>
      /// This gets the table notes of pivoted tables.
      /// </summary>
      /// <remarks>For all data field columns the tables notes are removed if not used by any other column, 
      /// because the output of pivot tables does not show the column names anymore.</remarks>
      /// <param name="pivotData"></param>
      /// <param name="pivotInfo"></param>
      /// <returns></returns>
      private static Dictionary<string, char> getTableNotes(DataTable pivotData, PivotInfo pivotInfo)
      {
         var notes = new Dictionary<string, char>();

         if (pivotData != null)
         {
            var tableNotes = getTableNotes(pivotData.DefaultView);
            //remove notes only used by data field, because data field column name is not shown for pivot tables.
            if (pivotInfo.DataFields.Count == 1)
            {
               var dataField = pivotData.Columns[pivotInfo.DataFields[0]];
               foreach (var note in dataField.GetNotes())
                  if (!pivotData.Columns.Cast<DataColumn>().Any(c => (c.ColumnName != dataField.ColumnName && c.GetNotes().Contains(note))))
                     tableNotes.Remove(note);
            }
            return tableNotes;
         }

         return notes;
      }

      /// <summary>
      /// This gets the table notes of tables and their child tables.
      /// </summary>
      /// <param name="data"></param>
      /// <returns></returns>
      private static Dictionary<string, char> getTableNotes(DataView data)
      {
         var notes = new Dictionary<string, char>();

         char symbol = '0';
         foreach (DataColumn col in data.Table.Columns)
            foreach (var note in col.GetNotes())
               if (!notes.ContainsKey(note))
                  notes.Add(note, (++symbol));

         foreach (DataRelation child in data.Table.ChildRelations)
            foreach (DataColumn col in child.ChildTable.Columns)
               foreach (var note in col.GetNotes())
                  if (!notes.ContainsKey(note))
                     notes.Add(note, (++symbol));

         return notes;
      }

      private static string TNote(DataColumn column, Dictionary<string, char> tableNotes)
      {
         var symbols = new List<char>();
         foreach (var note in column.GetNotes())
            if (tableNotes.ContainsKey(note))
               symbols.Add(tableNotes[note]);
         if (symbols.Count == 0) return String.Empty;

         //the \tnote command take no space into account. Therefor I replaced that command with an equivalent command which takes the place into account
         //see http://TeX.stackexchange.com/questions/108634/why-is-the-tnote-of-my-threeparttables-table-cheekily-protruding-how-can-the-w
         //return String.Format("\\tnote{{{0}}}", String.Join(", ", symbols));

         return $"\\textsuperscript{{\\TPTtagStyle{{{String.Join(", ", symbols)}}}}}";
      }

      /// <summary>
      /// This specifies the layout of notes.
      /// </summary>
      /// <returns></returns>
      private static string notesRule()
      {
         return Helper.Rule(Helper.Length(0, Helper.MeasurementUnits.pt), Helper.GetLengthInPercentageOfTextWidth(40),
                                          Helper.Length(0.4, Helper.MeasurementUnits.pt));
      }

      public static bool HasNotes(DataTable data)
      {
         bool hasNotes = false;

         foreach (DataRelation child in data.ChildRelations)
            hasNotes |= HasNotes(child.ChildTable);
         hasNotes |= data.Columns.Cast<DataColumn>().Any(c => c.GetNotes().Length > 0);
         return hasNotes;
      }

      private static string tableNote(char symbol, string text)
      {
         return $"\\item [{symbol}] {text}\n";
      }

      private static string phantomline()
      {
         return "\\tabuphantomline\n";
      }

      private static bool isLastRow(this DataRowView row)
      {
         return Equals(row, row.DataView[row.DataView.Count - 1]);
      }

      private static string createRows(DataView data, Dictionary<string, char> tableNotes , ITeXConverter converter, ITeXBuilderRepository builderRepository, DataRelation relation = null, int? level=null)
      {
         var tex = new StringBuilder();
         foreach (DataRowView row in data)
            tex.Append(createRow(row, tableNotes, converter, builderRepository, relation, level));
         tex.Append(phantomline());
         return tex.ToString();
      }

      /// <summary>
      /// This is for tweaking page breaking in long tables.
      /// </summary>
      /// <param name="row">For last row of a view the pagebreak is more desirable.</param>
      /// <param name="level">For master tables the level is null. For detail tables it is >= 1.</param>
      /// <returns>Hint value for page breaking.</returns>
      private static PageBreakDesirability getPagebreakDesirability(DataRowView row, int? level)
      {
         var lastRow = row.isLastRow();
         return level == null ? 
            (lastRow ? PageBreakDesirability.High : PageBreakDesirability.Medium) : 
            (lastRow ? PageBreakDesirability.Low : PageBreakDesirability.VeryLow);
      }

      private static string setPagebreakDesirability(PageBreakDesirability pagebreakDesirability)
      {
         return $"\\pagebreak[{(int) pagebreakDesirability}]";
      }

      private static string createRow(DataRowView row, Dictionary<string, char> tableNotes, ITeXConverter converter, ITeXBuilderRepository builderRepository, DataRelation relation = null, int? level=null)
      {
         var rowLine = new StringBuilder();
         rowLine.Append(setPagebreakDesirability(getPagebreakDesirability(row, level)));

         var columnsCount = 0;
         foreach (DataColumn col in row.DataView.Table.Columns)
         {
            if (col.IsHidden()) continue;
            if (relation != null)
               if (relation.ChildColumns.Contains(col)) continue;
            columnsCount++;
            if (columnsCount > 1) rowLine.AppendFormat(" {0} ", VALUE_SEPARATOR);
            var colValue = row[col.ColumnName];
            if (colValue == null || colValue == DBNull.Value) continue;
            // datatype specifics 
            string value;
            if (col.DataType == typeof(double) || col.DataType == typeof(float))
               value = converter.StringToTeX(_numericFormatter.Format(Convert.ToDouble(colValue)));
            else if (col.DataType == typeof(DateTime))
               value = converter.StringToTeX(((DateTime) colValue).ToShortTimeString());
            else if (col.DataType == typeof(Text))
               value = builderRepository.ChunkFor(colValue);
            else
               value = converter.StringToTeX(colValue.ToString());
            rowLine.Append(value);
         }
         rowLine.Append(Helper.LineBreak());
         rowLine.Append(phantomline());
         var first = true;
         foreach (DataRelation childrelation in row.DataView.Table.ChildRelations)
         {
            var childView = row.CreateChildView(childrelation);
            if (childView.Count == 0) continue;
            if (first)
            {
               rowLine.Append(CLine(1, columnsCount));
               first = false;
            }

            rowLine.Append(multiFirstColumn(columnsCount, ColumnAlignments.c, 
                                            createChildTable(childView, tableNotes, childrelation, converter, builderRepository, ref level)));
            rowLine.Append(Helper.LineBreak());
            rowLine.Append(phantomline());
         }
         if (row.DataView.Table.ChildRelations.Count > 0)
            rowLine.Append(CLine(1, columnsCount));

         return rowLine.ToString();
      }

      private static string createChildTable(DataView childView, Dictionary<string, char> tableNotes, DataRelation relation, ITeXConverter converter, ITeXBuilderRepository builderRepository, ref int? level)
      {
         var childTable = new StringBuilder();
         if (level == null) level = 1;

         childTable.Append(Helper.Begin(Helper.Environments.tabu));
         level++;
         childTable.AppendFormat(" to {0}\\linewidth ", Math.Pow(TABLE_WIDTH, (double)level));
         childTable.Append(createColumnDesign(childView, relation));

         var childHeader = new StringBuilder();
         childHeader.Append(header(childView, tableNotes, converter, relation));
         var columnsCount = childView.Table.Columns.Cast<DataColumn>().Count(col => !col.IsHidden() && !relation.ChildColumns.Contains(col));
         childHeader.Append(CLine(1, columnsCount));
         childTable.Append(childHeader);

         childTable.Append(createRows(childView, tableNotes, converter, builderRepository, relation, level));
         childTable.Append(CLine(1, columnsCount));
         childTable.Append(Helper.End(Helper.Environments.tabu));
         level--;

         return childTable.ToString();
      }
   }
}