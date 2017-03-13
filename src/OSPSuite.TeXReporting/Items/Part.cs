using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public class Part : StructureElement
   {
      public Part(string name) : base(Helper.StructureElements.part, name)
      {
      }

      public Part(string tableOfContentsTitle, Text text)
         : base(Helper.StructureElements.part, tableOfContentsTitle, text)
      {
      }
   }
}