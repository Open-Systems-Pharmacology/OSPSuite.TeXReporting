using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public class Paragraph : StructureElement
   {
      public Paragraph(string name) : base(Helper.StructureElements.paragraph, addLineBreak(name))
      {
         CreateTableOfContentsEntry = false;
      }

      public Paragraph(string tableOfContentsTitle, Text text)
         : base(Helper.StructureElements.paragraph, tableOfContentsTitle, text)
      {
         CreateTableOfContentsEntry = false;
         text.Content = addLineBreak(text.Content);
      }

      private static string addLineBreak(string name)
      {
         if (name.EndsWith("\n"))
            return name;

         return $"{name}\n";
      }
   }
}