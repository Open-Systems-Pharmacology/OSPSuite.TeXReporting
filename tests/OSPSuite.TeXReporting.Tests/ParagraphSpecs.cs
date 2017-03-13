using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.TeXReporting.Items;

namespace OSPSuite.TeXReporting.Tests
{
   public class When_creating_a_paragraph_ending_with_a_line_break : ContextSpecification<Paragraph>
   {
      [Observation]
      public void should_not_add_a_line_break()
      {
         new Paragraph("toto\n").Name.ShouldBeEqualTo("toto\n");
      }
   }

   public class When_creating_a_paragraph_that_does_not_end_with_a_line_break : ContextSpecification<Paragraph>
   {
      [Observation]
      public void should_add_a_line_break()
      {
         new Paragraph("toto").Name.ShouldBeEqualTo("toto\n");
      }
   }
}