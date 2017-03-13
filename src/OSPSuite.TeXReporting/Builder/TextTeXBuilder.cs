using System.Collections.Generic;
using System.Text;
using OSPSuite.TeXReporting.Items;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting.Builder
{
   internal class TextTeXBuilder : TeXChunkBuilder<Text>
   {
      private readonly ITeXBuilderRepository _builderRepository;

      public TextTeXBuilder(ITeXBuilderRepository builderRepository)
      {
         _builderRepository = builderRepository;
      }

      public override void Build(Text text, BuildTracker tracker)
      {
         base.Build(text,tracker);
         tracker.Track(text.Items);
      }

      public override string TeXChunk(Text text)
      {
         if (string.IsNullOrEmpty(text.Content))
            return string.Empty;

         string newText;
         if (text.Items.Count > 0)
         {
            newText = text.Content;
            for (var i = 0; i < text.Items.Count; i++)
               newText = newText.Replace(string.Format("{{{0}}}", i), string.Format("@{0}@", i));

            newText = text.Converter.StringToTeX(newText);
            
            for (var i = 0; i < text.Items.Count; i++)
               newText = newText.Replace(string.Format("@{0}@", i), string.Format("{{{0}}}", i));

            var texChunks = new List<object>();
            text.Items.Each(item=>texChunks.Add(_builderRepository.ChunkFor(item)));

            newText = string.Format(newText, texChunks.ToArray());
         }
         else
         {
            newText = text.Converter.StringToTeX(text.Content);
         }

         return alignedText(text.Alignment, styledText(text.FontStyle, newText));
      }
      
      private string alignedText(Text.Alignments alignment, string text)
      {
         var tex = new StringBuilder();
         const string FORMAT = "{0}{1}{2}";
         switch (alignment)
         {
            case Text.Alignments.centered:
               tex.AppendFormat(FORMAT, Helper.Begin(Helper.Environments.center), text,
                                Helper.End(Helper.Environments.center));
               return tex.ToString();
            case Text.Alignments.flushleft:
               tex.AppendFormat(FORMAT, Helper.Begin(Helper.Environments.flushleft), text,
                                Helper.End(Helper.Environments.flushleft));
               return tex.ToString();
            case Text.Alignments.flushright:
               tex.AppendFormat(FORMAT, Helper.Begin(Helper.Environments.flushright), text,
                                Helper.End(Helper.Environments.flushright));
               return tex.ToString();
            default:
               return text;
         }
      }


      private string styledText(Text.FontStyles fontStyle, string text)
      {
         switch (fontStyle)
         {
            case Text.FontStyles.bold:
               return Helper.Bold(text);
            case Text.FontStyles.italic:
               return Helper.Italic(text);
            case Text.FontStyles.slanted:
               return Helper.Slanted(text);
            default:
               return text;
         }
      }
   }
}