using System.Drawing;

namespace OSPSuite.TeXReporting.Items
{
   public class ColorText : Text
   {
      public Color Color { get; }

      public ColorText(string content, Color color, params object[] items) : base(content, items)
      {
         Color = color;
      }
   }
}