
using System.Collections.Generic;

namespace OSPSuite.TeXReporting.Items
{

   /// <summary>
   /// This class allows to present all items in a landscaped environment.
   /// </summary>
   public class InLandscape
   {
      /// <summary>
      /// This class allows to present all items in a landscaped environment.
      /// </summary>
      /// <param name="items">Items to be presented in a landscaped environment.</param>
      public InLandscape(IEnumerable<object> items)
      {
         Items = items;
      }

      /// <summary>
      /// Items to be presented in a landscaped environment.
      /// </summary>
      public IEnumerable<object> Items { get; private set; }

   }
}
