using NationalNeon.Repository.Concrete;
using NationalNeon.Repository.DB;
using NationalNeon.Repository.Interface;

namespace NationalNeon.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>
    {
        public EmployeeRepository(IUnitOfWork unit) : base(unit)
        { }
    }
}
