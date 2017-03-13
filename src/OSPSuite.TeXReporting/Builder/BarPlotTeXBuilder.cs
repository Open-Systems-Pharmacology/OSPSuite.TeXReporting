using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class BarPlotTeXBuilder : TeXBuilder<BarPlot>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public BarPlotTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(BarPlot barPlot, BuildTracker tracker)
      {
         var captionText = _builderRepository.ChunkFor(barPlot.Caption);
         tracker.Track(barPlot.Caption.Items);
         tracker.Track(barPlot.Caption);

         tracker.TeX.Append(FigureWriter.BarPlotFigure(captionText, barPlot, TeX.Converter.DefaultConverter.Instance));
      }
   }
}