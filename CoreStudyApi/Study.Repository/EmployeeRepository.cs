using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Study.IRepository;
using Study.Models;
using Study.Repository.Base;

namespace Study.Repository
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {

    }
}
