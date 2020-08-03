using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Study.CoreApi.Models;
using Study.IRepository;
using Study.Models;

namespace Study.CoreApi.Controllers
{
    [Route("api/[controller]")]
    //[ApiExplorerSettings(IgnoreApi = true)]//awagger忽略当前Api
    //[Authorize(Roles = "Admin")] //只允许 用户 Role 是 Admin 可以访问
    [Authorize] //任何登陆的用户都可以访问
    //[Authorize(Policy = "Admin")] //对应 services.AddAuthorization 定义的策略名 
    public class EmployeeController : Controller
    {
        public IEmployeeRepository _employeeRepository;
        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        /// <summary>
        /// 获取所有员工
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        public async Task<ResultData<IList<Employee>>> Get()
        {
            //var data = await _employeeRepository.LoadAll();
            var data = await Task.Run(() =>
            {
                return GetSourceEmployees();
            });
            var result = new ResultData<IList<Employee>>
            {
                Code = (int)ResultCodeEnum.SUCCESS,
                Msg = "成功",
                Data = data
            };
            return result;
        }

        /// <summary>
        /// 根据Id 获取员工
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ResultData<Employee>> Get(int id)
        {
            var result = new ResultData<Employee>
            {
                Code = (int)ResultCodeEnum.SUCCESS,
                Msg = "成功",
            };

            //var data = await _employeeRepository.GetById(id);
            var employees = await Task.Run(() => GetSourceEmployees());

            var data = employees.FirstOrDefault(l => l.Id == id);
            if (data != null)
            {
                result.Code = (int)ResultCodeEnum.DATA_NULL;
                result.Msg = "找不到所需数据";
            }
            else
            {
                result.Code = (int)ResultCodeEnum.SUCCESS;
                result.Msg = "成功";
                result.Data = data;
            }
            return result;
        }

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResultData<int>> Post([FromBody]EmployeeParam employee)
        {
            var entity = new Employee()
            {
                Address = employee.Address,
                Gender = employee.Gender,
                Name = employee.Name,
                Mobile = employee.Mobile
            };
            var _result = await _employeeRepository.Add(entity);
            var result = new ResultData<int>();
            if (_result > 0)
            {
                result.Code = (int)ResultCodeEnum.SUCCESS;
                result.Msg = "成功";
                result.Data = _result;

            }
            else
            {
                result.Code = (int)ResultCodeEnum.FAIL;
                result.Msg = "失败";
            }
            return result;
        }

        [NonAction]
        private IList<Employee> GetSourceEmployees()
        {
            IList<Employee> employees = new List<Employee>();
            employees.Add(new Employee
            {
                Address = "北京市朝阳公园",
                Gender = 1,
                Id = 1,
                Mobile = "16578976589",
                Name = "张三"
            });
            employees.Add(new Employee
            {
                Address = "批量添加",
                Gender = 2,
                Id = 2,
                Mobile = "16897623407",
                Name = "测试员工-001"
            });
            return employees;
        }
    }
}
