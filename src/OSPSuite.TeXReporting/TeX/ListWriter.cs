using System;
using System.Text;

namespace OSPSuite.TeXReporting.TeX
{
   /// <summary>
   /// This class encapsulates the list environments in latex.
   /// </summary>
   internal static class ListWriter
   {
      /// <summary>
      /// These are the supported list environments.
      /// </summary>
      public enum ListEnvironments
      {
         /// <summary>
         /// The description environment produces a labeled list. 
         /// </summary>
         description,
         /// <summary>
         /// The enumerate environment produces a numbered list. 
         /// At least one item is needed. 
         /// Enumerate lists can be nested inside other enumerate lists, up to four levels deep. 
         /// </summary>
         enumerate,
         /// <summary>
         /// The itemize environment creates an unnumbered, or "bulleted" list. 
         /// </summary>
         itemize
      }


      public static string Begin(ListEnvironments environment)
      {
         return $"\\begin{{{environment}}}\n";
      }

      public static string End(ListEnvironments environment)
      {
         return $"\\end{{{environment}}}\n";
      }

      public static string Item(string item)
      {
         return $"\\item {item}\n";
      }

      public static string LabelledItem(string label, string item)
      {
         return $"\\item[{label}] {item}\n";
      }

      private static string CreateList(ListEnvironments environment, string[] items)
      {
         if (items == null) return String.Empty;
         if (items.Length == 0) return String.Empty;

         var tex = new StringBuilder();
         tex.Append(Begin(environment));
         foreach (var item in items)
            tex.Append(Item(item));
         tex.Append(End(environment));
         return tex.ToString();
      }

      public class LabeledItem
      {
         public string Label;
         public string Item;
      }

      public static string Description(LabeledItem[] items)
      {
         if (items == null) return String.Empty;
         if (items.Length == 0) return String.Empty;

         var tex = new StringBuilder();
         tex.Append(Begin(ListEnvironments.description));
         foreach (LabeledItem item in items)
            tex.Append(LabelledItem(item.Label, item.Item));
         tex.Append(End(ListEnvironments.description));
         return tex.ToString();
      }

      public static string ItemizedList(string[] items)
      {
         return CreateList(ListEnvironments.itemize, items);
      }

      public static string EnumeratedList(string[] items)
      {
         return CreateList(ListEnvironments.enumerate, items);
      }

   }
}
