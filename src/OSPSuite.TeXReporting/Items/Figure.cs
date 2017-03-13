using System.IO;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.TeX;

namespace OSPSuite.TeXReporting.Items
{
   public class Figure : Attachement, ICaptionItem, IReferenceable, ILandscapeDepending
   {
      public string Label { get; private set; }
      public Text Caption { get; private set; }
      public bool Landscape { get; set; }
      public FigureWriter.FigurePositions Position { get; set; }

      private const string _subFolder = "figures";

      public Figure(Text caption, string fullPath) : base(fullPath, _subFolder)
      {
         Position = FigureWriter.FigurePositions.H;
         Caption = caption;
         Label = Helper.Marker();
         Landscape = false;
      }

      public Figure(string caption, string fullPath) : this(new Text(caption), fullPath)
      {
      }

      public static Figure ForCreation(string caption, string fileName, BuildTracker tracker)
      {
         var figure = new Figure(caption, string.Empty);

         var figureFolder = new DirectoryInfo(Path.Combine(tracker.WorkingDirectory, _subFolder));
         if (!figureFolder.Exists)
            figureFolder.Create();

         figure.FullPath = Path.Combine(figureFolder.FullName, fileName);
         return figure;     
      }
   }
}