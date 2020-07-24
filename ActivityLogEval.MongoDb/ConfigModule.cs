using System;
using Autofac;
using ActivityLogEval.Abstractions;

namespace ActivityLogEval.MongoDb
{
    public class ConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DbConnectionFactory>().As<IDbConnectionFactory>().SingleInstance();
            builder.RegisterType<MongoDbRepo>().As<IRepo>();
        }
    }
}
