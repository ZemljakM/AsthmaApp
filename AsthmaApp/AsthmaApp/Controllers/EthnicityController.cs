using AsthmaApp.Service.Common;
using AsthmaApp.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsthmaApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EthnicityController : ControllerBase
    {
        private IEthnicityService _service;

        public EthnicityController(IEthnicityService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Patient, Doctor")]
        public async Task<IActionResult> GetAllEthnicitiesAsync()
        {
            var entities = await _service.GetAllEthnicitiesAsync();
            if (entities is null)
            {
                return NotFound();
            }

            List<EthnicityModel> ethnicities = new List<EthnicityModel>();

            foreach (var entity in entities)
            {
                EthnicityModel ethnicityModel = new EthnicityModel
                {
                    Id = entity.Id,
                    Name = entity.Name
                };
                ethnicities.Add(ethnicityModel);
            }

            return Ok(ethnicities);
        }
    }
}
