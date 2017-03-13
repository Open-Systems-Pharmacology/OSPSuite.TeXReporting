using System.Collections.Generic;
using System.Drawing;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.PGFPlots;

namespace OSPSuite.TeXReporting.Items
{
   public class Plot : BasePlot, IPlot
   {
      public IEnumerable<Color> Colors { get; private set; }
      public string Label { get; private set; }
      public Text Caption { get; private set; }
      public bool Landscape { get; private set; }
      public FigureWriter.FigurePositions Position { get; set; }

      public Plot(IEnumerable<Color> colors, AxisOptions axisOptions, IEnumerable<TeX.PGFPlots.Plot> plots, Text caption) :
         base(axisOptions, plots)
      {
         Colors = colors;
         Position = FigureWriter.FigurePositions.htbp;
         Caption = caption;
         Label = Helper.Marker();
         Landscape = false;
      }
   }
}
