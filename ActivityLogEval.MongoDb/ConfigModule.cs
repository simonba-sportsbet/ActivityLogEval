using System;
using Autofac;
using ActivityLogEval.Abstractions;
using MongoDB.Driver;

namespace ActivityLogEval.MongoDb
{
    public class ConfigModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DbConnectionFactory>().As<IDbConnectionFactory>().SingleInstance();
            builder.Register(ctx => ctx.Resolve<IDbConnectionFactory>().GetDatabase());

            builder.RegisterType<BetRepo>().As<IBetRepo>();
        }
    }
}
