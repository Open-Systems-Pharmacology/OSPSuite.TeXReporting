namespace OSPSuite.TeXReporting.Items
{
   public class TextBox : BaseItem
   {
      public string Title { get; private set; }
      public Text Content { get; private set; }

      public TextBox(string title, Text content)
      {
         Title = title;
         Content = content;
      }
   }
}
