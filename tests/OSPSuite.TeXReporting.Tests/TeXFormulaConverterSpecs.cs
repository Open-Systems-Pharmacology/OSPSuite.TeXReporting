using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.TeXReporting.TeX.Converter;

namespace OSPSuite.TeXReporting.Tests
{
   public abstract class concern_for_TexFormulaConverterSpecs : ContextSpecification<ITeXConverter>
   {
      protected override void Context()
      {
         sut = FormulaConverter.Instance;
      }
   }

   public class When_converting_a_string_containing_invalid_characters_to_a_tex_representation : concern_for_TexFormulaConverterSpecs
   {
      [Observation]
      public void should_remove_the_invalid_characters()
      {

         sut.StringToTeX("Test#Test").ShouldBeEqualTo("Test\\#Test");
         sut.StringToTeX("Test$Test").ShouldBeEqualTo("Test\\$Test");
         sut.StringToTeX("Test%Test").ShouldBeEqualTo("Test\\%Test");
         sut.StringToTeX("Test&Test").ShouldBeEqualTo("Test\\&Test");
         sut.StringToTeX("Test~Test").ShouldBeEqualTo("Test{\\raise.17ex\\hbox{$\\scriptstyle\\sim$}}Test");
         sut.StringToTeX("Test_Test").ShouldBeEqualTo("Test\\_\\-Test");
         sut.StringToTeX("Test^Test").ShouldBeEqualTo("Test\\textasciicircum Test");
         sut.StringToTeX("Test\\Test").ShouldBeEqualTo("Test\\textbackslash Test");
         sut.StringToTeX("Test{Test").ShouldBeEqualTo("Test\\{Test");
         sut.StringToTeX("Test}Test").ShouldBeEqualTo("Test\\}Test");
         sut.StringToTeX("Test[Test").ShouldBeEqualTo("Test{[}Test");
         sut.StringToTeX("Test]Test").ShouldBeEqualTo("Test{]}Test");
         sut.StringToTeX("Test<Test").ShouldBeEqualTo("Test\\textless Test");
         sut.StringToTeX("Test>Test").ShouldBeEqualTo("Test\\textgreater Test");
         sut.StringToTeX("Test|Test").ShouldBeEqualTo("Test\\textbar \\-Test");
         sut.StringToTeX("TestäTest").ShouldBeEqualTo("Test\\\"aTest");
         sut.StringToTeX("TestöTest").ShouldBeEqualTo("Test\\\"oTest");
         sut.StringToTeX("TestüTest").ShouldBeEqualTo("Test\\\"uTest");
         sut.StringToTeX("TestÄTest").ShouldBeEqualTo("Test\\\"ATest");
         sut.StringToTeX("TestÖTest").ShouldBeEqualTo("Test\\\"OTest");
         sut.StringToTeX("TestÜTest").ShouldBeEqualTo("Test\\\"UTest");
         sut.StringToTeX("Test\nTest").ShouldBeEqualTo("Test\\newline Test");
         sut.StringToTeX("Test®Test").ShouldBeEqualTo("Test\\textsuperscript{\\textregistered}Test");
         sut.StringToTeX("TestµTest").ShouldBeEqualTo("Test\\textmu Test");

         sut.StringToTeX("Test+Test").ShouldBeEqualTo("Test + Test");
         sut.StringToTeX("Test+ Test").ShouldBeEqualTo("Test + Test");
         sut.StringToTeX("Test + Test").ShouldBeEqualTo("Test + Test");
         sut.StringToTeX("Test +  Test").ShouldBeEqualTo("Test + Test");
         sut.StringToTeX("Test  +  Test").ShouldBeEqualTo("Test + Test");

         sut.StringToTeX("Test-Test").ShouldBeEqualTo("Test - Test");
         sut.StringToTeX("Test- Test").ShouldBeEqualTo("Test - Test");
         sut.StringToTeX("Test - Test").ShouldBeEqualTo("Test - Test");
         sut.StringToTeX("Test -  Test").ShouldBeEqualTo("Test - Test");
         sut.StringToTeX("Test  -  Test").ShouldBeEqualTo("Test - Test");

         sut.StringToTeX("Test*Test").ShouldBeEqualTo("Test * Test");
         sut.StringToTeX("Test* Test").ShouldBeEqualTo("Test * Test");
         sut.StringToTeX("Test * Test").ShouldBeEqualTo("Test * Test");
         sut.StringToTeX("Test *  Test").ShouldBeEqualTo("Test * Test");
         sut.StringToTeX("Test  *  Test").ShouldBeEqualTo("Test * Test");

         sut.StringToTeX("Test/Test").ShouldBeEqualTo("Test / Test");
         sut.StringToTeX("Test/ Test").ShouldBeEqualTo("Test / Test");
         sut.StringToTeX("Test / Test").ShouldBeEqualTo("Test / Test");
         sut.StringToTeX("Test /  Test").ShouldBeEqualTo("Test / Test");
         sut.StringToTeX("Test  /  Test").ShouldBeEqualTo("Test / Test");

      }
   }

