using AsthmaApp.Repository.Common;
using Autofac;

namespace AsthmaApp.Repository
{
    public class RepositoryDIModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RoleRepository>().As<IRoleRepository>().InstancePerDependency();
            builder.RegisterType<EthnicityRepository>().As<IEthnicityRepository>().InstancePerDependency();
            builder.RegisterType<EducationLevelRepository>().As<IEducationLevelRepository>().InstancePerDependency();
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerDependency();
            builder.RegisterType<RecordRepository>().As<IRecordRepository>().InstancePerDependency();
            builder.RegisterType<LocationRepository>().As<ILocationRepository>().InstancePerDependency();
        }
    }
}
