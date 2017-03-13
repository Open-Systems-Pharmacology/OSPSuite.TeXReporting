using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.Builder
{
   internal class FigureTeXBuilder : TeXBuilder<Figure>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public FigureTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(Figure figure, BuildTracker tracker)
      {
         var captionText = _builderRepository.ChunkFor(figure.Caption);
         tracker.Track(figure.Caption.Items);
         tracker.Track(figure.Caption);

         tracker.TeX.Append(FigureWriter.IncludeFigure(figure.Position, captionText, figure.Label, figure.RelativeFilePath, figure.Landscape,DefaultConverter.Instance));
      }
   }
}