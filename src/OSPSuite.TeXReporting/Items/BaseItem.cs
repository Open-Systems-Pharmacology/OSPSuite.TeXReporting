using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.Items
{
   public abstract class BaseItem
   {
      public ITeXConverter Converter { get; set; }

      protected BaseItem()
      {
         Converter = DefaultConverter.Instance;
      }
   }
}