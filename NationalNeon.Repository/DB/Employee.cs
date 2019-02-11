using NationalNeon.Utility.Enums;
using System;

namespace NationalNeon.Repository.DB
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public int DepartmentId { get; set; }
    }
}