using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.Builder
{
   internal class InlineImageTeXBuilder : TeXChunkBuilder<InlineImage>
   {
      public override void Build(InlineImage image, BuildTracker tracker)
      {
         base.Build(image, tracker);
         tracker.Track(image);
      }

      public override string TeXChunk(InlineImage image)
      {
         return TeX.FigureWriter.InlineGraphic(image.RelativeFilePath, DefaultConverter.Instance);
      }
   }
}
