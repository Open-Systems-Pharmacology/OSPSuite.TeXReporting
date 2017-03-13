using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public class SubSubSection : StructureElement
   {
      public SubSubSection(string name) : base(Helper.StructureElements.subsubsection, name)
      {
      }

      public SubSubSection(string tableOfContentsTitle, Text text)
         : base(Helper.StructureElements.subsubsection, tableOfContentsTitle, text)
      {
      }
   }
}