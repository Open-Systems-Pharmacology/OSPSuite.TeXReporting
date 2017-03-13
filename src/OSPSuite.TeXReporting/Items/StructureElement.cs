using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public abstract class StructureElement : BaseItem, IReferenceable
   {
      public Helper.StructureElements Element { get; set; }
      public string Name { get; private set; }
      public Text Text { get; private set; }
      public string Label { get; private set; }

      public string TableOfContentsTitle { get; set; }
      public bool CreateTableOfContentsEntry { get; set; }

      protected StructureElement(Helper.StructureElements structureElement, string name)
      {
         Element = structureElement;
         Name = name;
         Text = null;
         Label = Helper.Marker();
         TableOfContentsTitle = string.Empty;
         CreateTableOfContentsEntry = true;
      }

      protected StructureElement(Helper.StructureElements structureElement, string tableOfContentsTitle, Text text)
      {
         Element = structureElement;
         Name = string.Empty;
         TableOfContentsTitle = TeX.Converter.DefaultConverter.Instance.StringToTeX(tableOfContentsTitle);
         Text = text;
         Converter = TeX.Converter.NoConverter.Instance;
         Label = Helper.Marker();
         CreateTableOfContentsEntry = true;
      }
   }
}