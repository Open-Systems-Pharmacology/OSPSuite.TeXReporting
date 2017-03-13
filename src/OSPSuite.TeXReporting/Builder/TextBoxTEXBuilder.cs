using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Builder
{
   public class TextBoxTeXBuilder :TeXBuilder<TextBox>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public TextBoxTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(TextBox objectToReport, BuildTracker tracker)
      {
         tracker.TeX.Append(TeX.Helper.TextBox(objectToReport.Converter.StringToTeX(objectToReport.Title), _builderRepository.ChunkFor(objectToReport.Content)));
      }
   }
}
