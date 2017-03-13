using System.Collections.Generic;
using System.Drawing;
using OSPSuite.TeXReporting.TeX.PGFPlots;

namespace OSPSuite.TeXReporting.Items
{
   public class BarPlot : Plot
   {
      public BarPlotOptions BarPlotOptions { get; private set; }

      public BarPlot(IEnumerable<Color> colors, AxisOptions axisOptions, BarPlotOptions barPlotOptions, IEnumerable<TeX.PGFPlots.Plot> plots, Text caption) 
         :base(colors, axisOptions, plots, caption)
      {
         BarPlotOptions = barPlotOptions;
      }

   }
}
