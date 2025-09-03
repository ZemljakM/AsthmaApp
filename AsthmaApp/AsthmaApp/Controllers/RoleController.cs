using AsthmaApp.Service.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsthmaApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoleAsync(string name)
        {
            var result = await _service.GetRoleByNameAsync(name);
            if (result is null)
            {
                return NotFound();

            }

            return Ok(result);
        }
    }
}
