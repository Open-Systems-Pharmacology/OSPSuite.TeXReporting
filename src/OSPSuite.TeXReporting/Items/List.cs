using System.Collections.Generic;

namespace OSPSuite.TeXReporting.Items
{
   public class List : BaseItem
   {
      public ListStyles ListStyle { get; private set; }
      public IReadOnlyList<Text> Items { get; private set; }

      public enum ListStyles
      {
         itemized,
         enumerated
      }

      public List(IEnumerable<Text> items, ListStyles style = ListStyles.itemized)
      {
         ListStyle = style;
         Items = new List<Text>(items);
      }
   }
}
