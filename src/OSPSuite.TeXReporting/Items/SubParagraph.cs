using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public class SubParagraph : StructureElement
   {
      public SubParagraph(string name) : base(Helper.StructureElements.subparagraph, addLineBreak(name))
      {
         CreateTableOfContentsEntry = false;
      }

      public SubParagraph(string tableOfContentsTitle, Text text)
         : base(Helper.StructureElements.subparagraph, tableOfContentsTitle, text)
      {
         CreateTableOfContentsEntry = false;
         text.Content = addLineBreak(text.Content);
      }

      private static string addLineBreak(string name)
      {
         if (name.EndsWith("\n"))
            return name;

         return string.Format("{0}\n", name);
      }
   }
}
