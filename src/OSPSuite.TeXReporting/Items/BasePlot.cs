using System.Collections.Generic;
using OSPSuite.TeXReporting.TeX.PGFPlots;

namespace OSPSuite.TeXReporting.Items
{
   public class BasePlot : IBasePlot
   {
      private readonly List<TeX.PGFPlots.Plot> _plots;
      public AxisOptions AxisOptions { get; set; }
      public IEnumerable<TeX.PGFPlots.Plot> Plots
      {
         get { return _plots; }
      }

      public BasePlot(AxisOptions axisOptions, IEnumerable<TeX.PGFPlots.Plot> plots)
      {
         AxisOptions = axisOptions;
         _plots = new List<TeX.PGFPlots.Plot>(plots);
      }

      public void AddPlots(IEnumerable<TeX.PGFPlots.Plot> plots)
      {
         _plots.AddRange(plots);
      }

      public void InsertPlots(int index, IEnumerable<TeX.PGFPlots.Plot> plots)
      {
         _plots.InsertRange(index, plots);
      }
   }
}
