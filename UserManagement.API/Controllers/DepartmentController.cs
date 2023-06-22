using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserManagememet.Data.Model;
using UserManagement.Services.IRepositories;

namespace UserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController: BaseController<Department>
    {
        private readonly IService<Department> service;

        public DepartmentController(IService<Department> service) :base(service)
        {
            this.service = service;
        }

    }
}
