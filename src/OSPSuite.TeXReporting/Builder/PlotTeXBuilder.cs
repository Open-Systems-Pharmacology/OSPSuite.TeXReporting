using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class PlotTeXBuilder : TeXBuilder<Plot>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public PlotTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(Plot plot, BuildTracker tracker)
      {
         var captionText = _builderRepository.ChunkFor(plot.Caption);
         tracker.Track(plot.Caption.Items);
         tracker.Track(plot.Caption);

         tracker.TeX.Append(FigureWriter.PlotFigure(captionText, plot, TeX.Converter.DefaultConverter.Instance));
      }
   }
}