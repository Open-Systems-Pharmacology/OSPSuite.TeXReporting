using System.Data;

namespace OSPSuite.TeXReporting.Items
{
   public class SimpleTable : BaseItem
   {
      public DataView Data { get; private set; }

      public SimpleTable(DataView data)
      {
         Data = data;
      }
   }
}