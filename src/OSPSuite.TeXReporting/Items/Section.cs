using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public class Section : StructureElement
   {
      public Section(string name) : base(Helper.StructureElements.section, name)
      {
      }

      public Section(string tableOfContentsTitle, Text text)
         : base(Helper.StructureElements.section, tableOfContentsTitle, text)
      {
      }
   }
}
