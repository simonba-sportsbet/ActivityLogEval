using System;
using System.Linq;
using Autofac;

namespace ActivityLogEval.Client
{
    public class ConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            foreach(var (service, intf) in ThisAssembly.GetTypes()
                .Where(x => x.IsClass)
                .Select(DefaultConvention)
                .Where(x => x.intf != null))
            {
                builder.RegisterType(service).As(intf);
            }
        }

        private (Type service, Type? intf) DefaultConvention(Type service)
        {
            var intf = service.GetInterfaces().FirstOrDefault(x => x.Name == "I" + service.Name);
            return (service, intf);
        }
    }
}
