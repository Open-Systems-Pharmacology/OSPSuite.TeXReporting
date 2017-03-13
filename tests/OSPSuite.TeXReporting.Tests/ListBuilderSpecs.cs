using System;
using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Tests
{
   public class ListBuilderSpecs : ContextForReporting
   {
      protected override void Context()
      {
         base.Context();
         _pdfFileName = "List";

         var chapter = new Chapter("Test");
         var stringItems = new string[] {"Item1", "Item2", "Item3", "Item4"};
         var simpleItems = new Text[] {new Text("Item1"), new Text("Item2"), new Text("Item3"), new Text("Item4")};
         var fontStyledItems = new Text[] {new Text("normal Item1") {FontStyle = Text.FontStyles.normal}, 
                                           new Text("bold Item2") {FontStyle = Text.FontStyles.bold}, 
                                           new Text("italic Item3") {FontStyle = Text.FontStyles.italic}, 
                                           new Text("slanted Item4") {FontStyle = Text.FontStyles.slanted}};
         var figurepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "figure.png");
         var imagedItems = new Text[] {new Text("Item1 with image {0}.", new InlineImage(figurepath)),
                                       new Text("Item2 with image {0}.", new InlineImage(figurepath))};
         
         _objectsToReport.Add(chapter);
         _objectsToReport.Add(new Text("This is a very simple itemized list of strings:{0}", new LineBreak()));
         _objectsToReport.Add(stringItems);
         //simple list
         _objectsToReport.Add(new Par());
         _objectsToReport.Add(new Text("This is a very simple itemized list:{0}", new LineBreak()));
         _objectsToReport.Add(new List(simpleItems));
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new Text("This is a very simple enumerated list:{0}", new LineBreak()));
         _objectsToReport.Add(new List(simpleItems, List.ListStyles.enumerated));
         //font styled
         _objectsToReport.Add(new Par());
         _objectsToReport.Add(new Text("This is a font styled itemized list:{0}", new LineBreak()));
         _objectsToReport.Add(new List(fontStyledItems));
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new Text("This is a font styled enumerated list:{0}", new LineBreak()));
         _objectsToReport.Add(new List(fontStyledItems, List.ListStyles.enumerated));
         //imaged list
         _objectsToReport.Add(new Par());
         _objectsToReport.Add(new Text("This is a itemized list with images:{0}", new LineBreak()));
         _objectsToReport.Add(new List(imagedItems));
         _objectsToReport.Add(new LineBreak());
         _objectsToReport.Add(new Text("This is an enumerated list with images:{0}", new LineBreak()));
         _objectsToReport.Add(new List(imagedItems, List.ListStyles.enumerated));

         //list in list
         _objectsToReport.Add(new Par());
         _objectsToReport.Add(new Text("This is an itemized list with multiple levels:{0}", new LineBreak()));
         var multipleLevels = new Text[]
                                 {
                                    new Text("{0}{1}", new Text("simple items"), new List(simpleItems)), 
                                    new Text("{0}{1}", new Text("font styled items"), new List(fontStyledItems)),
                                    new Text("{0}{1}", "imaged items", new List(imagedItems)),
                                    new Text("{0}{1}", "imaged items enumerated", new List(imagedItems, List.ListStyles.enumerated))
                                 };
         _objectsToReport.Add(new List(multipleLevels));


      }

      [Observation]
      public void should_create_a_pdf_containing_the_report()
      {
         VerifyThatFileExists();
      }
   }
}