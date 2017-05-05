using System;

namespace OSPSuite.TeXReporting.TeX
{
   public static class EquationWriter
   {

      /// <summary>
      /// This method creates a fraction.
      /// </summary>
      /// <param name="numerator">Numerator.</param>
      /// <param name="denominator">Denominator</param>
      /// <returns>TEX chunk.</returns>
      public static string Fraction(string numerator, string denominator)
      {
         return $"\\frac{{{numerator}}}{{{denominator}}}";
      }

      /// <summary>
      /// This method creates a root
      /// </summary>
      /// <param name="magnitude">Magnitude of root.</param>
      /// <param name="term">Term rooted.</param>
      /// <returns>TEX chunk.</returns>
      public static string Root(string magnitude, string term)
      {
         return $"\\sqrt[{magnitude}]{{{term}}}";         
      }

      /// <summary>
      /// This method creates a sum.
      /// </summary>
      /// <param name="start"></param>
      /// <param name="end"></param>
      /// <param name="term"></param>
      /// <returns></returns>
      public static string Sum(string start, string end, string term)
      {
         return $"\\sum_{{{start}}}^{{{end}}} {term}";         
      }


   }
}
