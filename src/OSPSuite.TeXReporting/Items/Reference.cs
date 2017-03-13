namespace OSPSuite.TeXReporting.Items
{
   public class Reference
   {
      public IReferenceable Referenceable { get; private set; }

      public Reference(IReferenceable referenceable)
      {
         Referenceable = referenceable;
      }
   }
}