using AsthmaApp.Service.Common;
using AsthmaApp.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AsthmaApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EducationLevelController : ControllerBase
    {
        private IEducationLevelService _service;

        public EducationLevelController(IEducationLevelService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Patient, Doctor")]
        public async Task<IActionResult> GetAllEducationLevelsAsync()
        {
            var entities = await _service.GetAllEducationLevelsAsync();
            if (entities is null)
            {
                return NotFound();
            }

            List<EducationLevelModel> levels = new List<EducationLevelModel>();

            foreach (var level in entities)
            {
                EducationLevelModel educationLevelModel = new EducationLevelModel
                {
                    Id = level.Id,
                    Name = level.Name
                };
                levels.Add(educationLevelModel);
            }

            return Ok(levels);
        }
    }
}
