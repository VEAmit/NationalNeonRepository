using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NationalNeon.Business.Interfaces;
using NationalNeon.Domain.Admin.Employee;
using NationalNeon.Utility.Enums;
using NationalNeon.Web.ViewModels;

namespace NationalNeon.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IEmployeeBusiness iemployeeBusiness;
        private readonly IDepartmentBusiness idepartmentBusiness;
        private readonly EmployeeModel employeeModel;
        public AdminController(IEmployeeBusiness iemployeeBusiness, EmployeeModel employeeModel, IDepartmentBusiness idepartmentBusiness)
        {
            this.iemployeeBusiness = iemployeeBusiness;
            this.idepartmentBusiness = idepartmentBusiness;
            this.employeeModel = employeeModel;
        }
        public IActionResult EmployeeIndex()
        {
            return View();
        }
        public IActionResult GetEmployeeList()
        {
            List<EmployeeViewModel> employeeViewModel = new List<EmployeeViewModel>();
            var employeeList = iemployeeBusiness.GetAll();
            Mapper.Map(employeeList, employeeViewModel);
            return PartialView("_EmployeeList", employeeViewModel);
        }
        public IActionResult SystemSetupIndex()
        {
            return View();
        }
        public IActionResult AddUpdateEmployee()
        {
            var deptList = idepartmentBusiness.GetAll().Select(item => new { Id = item.departmentId, Name = item.departmentname }).ToList();
            ViewBag.DepartmentList = new SelectList(deptList, "Id", "Name");
            EmployeeType employeeType = EmployeeType.Sales;
            var empTypeList = EnumHelper<EmployeeType>.GetValues(employeeType).Select(item => new { Id = (int)item, Name = item.ToString() }).ToList();
            ViewBag.EmployeeType = new SelectList(empTypeList, "Id", "Name");
            return PartialView("_AddUpdateEmployee");
        }
        [HttpPost]
        public IActionResult AddUpdateEmployee(EmployeeViewModel employeeViewModel)
        {
            Mapper.Map(employeeViewModel, employeeModel);
            if (employeeModel.EmployeeId > 0)
                iemployeeBusiness.Update(employeeModel);
            else
                iemployeeBusiness.AddEmployee(employeeModel);
            return Json(new
            {
                success = true
            });

        }

        [HttpPost]
        public IActionResult GetEmployee(int employeeId)
        {
            var employeeModel = iemployeeBusiness.GetById(employeeId);
            EmployeeViewModel employeeViewModel = new EmployeeViewModel();
            if (employeeModel != null)
            {
                Mapper.Map(employeeModel, employeeViewModel);
            }
            return Json(new
            {
                success = true,
                data = employeeViewModel
            });
            //return PartialView("_AddUpdateEmployee");
        }
        [HttpPost]
        public IActionResult DeleteEmployee(int employeeId)
        {
            iemployeeBusiness.Delete(employeeId);
            return Json(new
            {
                success = true
            });
        }


    }
}
