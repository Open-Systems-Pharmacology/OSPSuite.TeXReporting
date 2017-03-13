namespace OSPSuite.TeXReporting.TeX.Converter

{
   public class DefaultConverter : TeXConverter
   {
      /// <summary>
      ///    The singleton instance of <see cref="DefaultConverter" />.
      /// </summary>
      /// <value>
      ///    The singleton instance of <see cref="DefaultConverter" />.
      /// </value>
      public static readonly DefaultConverter Instance = new DefaultConverter();

      /// <summary>
      ///    This converter replaces special characters from text values and file path strings.
      /// </summary>
      private DefaultConverter()
      {
         _textCriticals.Add('_', "\\_\\-"); //with \- it is allowed to make a word wrap
      }
   }
}