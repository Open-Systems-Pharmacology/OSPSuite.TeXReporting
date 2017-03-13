using System.Collections.Generic;
using System.Drawing;
using OSPSuite.TeXReporting.TeX.PGFPlots;

namespace OSPSuite.TeXReporting.Items
{
   public class PlotTwoOrdinates : Plot
   {
      public AxisOptions AxisOptionsY2 { get; private set; }
      public IEnumerable<TeX.PGFPlots.Plot> PlotsY2 { get; private set; }

      public PlotTwoOrdinates(IEnumerable<Color> colors, AxisOptions axisOptions, IEnumerable<TeX.PGFPlots.Plot> plots, AxisOptions axisOptionsY2, IEnumerable<TeX.PGFPlots.Plot> plotsY2, Text caption) :
         base(colors, axisOptions, plots, caption)
      {
         AxisOptionsY2 = axisOptionsY2;
         PlotsY2 = plotsY2;
      }
   }
}
