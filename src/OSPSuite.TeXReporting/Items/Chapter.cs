using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public class Chapter : StructureElement
   {
      public Chapter(string name) : base(Helper.StructureElements.chapter, name)
      {
      }

      public Chapter(string tableOfContentsTitle, Text text)
         : base(Helper.StructureElements.chapter, tableOfContentsTitle, text)
      {
      }

   }
}