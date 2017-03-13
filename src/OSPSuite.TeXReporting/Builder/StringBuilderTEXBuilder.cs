using System.Text;

namespace OSPSuite.TeXReporting.Builder
{
   internal class StringBuilderTeXBuilder : TeXBuilder<StringBuilder>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public StringBuilderTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(StringBuilder sb, BuildTracker tracker)
      {
         _builderRepository.Report(sb.ToString(),tracker);
      }
   }
}