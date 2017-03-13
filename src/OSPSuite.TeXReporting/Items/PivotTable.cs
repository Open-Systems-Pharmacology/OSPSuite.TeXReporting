using System.Data;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Data;

namespace OSPSuite.TeXReporting.Items
{
   internal class PivotTable : Table
   {
      public PivotInfo PivotInfo { get; private set; }
      public Cache<Aggregate, string> TexTranslations { get; private set; }

      public PivotTable(DataView data, PivotInfo pivotInfo, Cache<Aggregate, string> texTranslations, Text caption)
         : base(data, caption)
      {
         PivotInfo = pivotInfo;
         TexTranslations = texTranslations;
      }

      public PivotTable(DataView data, PivotInfo pivotInfo, Cache<Aggregate, string> texTranslations, string caption) : this(data, pivotInfo, texTranslations, new Text(caption))
      {
      }
   }
}