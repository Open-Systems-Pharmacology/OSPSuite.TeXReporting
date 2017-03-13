using System.Collections.Generic;
using System.Linq;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Builder
{
   public class ListTeXBuilder : TeXChunkBuilder<List>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public ListTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(List list, BuildTracker tracker)
      {
         base.Build(list,tracker);

         tracker.Track(list.Items.SelectMany(items => items.Items));
         tracker.Track(list.Items);
      }

      public override string TeXChunk(List list)
      {
         var texList = new List<string>();
         foreach (var item in list.Items)
            texList.Add(_builderRepository.ChunkFor(item));

         switch (list.ListStyle)
         {
            case List.ListStyles.enumerated:
               return TeX.ListWriter.EnumeratedList(texList.ToArray());
            default:
               return TeX.ListWriter.ItemizedList(texList.ToArray());
         }
      }
   }
}
