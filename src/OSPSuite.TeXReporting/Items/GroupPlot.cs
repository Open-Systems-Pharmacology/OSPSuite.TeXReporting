using System.Collections.Generic;
using System.Drawing;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.PGFPlots;

namespace OSPSuite.TeXReporting.Items
{
   public class GroupPlot : IPlot
   {
      public IEnumerable<Color> Colors { get; private set; }
      public AxisOptions AxisOptions { get; set; }
      public GroupOptions GroupOptions { get; private set; }
      public IEnumerable<IBasePlot> GroupedPlots { get; private set; }
      public string Label { get; private set; }
      public Text Caption { get; private set; }
      public bool Landscape { get; set; }
      public FigureWriter.FigurePositions Position { get; set; }

      public GroupPlot(IEnumerable<Color> colors, AxisOptions axisOptions, GroupOptions groupOptions, IEnumerable<IBasePlot> groupedPlots, Text caption)
      {
         Colors = colors;
         AxisOptions = axisOptions;
         GroupOptions = groupOptions;
         if (GroupOptions.GroupLegendOptions != null && groupOptions.GroupLegendOptions.LegendOptions != null)
            axisOptions.LegendOptions = null;
         GroupedPlots = groupedPlots;
         Position = FigureWriter.FigurePositions.htbp;
         Caption = caption;
         Label = Helper.Marker();
         Landscape = false;
      }
   }
}
