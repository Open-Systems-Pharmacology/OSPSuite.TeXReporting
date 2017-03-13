namespace OSPSuite.TeXReporting.Items
{
   public class SideBySide : BaseItem
   {
      public Text LeftSide { get; private set; }
      public Text RightSide { get; private set; }

      public SideBySide(Text leftSide, Text rightSide)
      {
         LeftSide = leftSide;
         RightSide = rightSide;
      }
   }
}
