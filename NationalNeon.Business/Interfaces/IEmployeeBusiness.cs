using NationalNeon.Domain.Admin.Employee;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalNeon.Business.Interfaces
{
    public interface IEmployeeBusiness
    {
        EmployeeModel AddEmployee(EmployeeModel employee);
        List<EmployeeModel> GetAll();
        List<EmployeeModel> GetAllEmployeeBySales();

        EmployeeModel GetById(int employeeId);

        void Update(EmployeeModel employee);

        void Delete(int employeeId);
    }
}