   public class When_converting_a_string_array_containing_invalid_characters_to_a_tex_representation : concern_for_TexFormulaConverterSpecs
   {
      [Observation]
      public void should_remove_the_invalid_characters()
      {
         var testArray = new string[]
                            {
                               "Test#Test",
                               "Test$Test",
                               "Test%Test",
                               "Test&Test",
                               "Test~Test",
                               "Test_Test",
                               "Test^Test",
                               "Test\\Test",
                               "Test{Test",
                               "Test}Test",
                               "Test[Test",
                               "Test]Test",
                               "Test<Test",
                               "Test>Test",
                               "Test|Test",
                               "TestäTest",
                               "TestöTest",
                               "TestüTest",
                               "TestÄTest",
                               "TestÖTest",
                               "TestÜTest",
                               "Test\nTest",
                               "Test®Test",
                               "TestµTest",
                               "Test+Test",
                               "Test+ Test",
                               "Test + Test",
                               "Test +  Test",
                               "Test  +  Test",
                               "Test-Test",
                               "Test- Test",
                               "Test - Test",
                               "Test -  Test",
                               "Test  -  Test",
                               "Test*Test",
                               "Test* Test",
                               "Test * Test",
                               "Test *  Test",
                               "Test  *  Test",
                               "Test/Test",
                               "Test/ Test",
                               "Test / Test",
                               "Test /  Test",
                               "Test  /  Test"
                            };
         var expectedArray = new string[]
                                {
                                   "Test\\#Test",
                                   "Test\\$Test",
                                   "Test\\%Test",
                                   "Test\\&Test",
                                   "Test{\\raise.17ex\\hbox{$\\scriptstyle\\sim$}}Test",
                                   "Test\\_\\-Test",
                                   "Test\\textasciicircum Test",
                                   "Test\\textbackslash Test",
                                   "Test\\{Test",
                                   "Test\\}Test",
                                   "Test{[}Test",
                                   "Test{]}Test",
                                   "Test\\textless Test",
                                   "Test\\textgreater Test",
                                   "Test\\textbar \\-Test",
                                   "Test\\\"aTest",
                                   "Test\\\"oTest",
                                   "Test\\\"uTest",
                                   "Test\\\"ATest",
                                   "Test\\\"OTest",
                                   "Test\\\"UTest",
                                   "Test\\newline Test",
                                   "Test\\textsuperscript{\\textregistered}Test",
                                   "Test\\textmu Test",
                                   "Test + Test",
                                   "Test + Test",
                                   "Test + Test",
                                   "Test + Test",
                                   "Test + Test",
                                   "Test - Test",
                                   "Test - Test",
                                   "Test - Test",
                                   "Test - Test",
                                   "Test - Test",
                                   "Test * Test",
                                   "Test * Test",
                                   "Test * Test",
                                   "Test * Test",
                                   "Test * Test",
                                   "Test / Test",
                                   "Test / Test",
                                   "Test / Test",
                                   "Test / Test",
                                   "Test / Test"
                                };
         sut.StringToTeX(testArray).ShouldOnlyContainInOrder(expectedArray);
      }
   }
}