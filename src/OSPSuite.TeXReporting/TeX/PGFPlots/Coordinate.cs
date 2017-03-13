namespace OSPSuite.TeXReporting.TeX.PGFPlots
{
   public class Coordinate
   {
      public float X { get; private set; }
      public float Y { get; private set; }
      public float? errX;
      public float? errY;
      /// <summary>
      /// If the second y error is set this value is used to get error bars which are equidistant.
      /// </summary>
      public float? errY2;

      public Coordinate(float x, float y)
      {
         X = x;
         Y = y;
      }
   }
}
