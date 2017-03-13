namespace OSPSuite.TeXReporting.TeX.Converter
{
   public class NoConverter : TeXConverter
   {
      /// <summary>
      ///    The singleton instance of <see cref="NoConverter" />.
      /// </summary>
      /// <value>
      ///    The singleton instance of <see cref="NoConverter" />.
      /// </value>
      public static readonly NoConverter Instance = new NoConverter();

      private NoConverter()
      {
      }

      public override string StringToTeX(string text)
      {
         return text;
      }
   }
}