using System.Collections.Generic;
using System.Drawing;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public interface IPlot : ICaptionItem, IReferenceable
   {
      IEnumerable<Color> Colors { get; }
      FigureWriter.FigurePositions Position { get; }
      bool Landscape { get; }
   }
}
