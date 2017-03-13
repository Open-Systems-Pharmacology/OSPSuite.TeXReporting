using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class PlotThreeOrdinatesTeXBuilder : TeXBuilder<PlotThreeOrdinates>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public PlotThreeOrdinatesTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(PlotThreeOrdinates plot, BuildTracker tracker)
      {
         var captionText = _builderRepository.ChunkFor(plot.Caption);
         tracker.Track(plot.Caption.Items);
         tracker.Track(plot.Caption);

         tracker.TeX.Append(FigureWriter.PlotThreeOrdinatesFigure(captionText, plot, TeX.Converter.DefaultConverter.Instance));
      }
   }
}