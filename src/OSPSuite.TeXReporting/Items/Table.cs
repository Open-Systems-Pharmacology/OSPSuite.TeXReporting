using System.Data;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public class Table : BaseItem, IReferenceable, ICaptionItem
   {
      public DataView Data { get; private set; }
      public string Label { get; private set; }
      public Text Caption { get; private set; }

      public Table(DataView data, Text caption)
      {
         Data = data;
         Label = Helper.Marker();
         Caption = caption;
      }

      public Table(DataView data, string caption) : this(data, new Text(caption))
      {
      }
   }
}