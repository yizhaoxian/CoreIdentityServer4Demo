using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Study.CoreApi
{
    public class RegisterServices : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //将程序集下的所有接口自动注册 
            builder.RegisterAssemblyTypes(typeof(Study.IRepository.IEmployeeRepository).Assembly, typeof(Study.Repository.EmployeeRepository).Assembly).Where(l => l.IsClass && l.Name.EndsWith("Repository")).AsImplementedInterfaces();
        }
    }
}
