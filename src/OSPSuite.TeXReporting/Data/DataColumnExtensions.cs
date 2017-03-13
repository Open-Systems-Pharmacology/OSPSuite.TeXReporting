using System.Data;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Data
{
   public static class DataColumnExtensions
   {
      private const string EXTENDED_PROPERTY_UNIT = "Unit";
      private const string EXTENDED_PROPERTY_ALIGNMENT = "Alignment";
      private const string EXTENDED_PROPERTY_HIDDEN = "Hidden";
      private const string EXTENDED_PROPERTY_NOTES = "Notes";

      public static void SetUnit(this DataColumn column, string unit)
      {
         column.SetExtendedProperty(EXTENDED_PROPERTY_UNIT, unit);
      }

      public static string GetUnit(this DataColumn column)
      {
         return column.GetExtendedProperty<string>(EXTENDED_PROPERTY_UNIT);
      }

      public static void SetAlignment(this DataColumn column, TableWriter.ColumnAlignments alignment)
      {
         column.SetExtendedProperty(EXTENDED_PROPERTY_ALIGNMENT, alignment);
      }

      public static TableWriter.ColumnAlignments? GetAlignment(this DataColumn column)
      {
         return column.GetExtendedProperty<TableWriter.ColumnAlignments?>(EXTENDED_PROPERTY_ALIGNMENT, null);
      }

      public static void SetHidden(this DataColumn column, bool hidden)
      {
         column.SetExtendedProperty(EXTENDED_PROPERTY_HIDDEN, hidden);
      }

      public static bool IsHidden(this DataColumn column)
      {
         return column.GetExtendedProperty<bool>(EXTENDED_PROPERTY_HIDDEN);
      }

      public static DataColumn AsHidden(this DataColumn column)
      {
         column.SetHidden(true);
         return column;
      }
      public static void SetNotes(this DataColumn column, string[] notes)
      {
         column.SetExtendedProperty(EXTENDED_PROPERTY_NOTES, notes);
      }

      public static string[] GetNotes(this DataColumn column)
      {
         return column.GetExtendedProperty(EXTENDED_PROPERTY_NOTES, new string[]{});
      }

      internal static void SetExtendedProperty<T>(this DataColumn column, string extendedProperty, T value)
      {
         if (column.ExtendedProperties.ContainsKey(extendedProperty))
            column.ExtendedProperties[extendedProperty] = value;
         else
            column.ExtendedProperties.Add(extendedProperty, value);
      }

      internal static T GetExtendedProperty<T>(this DataColumn column, string extendedProperty)
      {
         return column.GetExtendedProperty(extendedProperty, default(T));
      }

      internal static T GetExtendedProperty<T>(this DataColumn column, string extendedProperty, T defaultValue)
      {
         if (column.ExtendedProperties.ContainsKey(extendedProperty))
            return column.ExtendedProperties[extendedProperty].DowncastTo<T>();

         return defaultValue;
      }
   }
}