using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Reflection;

namespace OSPSuite.TeXReporting
{
   public class ReportSettings : Notifier
   {
      /// <summary>
      ///    Specifies the author of the report.
      /// </summary>
      /// <remarks>Could be also a company and do not need to be a single person.</remarks>
      public virtual string Author { get; set; }

      /// <summary>
      ///    Specifies the main title of the report.
      /// </summary>
      public virtual string Title { get; set; }

      /// <summary>
      ///    Specifies the subtitle of the report.
      /// </summary>
      public virtual string SubTitle { get; set; }

      /// <summary>
      ///    Optional keywords that will be added to the report
      /// </summary>
      public virtual string[] Keywords { get; set; }

      /// <summary>
      ///    Software used to generate the report. The name should not contain the version number
      /// </summary>
      public virtual string Software { get; set; }

      /// <summary>
      ///    Version of software used to generate the report.
      /// </summary>
      public virtual string SoftwareVersion { get; set; }

      /// <summary>
      ///    Folder containing the template to sue
      /// </summary>
      public virtual string TemplateFolder { get; set; }

      /// <summary>
      ///    Font used in the report
      /// </summary>
      public virtual ReportFont Font { get; set; }

      public virtual string Platform { get; private set; }
      public virtual string ContentFileName { get; set; }
      public virtual bool DeleteWorkingDir { get; set; }

      public virtual bool Draft { get; set; }

      /// <summary>
      ///    Should the report be open when created? Default is true
      /// </summary>
      public virtual bool OpenReport { get; set; }

      /// <summary>
      ///    Specifies of the report artifacts such as table, charts etc should be kept
      /// </summary>
      public virtual bool SaveArtifacts { get; set; }

      /// <summary>
      ///    Should the report be generated as Color or monochrome
      /// </summary>
      public virtual ReportColorStyles ColorStyle { get; set; }

      private string textForReportColor
      {
         get
         {
            switch (ColorStyle)
            {
               case ReportColorStyles.BlackAndWhite:
                  return "monochrome";
               case ReportColorStyles.GrayScale:
                  return "gray";
               default:
                  return "RGB";
            }
         }
      }

      private string textForFont
      {
         get
         {
            switch (Font)
            {
               case ReportFont.Default:
                  return "Default";
               case ReportFont.Helvetica:
                  return "Helvetica";
               case ReportFont.Optima:
                  return "Optima";
               case ReportFont.ComputerModernTeletype:
                  return "ComputerModernTeletype";
               case ReportFont.Courier:
                  return "Courier";
               case ReportFont.Bookman:
                  return "Bookman";
               case ReportFont.Inconsolata:
                  return "Inconsolata";
               case ReportFont.LatinModern:
                  return "LatinModern";
               default:
                  return "Default";
            }
         }
      }
      /// <summary>
      ///    Number of compilation processes that will be started when compiling the report to PDF. Default is 3
      /// </summary>
      public virtual int NumberOfCompilations { get; set; }

      public ReportSettings()
      {
         Platform = Environment.OSVersion.ToString();
         DeleteWorkingDir = true;
         NumberOfCompilations = 3;
         Keywords = new string[] {};
         ColorStyle = ReportColorStyles.Color;
         OpenReport = true;
         Draft = false;
         Font = ReportFont.Default;
      }

      /// <summary>
      ///    Checks the settings and throws an exception if something is wrong.
      ///    Note: ContentFileName is the only property that will be set during the compiling process
      ///    and should not be changed
      /// </summary>
      public virtual void Validate()
      {
         if (string.IsNullOrEmpty(Author))
            throw new OSPSuiteException("Author is missing.");

         if (string.IsNullOrEmpty(Title))
            throw new OSPSuiteException("Title is missing.");

         if (string.IsNullOrEmpty(SubTitle))
            throw new OSPSuiteException("Subtitle is missing.");

         if (string.IsNullOrEmpty(Software))
            throw new OSPSuiteException("Software is missing.");

         if (string.IsNullOrEmpty(SoftwareVersion))
            throw new OSPSuiteException("Version is missing.");

         if (string.IsNullOrEmpty(TemplateFolder))
            throw new OSPSuiteException("The template folder name is missing.");

         if (String.IsNullOrEmpty(ContentFileName))
            throw new OSPSuiteException("The Name of the content file is empty. Please Specifies a content file name with the .tex extension");

         var folder = new DirectoryInfo(TemplateFolder);
         if (!folder.Exists)
            throw new OSPSuiteException($"The template folder '{folder.FullName}' does not exists. Please Specifies an existing template folder.");

         var templateFullPath = Path.Combine(TemplateFolder, TemplateName);
         if (!FileHelper.FileExists(templateFullPath))
            throw new OSPSuiteException($"The template folder '{folder.FullName}' does not contain the expected layout tex file '{TemplateName}'");
      }

      public string TemplateName => $"{new DirectoryInfo(TemplateFolder).Name}.tex";

      public string TemplateFullPath => Path.Combine(TemplateFolder, TemplateName);

      public enum ReportColorStyles
      {
         Color,
         GrayScale,
         BlackAndWhite
      }

      public enum ReportFont
      {
         Default,
         Helvetica,
         Optima,
         ComputerModernTeletype,
         Courier,
         Bookman,
         Inconsolata,
         LatinModern
      }

      /// <summary>
      ///    Replaces the setting tokens in the given template.
      /// </summary>
      public void Implement(string texFile)
      {
         var fileContents = File.ReadAllText(texFile);
         var converter = DefaultConverter.Instance;
         var keywordList = new StringBuilder();
         foreach (var keyword in Keywords)
         {
            if (keywordList.Length > 0) keywordList.Append("; ");
            keywordList.AppendFormat("{{{0}}}", converter.StringToTeX(keyword));
         }

         var replacements = new Dictionary<string, string>
         {
            {"@AUTHOR", converter.StringToTeX(Author)},
            {"@TITLE", converter.StringToTeX(Title)},
            {"@SUBTITLE", converter.StringToTeX(SubTitle)},
            {"@KEYWORDS", keywordList.ToString()},
            {"@CONTENTFILE", converter.FilePathToTeX(Path.Combine(".", ContentFileName))},
            {"@SOFTWAREVERSION", converter.StringToTeX(SoftwareVersion)},
            {"@SOFTWARE", converter.StringToTeX(Software)},
            {"@PLATFORM", converter.StringToTeX(Platform)},
            {"@COLORSTYLE", textForReportColor},
            {"@DRAFT", Draft ? "stamp" : "nostamp"},
            {"@FONT", textForFont}
         };

         foreach (var replacement in replacements)
            fileContents = fileContents.Replace(replacement.Key, replacement.Value);

         File.WriteAllText(texFile, fileContents);
      }
   }
}