using Autofac;
using BetterJWall.Common;
using BetterJWall.JIRABridge.Client;
using BetterJWall.JIRABridge.Services.Issues;

namespace BetterJWall.Web.Resolver
{
    public class GenericModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // No assembly scanning here, register all
            // dependencies manually
            builder.RegisterType<ConfigurationHelper>().As<IConfigurationHelper>();
            builder.RegisterType<JiraClient>().As<IJiraClient>();
            builder.RegisterType<JiraIssueService>().As<IJiraIssueService>();
            builder.RegisterType<DateTimeFunctions>().As<IDateTimeFunctions>();

            base.Load(builder);
        }
    }
}
