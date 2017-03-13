using System.Collections.Generic;

namespace OSPSuite.TeXReporting.Items
{
   /// <summary>
   ///    This item can contain text which will be converted by specified converter and child items.
   ///    The child items can be placed within the specified text content with placeholders like
   ///    in <see cref="string.Format(string,object)" />.
   /// </summary>
   public class Text : BaseItem
   {
      public string Content { get; set; }
      public Alignments Alignment { get; set; }
      public FontStyles FontStyle { get; set; }
      public IReadOnlyList<object> Items { get; private set; }

      public enum Alignments
      {
         justified,
         flushleft,
         flushright,
         centered
      }

      public enum FontStyles
      {
         normal,
         bold,
         italic,
         slanted
      }

      public Text(string content, params object[] items)
      {
         Content = content;
         Alignment = Alignments.justified;
         FontStyle = FontStyles.normal;
         Items = new List<object>(items);
      }
   }
}