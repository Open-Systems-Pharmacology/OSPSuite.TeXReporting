using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class GroupPlotTeXBuilder : TeXBuilder<GroupPlot>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public GroupPlotTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(GroupPlot plot, BuildTracker tracker)
      {
         var captionText = _builderRepository.ChunkFor(plot.Caption);
         tracker.Track(plot.Caption.Items);
         tracker.Track(plot.Caption);

         tracker.TeX.Append(FigureWriter.GroupPlotFigure(captionText, plot, TeX.Converter.DefaultConverter.Instance));
      }
   }
}
