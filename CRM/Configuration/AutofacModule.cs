using Autofac;
using CRM.API.Controllers;
using CRM.DB.Storages;
using CrmRepository.Repositories;

namespace CRM.API
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LeadStorage>().As<ILeadStorage>().InstancePerLifetimeScope();
            builder.RegisterType<LeadRepository>().As<ILeadRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AccountRepository>().As<IAccountRepository>().InstancePerLifetimeScope();
        }
    }
}
