using Microsoft.Extensions.DependencyInjection;
using NationalNeon.Repository.Concrete;
using NationalNeon.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalNeon.Business.Interfaces;
using NationalNeon.Business.Concrete;
using NationalNeon.Domain.Admin.Employee;

namespace NationalNeon.Business.Intrastructure
{
   public class Bindings
    {
        public static void AddBindings(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IUserBusiness, UserBusiness>();
            services.AddTransient<IDepartmentBusiness, DepartmentBusiness>();
            services.AddTransient<IJobBusiness, JobBusiness>();
            services.AddTransient<ITaskBusiness,TaskBusiness>();
            services.AddTransient<IGraphReport, GraphReport>();
            services.AddTransient<EmployeeModel, EmployeeModel>();
            services.AddTransient<IEmployeeBusiness, EmployeeBusiness>();
            services.AddTransient<EmployeeBusiness, EmployeeBusiness>();
        }
    }
}
