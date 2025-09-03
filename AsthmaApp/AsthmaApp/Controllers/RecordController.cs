using AsthmaApp.Common;
using AsthmaApp.Models;
using AsthmaApp.Service.Common;
using AsthmaApp.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AsthmaApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private IRecordService _recordService;
        private ILocationService _locationService;
        private IUserService _userService;

        public RecordController(IRecordService recordService, ILocationService locationService, IUserService userService)
        {
            _recordService = recordService;
            _locationService = locationService;
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Doctor, Patient")]
        public async Task<IActionResult> GetAllRecordsAsync(Guid userId, int pageSize = 3, int pageNumber = 1, string orderBy = "DateUpdated", string orderDirection = "DESC")
        {
            Filter filter = new Filter
            {
                UserId = userId
            };

            Paging paging = new Paging
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            Sorting sorting = new Sorting
            {
                OrderBy = orderBy,
                OrderDirection = orderDirection
            };

            var result = await _recordService.GetAllRecordsAsync(filter, paging, sorting);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Doctor, Patient")]
        [Route("{id}")]
        public async Task<IActionResult> GetRecordByIdAsync(Guid id)
        {
            var result = await _recordService.GetRecordByIdAsync(id);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Doctor, Patient")]
        [Route("count-records")]
        public async Task<IActionResult> CountRecordsAsync(Guid userId)
        {
            Filter filter = new Filter
            {
                UserId = userId
            };

            var result = await _recordService.CountRecordsAsync(filter);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Doctor, Patient")]
        public async Task<IActionResult> PredictAsthmaAsync(RecordModel recordModel)
        {
            if (recordModel == null)
                return BadRequest();

            var userId = Guid.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var userProfile = await _userService.GetUserByIdAsync(userId);
            var record = await _recordService.CreateRecordAsync();
            record.BMI = Math.Round(recordModel.Weight / (recordModel.Height * recordModel.Height), 1);

            #region Location

            var location = await _locationService.GetLocationAsync(recordModel.City, recordModel.Country);
            if (location == null)
            {
                location = await _locationService.GetCoordinatesAsync(recordModel.City, recordModel.Country);
            }

            record.LocationId = location.Id;

            #endregion Location

            var pollenExposure = await _recordService.GetPollenExposureAsync(location.Latitude, location.Longitude);
            if (pollenExposure != null)
            {
                record.PollenExposure = (double)pollenExposure;
            }

            var pollutionExposure = await _recordService.GetPollutionExposureAsync(location.Latitude, location.Longitude);
            if (pollutionExposure != null)
            {
                record.PollutionExposure = (double)pollutionExposure;
            }


            record.Smoking = recordModel.Smoking;
            record.PhysicalActivity = recordModel.PhysicalActivity;
            record.DietQuality = recordModel.DietQuality;
            record.SleepQuality = recordModel.SleepQuality;
            record.DustExposure = recordModel.DustExposure;
            record.PetAllergy = recordModel.PetAllergy;
            record.FamilyHistoryOfAsthma = recordModel.FamilyHistoryOfAsthma;
            record.HistoryOfAllergies = recordModel.HistoryOfAllergies;
            record.Eczema = recordModel.Eczema;
            record.HayFever = recordModel.HayFever;
            record.GastroesophagealReflux = recordModel.GastroesophagealReflux;
            record.Wheezing = recordModel.Wheezing;
            record.ShortnessOfBreath = recordModel.ShortnessOfBreath;
            record.ChestTightness = recordModel.ChestTightness;
            record.Coughing = recordModel.Coughing;
            record.NighttimeSymptoms = recordModel.NighttimeSymptoms;
            record.ExerciseInduced = recordModel.ExerciseInduced;
            record.CreatedByUserId = userId;
            record.UpdatedByUserId = userId;
            record.IsApproved = false;
            record.UserId = userId;

            var probability = await _recordService.PredictAsthmaAsync(record, userProfile);
            if (probability == null)
            {
                return BadRequest();
            }

            record.Diagnosis = Math.Round(probability.Value, 2);

            record.Recommendation = await _recordService.GeneratePromptAsync(record, userProfile);

            var result = await _recordService.InsertRecordAsync(record);

            return result ? Ok() : BadRequest();
        }



        [HttpPost]
        [Authorize(Roles = "Admin, Doctor, Patient")]
        [Route("generate")]
        public async Task<IActionResult> GenerateAsthmaAsync(Record record)
        {
            var user = await _userService.GetUserByIdAsync(record.CreatedByUserId);

            var result = await _recordService.GeneratePromptAsync(record, user);

            return Ok(result);

        }

        [HttpPut]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateRecordAsync(UpdateRecordModel updateRecordModel)
        {
            if (updateRecordModel == null)
            {
                return BadRequest();
            }

            var record = await _recordService.GetRecordByIdAsync(updateRecordModel.Id);

            if (record == null)
            {
                return NotFound();
            }

            if (!String.IsNullOrEmpty(updateRecordModel.Recommendation))
            {
                record.Recommendation = updateRecordModel.Recommendation;
                record.IsApproved = true;
            }

            if (updateRecordModel.FEV1 != null)
            {
                record.FEV1 = (double)updateRecordModel.FEV1;
            }

            if (updateRecordModel.FVC != null)
            {
                record.FVC = (double)updateRecordModel.FVC;
            }

            var result = await _recordService.UpdateRecordAsync(record);

            return Ok();

        }
    }
}
