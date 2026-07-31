using Autofac;
using Demo.Web.Codes;

namespace Demo.Web
{
    public class WebModule : Module
    {
        private readonly string _connectionString;

        public WebModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<ImprovedMembership>().As<IMembership>().InstancePerLifetimeScope()
            //    .WithParameter("name", "asp.net");

            base.Load(builder);
        }
    }
}
