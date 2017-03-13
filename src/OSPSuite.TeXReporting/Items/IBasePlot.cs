using System.Collections.Generic;
using OSPSuite.TeXReporting.TeX.PGFPlots;

namespace OSPSuite.TeXReporting.Items
{
   public interface IBasePlot
   {
      AxisOptions AxisOptions { get; set; }
      IEnumerable<TeX.PGFPlots.Plot> Plots { get; }
      void AddPlots(IEnumerable<TeX.PGFPlots.Plot> plots);
      void InsertPlots(int index, IEnumerable<TeX.PGFPlots.Plot> plots);
   }
}
