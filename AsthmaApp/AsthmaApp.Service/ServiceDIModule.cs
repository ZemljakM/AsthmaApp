using AsthmaApp.Service.Common;
using Autofac;

namespace AsthmaApp.Service
{
    public class ServiceDIModule: Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<RoleService>().As<IRoleService>().InstancePerDependency();
            builder.RegisterType<EthnicityService>().As<IEthnicityService>().InstancePerDependency();
            builder.RegisterType<EducationLevelService>().As<IEducationLevelService>().InstancePerDependency();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();
            builder.RegisterType<RecordService>().As<IRecordService>().InstancePerDependency();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerDependency();
            builder.RegisterType<LocationService>().As<ILocationService>().InstancePerDependency();
        }
    }
}
