using System.Globalization;
using System.Text;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.TeX.PGFPlots
{
   public class BoxPlotPrepared
   {
      public BoxPlotPrepared Clone()
      {
         return MemberwiseClone().DowncastTo<BoxPlotPrepared>();
      }

      public readonly float? DrawPosition;

      public readonly float LowerWhisker;
      public readonly float LowerQuartile;
      public readonly float Median;
      public readonly float UpperQuartile;
      public readonly float UpperWhisker;

      public BoxPlotPrepared(float lowerWhisker, float lowerQuartile, float median, float upperQuartile, float upperWhisker, float? drawPosition = null)
      {
         LowerWhisker = lowerWhisker;
         LowerQuartile = lowerQuartile;
         Median = median;
         UpperQuartile = upperQuartile;
         UpperWhisker = upperWhisker;
         DrawPosition = drawPosition;
      }

      public override string ToString()
      {
         const string FORMAT_STRING = "{0}={1}, ";

         var text = new StringBuilder();
         
         text.Append("boxplot prepared={");

         if (DrawPosition != null)
         {
            text.AppendFormat(FORMAT_STRING, "draw position", ((float)DrawPosition).ToString(CultureInfo.InvariantCulture));
         }

         text.AppendFormat(FORMAT_STRING, "lower whisker", LowerWhisker.ToString(CultureInfo.InvariantCulture));
         text.AppendFormat(FORMAT_STRING, "lower quartile", LowerQuartile.ToString(CultureInfo.InvariantCulture));
         text.AppendFormat(FORMAT_STRING, "median", Median.ToString(CultureInfo.InvariantCulture));
         text.AppendFormat(FORMAT_STRING, "upper quartile", UpperQuartile.ToString(CultureInfo.InvariantCulture));
         text.AppendFormat(FORMAT_STRING, "upper whisker", UpperWhisker.ToString(CultureInfo.InvariantCulture));

         text.Append("}");

         return text.ToString();
      }
   }
}
