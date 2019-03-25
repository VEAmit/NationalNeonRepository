using ExpressMapper;
using NationalNeon.Business.Interfaces;
using NationalNeon.Domain.Admin.Employee;
using NationalNeon.Repository;
using NationalNeon.Repository.DB;
using NationalNeon.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalNeon.Business.Concrete
{
    public class EmployeeBusiness : IEmployeeBusiness
    {
        private readonly EmployeeRepository employeeRepository;

        public EmployeeBusiness(IUnitOfWork unit)
        {
            employeeRepository = new EmployeeRepository(unit);
        }
        public EmployeeModel AddEmployee(EmployeeModel model)
        {
            Employee employee = new Employee();
            Mapper.Map(model, employee);
            employeeRepository.Insert(employee);
            //Mapper.Map(employee, model);
            return model;
        }

        public List<EmployeeModel> GetAll()
        {
            var model = new List<EmployeeModel>();
            var data = employeeRepository.GetAll();
            Mapper.Map(data, model);
            return model;
        }

        public List<EmployeeModel> GetAllEmployeeBySales()
        {
            var model = new List<EmployeeModel>();
            var data = employeeRepository.GetAll().Where(a=> a.EmployeeType == 0);
            Mapper.Map(data, model);
            return model;
        }

        public EmployeeModel GetById(int employeeId)
        {
            var model = new EmployeeModel();
            var data = employeeRepository.FindBy(x => x.EmployeeId == employeeId);
            Mapper.Map(data, model);
            return model;
        }

        public void Update(EmployeeModel model)
        {
            var data = employeeRepository.FindBy(x => x.EmployeeId == model.EmployeeId);
            if (data != null)
            {
                data.FirstName = model.FirstName;
                data.LastName = model.LastName;
                data.UpdatedOn = DateTime.Now;
                data.EmployeeType = model.EmployeeType;
                data.DepartmentId = model.DepartmentId;
                employeeRepository.Update(data);
            }
        }

        public void Delete(int employeeId)
        {
            employeeRepository.Delete(u => u.EmployeeId == employeeId);

        }
    }
}
