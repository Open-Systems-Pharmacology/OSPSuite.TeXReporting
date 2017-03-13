using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Tests
{
   public class TextSpecs : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "Text";
         var chapter = new Chapter("Test");
         var text = new Text("This is a text describing {0} and has a reference to {1}.{2}","something", new Reference(chapter), new LineBreak());

         _objectsToReport.Add(chapter);
         _objectsToReport.Add(new Section("Font Styles"));
         _objectsToReport.Add(text);
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add("This is the same in bold style:");
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new Text(text.Content, text.Items.ToArray()) {FontStyle = Text.FontStyles.bold});
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add("This is the same in italic style:");
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new Text(text.Content, text.Items.ToArray()) {FontStyle = Text.FontStyles.italic});
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add("This is the same in slanted style:");
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new Text(text.Content, text.Items.ToArray()) {FontStyle = Text.FontStyles.slanted});

         _objectsToReport.Add(new Section("Aligments"));
         var longtext =
            new Text(
               "This is a very long text to test the aligment options of the text item. The text item can contain a list of text items as well as normal text. All the text is printed in the specified font styles and with the specified aligment. The text should be even longer to make the aligment more visible.");
         _objectsToReport.Add(longtext);

         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add("This is the same in left aligment:");
         _objectsToReport.Add(new Text(longtext.Content) { Alignment = Text.Alignments.flushleft });
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add("This is the same in right aligment:");
         _objectsToReport.Add(new Text(longtext.Content) { Alignment = Text.Alignments.flushright });
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add("This is the same in centered aligment:");
         _objectsToReport.Add(new Text(longtext.Content) { Alignment = Text.Alignments.centered });
         _objectsToReport.Add(new LineBreak());
      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}