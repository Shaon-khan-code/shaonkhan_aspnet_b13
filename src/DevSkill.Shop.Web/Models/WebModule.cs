using Autofac;

namespace DevSkill.Shop.Web.Models
{
    public class WebModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register services here
            //builder.RegisterType<SomeService>()
            //       .As<ISomeService>()
            //       .InstancePerLifetimeScope();
        }
    }
}
