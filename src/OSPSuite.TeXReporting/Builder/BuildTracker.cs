using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Builder
{
   public class BuildTracker
   {
      /// <summary>
      ///    Returns the directory where all artifacts should be copied
      /// </summary>
      public string WorkingDirectory { get; set; }

      /// <summary>
      ///    Report full path with extension
      /// </summary>
      public string ReportFullPath { get; internal set; }

      /// <summary>
      ///    Name of the pdf file that should created (without extension)
      /// </summary>
      public string ReportFileName { get; internal set; }

      /// <summary>
      ///    Name of the pdf file that should created (without extension)
      /// </summary>
      public string ReportFolder { get; internal set; }

      /// <summary>
      ///    set containing all objects reported during the build process
      /// </summary>
      private readonly HashSet<object> _allReportedObjects;

      private readonly ICache<object, Reference> _references;

      /// <summary>
      ///    The string builder containing the actual TEX being generated
      /// </summary>
      public StringBuilder TeX { get; private set; }

      /// <summary>
      ///    Tracks the reference to the given
      ///    <paramref name="objectToReference"/> 
      /// </summary>
      /// <param name="objectToReference">Object for which a reference should be stored</param>
      /// <param name="referenceToObject">Reference of the given object</param>
      public void AddReference(object objectToReference, IReferenceable referenceToObject)
      {
         if (_references.Contains(objectToReference)) return;
         _references.Add(objectToReference, new Reference(referenceToObject));
      }

      /// <summary>
      ///    Returns the reference cached for the given object.
      ///    Returns null if the reference was not found for the given object
      /// </summary>
      /// <returns></returns>
      public Reference ReferenceFor(object objectWithReference)
      {
         return _references[objectWithReference];
      }

      public BuildTracker()
      {
         TeX = new StringBuilder();
         _allReportedObjects = new HashSet<object>();
         _references = new Cache<object, Reference>(onMissingKey: x => null);
      }

      /// <summary>
      ///    Track the given objects as being part of the report creation process
      /// </summary>
      public virtual void Track(params object[] objects)
      {
         Track(new ReadOnlyCollection<object>(objects));
      }

      /// <summary>
      ///    Track the given objects as being part of the report creation process
      /// </summary>
      public virtual void Track(IEnumerable<object> objectsToTrack)
      {
         objectsToTrack.Each(item => _allReportedObjects.Add(item));
      }

      private StructureElement getStructureElementFor(Helper.StructureElements structureElement, string name)
      {
         switch (structureElement)
         {
            case Helper.StructureElements.part:
               return new Part(name);
            case Helper.StructureElements.chapter:
               return new Chapter(name);
            case Helper.StructureElements.section:
               return new Section(name);
            case Helper.StructureElements.subsection:
               return  new SubSection(name);
            case Helper.StructureElements.subsubsection:
               return new SubSubSection(name);
            case Helper.StructureElements.paragraph:
               return new Paragraph(name);
            case Helper.StructureElements.subparagraph:
               return new SubParagraph(name);
            default:
               throw new ArgumentOutOfRangeException();
         }
      }

      /// <summary>
      /// Gets a struture element relative to last added structure element.
      /// </summary>
      /// <param name="name">Title of the new structure element.</param>
      /// <param name="structureOffset">With negative value you go up and with positive values you go down.</param>
      /// <returns>Structure element relative to last added one.</returns>
      public virtual StructureElement GetStructureElementRelativeToLast(string name, int structureOffset = 0)
      {
         var structureElement = GetLastInsertedStructureElementEnumValue() + structureOffset;
         return getStructureElementFor(structureElement, name);
      }

      /// <summary>
      /// Gives the last inserted structure elements enumeration value.
      /// </summary>
      /// <returns></returns>
      public virtual Helper.StructureElements GetLastInsertedStructureElementEnumValue()
      {
         var structureElement = getLastInsertedStructureElement();
         return structureElement == null ? Helper.StructureElements.part : structureElement.Element;
      }

      private StructureElement getLastInsertedStructureElement()
      {
         return (StructureElement)_allReportedObjects.LastOrDefault(x => x.IsAnImplementationOf(typeof(StructureElement)));
      }

      /// <summary>
      ///    Returns the objects tracked during the report creation process
      /// </summary>
      public virtual IEnumerable<object> TrackedObjects
      {
         get { return _allReportedObjects; }
      }

      public virtual void DeleteWorkingDirectory()
      {
         var dir = new DirectoryInfo(WorkingDirectory);
         if (dir.Exists)
            try
            {
               dir.Delete(true);
            }
            catch (IOException)
            {
               Thread.Sleep(0);
               try
               {
                  dir.Delete(true);
               }
               catch (IOException)
               {
               }
            }
      }
   }
}