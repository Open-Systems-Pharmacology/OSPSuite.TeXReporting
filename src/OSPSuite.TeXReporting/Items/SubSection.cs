using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public class SubSection : StructureElement
   {
      public SubSection(string name) : base(Helper.StructureElements.subsection, name)
      {
      }

      public SubSection(string tableOfContentsTitle, Text text)
         : base(Helper.StructureElements.subsection, tableOfContentsTitle, text)
      {
      }
   }
}