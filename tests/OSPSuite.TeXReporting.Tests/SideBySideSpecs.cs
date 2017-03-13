using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Tests
{
   public class SideBySideSpecs : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "SideBySide";
         var chapter = new Chapter("Test");
         var text = new Text("This is a text describing {0} and has a reference to {1}.{2}", "something", new Reference(chapter), new LineBreak());

         _objectsToReport.Add(chapter);
         _objectsToReport.Add(text);
         _objectsToReport.Add(new LineBreak());

         _objectsToReport.Add("This is the same, left side in bold style, right side in italic style:");
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new Par());
         var leftSide = new Text(text.Content, text.Items.ToArray()) { FontStyle = Text.FontStyles.bold };
         var rightSide = new Text(text.Content, text.Items.ToArray()) { FontStyle = Text.FontStyles.italic };
         _objectsToReport.Add(new SideBySide(leftSide, rightSide));
         _objectsToReport.Add(new LineBreak());

         _objectsToReport.Add(new Par());
         _objectsToReport.Add("Two lists side by side:");
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new Par());
         var simpleItems = new Text[] { new Text("Item1"), new Text("Item2"), new Text("Item3"), new Text("Item4", new Text("Item5")) };
         var fontStyledItems = new Text[] {new Text("normal Item1") {FontStyle = Text.FontStyles.normal}, 
                                           new Text("bold Item2") {FontStyle = Text.FontStyles.bold}, 
                                           new Text("italic Item3") {FontStyle = Text.FontStyles.italic}, 
                                           new Text("slanted Item4") {FontStyle = Text.FontStyles.slanted},
                                           new Text("{0}{1}", "SubItems", new List(simpleItems))};
         _objectsToReport.Add(new SideBySide(new Text("{0}", new List(simpleItems)), new Text("{0}", new List(fontStyledItems))));

      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}