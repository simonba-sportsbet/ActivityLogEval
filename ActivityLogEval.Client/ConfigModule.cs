using System;
using System.Diagnostics;
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
                Debug.WriteLine($" {service.Name} as {intf.Name}");
                builder.RegisterType(service).As(intf);
            }

            foreach(var service in ThisAssembly.GetTypes()
                .Where(x => x.IsClass && typeof(ICmd).IsAssignableFrom(x)))
            {
                Debug.WriteLine($" {service.Name} as ICmd");
                builder.RegisterType(service);
            }
        }

        private (Type service, Type? intf) DefaultConvention(Type service)
        {
            var intf = service.GetInterfaces().FirstOrDefault(x => x.Name == "I" + service.Name);
            return (service, intf);
        }
    }
}
