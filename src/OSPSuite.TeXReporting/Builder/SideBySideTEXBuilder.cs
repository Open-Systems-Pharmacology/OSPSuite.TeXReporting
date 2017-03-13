using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Builder
{
   public class SideBySideTeXBuilder : TeXChunkBuilder<SideBySide>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public SideBySideTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override string TeXChunk(SideBySide sideBySide)
      {
         return TeX.Helper.SideBySide(_builderRepository.ChunkFor(sideBySide.LeftSide),
                                      _builderRepository.ChunkFor(sideBySide.RightSide));
      }
   }
}
