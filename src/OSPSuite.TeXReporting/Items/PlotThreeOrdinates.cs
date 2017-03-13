using System.Collections.Generic;
using System.Drawing;
using OSPSuite.TeXReporting.TeX.PGFPlots;

namespace OSPSuite.TeXReporting.Items
{
   public class PlotThreeOrdinates : PlotTwoOrdinates
   {
      public AxisOptions AxisOptionsY3 { get; private set; }
      public IEnumerable<TeX.PGFPlots.Plot> PlotsY3 { get; private set; }

      public PlotThreeOrdinates(IEnumerable<Color> colors, AxisOptions axisOptions, IEnumerable<TeX.PGFPlots.Plot> plots, AxisOptions axisOptionsY2, IEnumerable<TeX.PGFPlots.Plot> plotsY2, AxisOptions axisOptionsY3, IEnumerable<TeX.PGFPlots.Plot> plotsY3, Text caption) :
         base(colors, axisOptions, plots, axisOptionsY2, plotsY2, caption)
      {
         AxisOptionsY3 = axisOptionsY3;
         PlotsY3 = plotsY3;
      }
   }
}
