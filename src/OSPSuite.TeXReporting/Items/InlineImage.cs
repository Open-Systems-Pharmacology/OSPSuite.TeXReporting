namespace OSPSuite.TeXReporting.Items
{
   public class InlineImage : Attachement
   {
      public static readonly string SubFolder = "Inline Images";

      public InlineImage(string fullPath): base(fullPath, SubFolder)
      {
      }
   }
}