using System.IO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.TeXReporting.TeX.Converter;
using OSPSuite.Utility;
using OSPSuite.Utility.Exceptions;

namespace OSPSuite.TeXReporting.Tests
{
   public abstract class concern_for_ReportSettings : ContextSpecification<ReportSettings>
   {
      protected string _workingTex;
      protected ITeXConverter _converter;

      protected override void Context()
      {
         _converter = DefaultConverter.Instance;
         sut = new ReportSettings
         {
            Title = "A GREAT TITLE",
            SubTitle = "A great report",
            Author = "ZTMSE",
            Keywords = new [] {"TEST"},
            Software = "SBSuite",
            SoftwareVersion = "2.5",
            ContentFileName = "myContent",
            ColorStyle = ReportSettings.ReportColorStyles.BlackAndWhite,
            TemplateFolder = ConstantsForSpecs.StandardTemplateFolder
         };
         _workingTex = FileHelper.GenerateTemporaryFileName() + ".tex";
         File.Copy(sut.TemplateFullPath,_workingTex);

      }

      public override void Cleanup()
      {
         base.Cleanup();
         FileHelper.DeleteFile(_workingTex);
      }
   }

   public class When_validating_some_invalid_settings : concern_for_ReportSettings
   {
      [Observation]
      public void should_throw_an_invalid_setting_exception()
      {
         The.Action(() => new ReportSettings().Validate()).ShouldThrowAn<OSPSuiteException>();
      }
   }


   public class When_validating_some_with_valid_settings : concern_for_ReportSettings
   {
      [Observation]
      public void should_be_able_to_validate_the_settings()
      {
         sut.Validate();
      }
   }

   public class When_creating_a_file_with_some_valid_settings : concern_for_ReportSettings
   {
      protected override void Because()
      {
         sut.Implement(_workingTex);
      }

      [Observation]
      public void should_have_replaced_the_author_token()
      {
         fileShouldContain(_converter.StringToTeX(sut.Author), _workingTex);
      }

      [Observation]
      public void should_have_replaced_the_keywords_token()
      {
         foreach (var keyword in sut.Keywords)
            fileShouldContain(_converter.StringToTeX(keyword), _workingTex);
      }

      [Observation]
      public void should_have_replaced_the_software_token()
      {
         fileShouldContain(_converter.StringToTeX(sut.Software), _workingTex);
      }
      [Observation]
      public void should_have_replaced_the_software_version_token()
      {
         fileShouldContain(_converter.StringToTeX(sut.SoftwareVersion), _workingTex);
      }

      [Observation]
      public void should_have_replaced_the_color()
      {
         fileShouldContain("monochrome", _workingTex);
      }

      [Observation]
      public void should_have_replaced_the_subtile_token()
      {
         fileShouldContain(_converter.StringToTeX(sut.SubTitle), _workingTex);
      }

      [Observation]
      public void should_have_replaced_the_title_token()
      {
         fileShouldContain(_converter.StringToTeX(sut.Title), _workingTex);
      }

      [Observation]
      public void should_have_replaced_the_content_token()
      {
         fileShouldContain(_converter.FilePathToTeX(sut.ContentFileName), _workingTex);
      }

      [Observation]
      public void should_have_replaced_the_platform_token()
      {
         fileShouldContain(_converter.StringToTeX(sut.Platform), _workingTex);
      }

      private void fileShouldContain(string token, string file)
      {
         var content = File.ReadAllText(file);
         content.Contains(token).ShouldBeTrue(string.Format("Token {0} not found", token));
      }
   }
}	