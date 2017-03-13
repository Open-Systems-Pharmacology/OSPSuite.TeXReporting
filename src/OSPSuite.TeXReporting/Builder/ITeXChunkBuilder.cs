using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Builder
{
   /// <summary>
   ///    Represents a builder capable of building an object into its TEX representation.
   /// </summary>
   public interface ITeXChunkBuilder : ITeXBuilder
   {
      /// <summary>
      ///    Builds the TEX representation of given item.
      /// </summary>
      string TeXChunk(object item);
   }

   public interface ITeXChunkBuilder<T> : ITeXChunkBuilder
   {
      string TeXChunk(T item);
   }

   public abstract class TeXChunkBuilder<T> : TeXBuilder<T>, ITeXChunkBuilder<T>
   {
      public abstract string TeXChunk(T item);

      public string TeXChunk(object item)
      {
         return TeXChunk(item.DowncastTo<T>());
      }

      public override void Build(T objectToReport, BuildTracker tracker)
      {
         //default implementation
         tracker.TeX.Append(TeXChunk(objectToReport));
      }
   }
}