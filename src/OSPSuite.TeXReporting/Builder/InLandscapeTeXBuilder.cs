using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Builder
{
   internal class InLandscapeTeXBuilder : TeXBuilder<InLandscape>
   {
      private readonly ITeXBuilderRepository _repository;

      public InLandscapeTeXBuilder(ITeXBuilderRepository repository)
      {
         _repository = repository;
      }

      public override void Build(InLandscape landscape, BuildTracker tracker)
      {
         tracker.TeX.Append(Helper.Begin(Helper.Environments.landscape));
         foreach (var landscapeObject in landscape.Items)
         {
            var objectLandscapeDepending = landscapeObject as ILandscapeDepending;
            if (objectLandscapeDepending != null)
               objectLandscapeDepending.Landscape = true;

            var builder = _repository.BuilderFor(landscapeObject);
            builder.Build(landscapeObject, tracker);
         }
         tracker.TeX.Append(Helper.End(Helper.Environments.landscape));
         tracker.Track(landscape.Items);
      }
   }
}