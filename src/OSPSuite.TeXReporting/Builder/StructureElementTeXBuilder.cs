using System.Text;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class StructureElementTeXBuilder : TeXChunkBuilder<StructureElement>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public StructureElementTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(StructureElement element, BuildTracker tracker)
      {
         base.Build(element, tracker);
         if (element.Text != null)
            tracker.Track(element.Text.Items);
      }

      public override string TeXChunk(StructureElement element)
      {
         var tex = new StringBuilder();
         tex.Append(Helper.Needspace(Helper.BaseLineSkip(6)));
         tex.Append(Helper.LineFeed());
         var content = (element.Text == null) ? element.Name : _builderRepository.ChunkFor(element.Text);
         if (string.IsNullOrEmpty(element.TableOfContentsTitle) || !element.CreateTableOfContentsEntry)
            tex.Append(Helper.CreateStructureElement(element.Element, content, element.Converter, element.CreateTableOfContentsEntry));
         else
            tex.Append(Helper.CreateStructureElement(element.Element, content, element.Converter, element.TableOfContentsTitle));

         tex.Append(Helper.Label(element.Label));
         tex.Append(Helper.LineFeed());
         return tex.ToString();
      }
   }
}