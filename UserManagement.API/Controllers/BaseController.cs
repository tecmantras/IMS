using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UserManagement.Services.IRepositories;

namespace UserManagement.API.Controllers
{

    public class BaseController<T> : ControllerBase where T : class
    {
        private readonly IService<T> _service;

        public BaseController(IService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            return new OkObjectResult(await _service.GetAllAsync());
        }
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById([FromRoute][Required] int id)
        {
            return new OkObjectResult(await _service.GetByIdAsync(id));
        }
        [HttpPost]
        public virtual IActionResult Post([FromBody][Required] T entity)
        {
            return new OkObjectResult(_service.Create(entity));
        }
        [HttpPut]
        public virtual IActionResult Put([FromBody][Required] T entity)
        {
            return new OkObjectResult(_service.Update(entity));
        }
        [HttpDelete("{id}")]
        public virtual IActionResult Delete([FromRoute][Required] int id)
        {
            return new OkObjectResult(_service.SoftDelete(id));
        }
    }
}
