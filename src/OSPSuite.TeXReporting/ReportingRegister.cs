using System;
using System.Linq;
using OSPSuite.TeXReporting.Builder;
using OSPSuite.TeXReporting.TeX;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Container.Conventions;
using OSPSuite.Utility.Extensions;

namespace OSPSuite.TeXReporting
{
   public class ReportingRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            //Register Builder with a special convention
            scan.AssemblyContainingType<ReportingRegister>();
            scan.IncludeNamespaceContainingType<ITeXBuilder>();
            scan.ExcludeType<TeXBuilderRepository>();
            scan.WithConvention<RegisterBuilderConvention>();
         });

         container.Register<IReportCreator, ReportCreator>();
         container.Register<IReportCompiler, ReportCompiler>();
         container.Register<IImageConverter, ImageConverter>();
         container.Register<ITeXCompiler, TeXCompiler>();
         container.Register<IArtifactsManager, ArtifactsManager>();
         container.Register<IBuildTrackerFactory, BuildTrackerFactory>();
         container.Register<ITeXBuilderRepository, TeXBuilderRepository>(LifeStyle.Singleton);
      }
   }

   /// <summary>
   ///    use this convetion ro register your builder implementing the <see cref="ITeXBuilder" />
   /// </summary>
   public class RegisterBuilderConvention : DefaultRegistrationConvention
   {
      public override void Process(Type concreteType, IContainer container, LifeStyle lifeStyle)
      {
         var texBuilderType = typeof(ITeXBuilder);
         if (concreteType.IsAnImplementationOf(texBuilderType))
         {
            var interfaces = concreteType.GetInterfaces().Where(t => t.Name.StartsWith(texBuilderType.Name)).ToList();
            container.Register(interfaces,concreteType, lifeStyle);
         }
         else
            base.Process(concreteType, container, lifeStyle);
      }
   }
}