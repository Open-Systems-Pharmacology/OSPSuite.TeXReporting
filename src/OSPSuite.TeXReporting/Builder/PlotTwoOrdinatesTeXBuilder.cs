using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class PlotTwoOrdinatesTeXBuilder : TeXBuilder<PlotTwoOrdinates>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public PlotTwoOrdinatesTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(PlotTwoOrdinates plot, BuildTracker tracker)
      {
         var captionText = _builderRepository.ChunkFor(plot.Caption);
         tracker.Track(plot.Caption.Items);
         tracker.Track(plot.Caption);

         tracker.TeX.Append(FigureWriter.PlotTwoOrdinatesFigure(captionText, plot, TeX.Converter.DefaultConverter.Instance));
      }
   }
}