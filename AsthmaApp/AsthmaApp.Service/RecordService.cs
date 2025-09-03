using AsthmaApp.Common;
using AsthmaApp.Models;
using AsthmaApp.Repository.Common;
using AsthmaApp.Service.Common;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace AsthmaApp.Service
{
    public class RecordService : IRecordService
    {
        private IRecordRepository _repository;
        private readonly HttpClient _httpClient;
        private readonly string _googleApiKey;
        private readonly string _openAIApiKey;

        public RecordService(HttpClient httpClient, IConfiguration config, IRecordRepository repository)
        {
            _repository = repository;
            _httpClient = httpClient;
            _googleApiKey = config["GoogleMaps:ApiKey"];
            _openAIApiKey = config["OpenAI:ApiKey"];
        }

        public async Task<Record> CreateRecordAsync()
        {
            var record = new Record();
            record.Id = Guid.NewGuid();
            record.IsActive = true;
            record.DateCreated = DateTime.Now;
            record.DateUpdated = DateTime.Now;
            return record;
        }

        public async Task<int> CountRecordsAsync(Filter filter)
        {
            return await _repository.CountRecordsAsync(filter);
        }

        public async Task<List<Record>> GetAllRecordsAsync(Filter filter, Paging paging, Sorting sorting)
        {
            return await _repository.GetAllRecordsAsync(filter, paging, sorting);
        }

        public async Task<Record?> GetRecordByIdAsync(Guid id)
        {
            return await _repository.GetRecordByIdAsync(id);
        }

        public async Task<string> GeneratePromptAsync(Record record, User user)
        {
            var sb = new StringBuilder("Patient details: \n");

            sb.AppendLine($"- Age: {GetUserAge((DateTime)user.DateOfBirth)}");
            sb.AppendLine($"- Gender: {user.Gender}");
            sb.AppendLine($"- Ethnicity: {user.Ethnicity.Name}");
            sb.AppendLine($"- EducationLevel: {user.EducationLevel.Name}");
            sb.AppendLine($"- BMI: {record.BMI}");
            sb.AppendLine($"- Smoking: {record.Smoking}");
            sb.AppendLine($"- PhysicalActivity: {record.PhysicalActivity} (scale: 0–10)");
            sb.AppendLine($"- Diet Quality: {record.DietQuality} (scale: 0-10)");
            sb.AppendLine($"- Sleep Quality: {record.SleepQuality} (scale: 4-10)");
            sb.AppendLine($"- Pollution Exposure: {record.PollutionExposure} (0–1.9: Poor, 2–3.9: Low, 4–5.9: Moderate, 6–7.9: Good, 8–10: Excellent)");
            sb.AppendLine($"- Pollen Exposure: {record.PollenExposure} (0: None, 2: Very Low, 4: Low, 6: Moderate, 8: High, 10: Very High)");
            sb.AppendLine($"- Dust Exposure: {record.DustExposure} (scale: 0-10)");
            sb.AppendLine($"- Pet Allergy: {record.PetAllergy}");
            sb.AppendLine($"- Family History Of Asthma: {record.FamilyHistoryOfAsthma}");
            sb.AppendLine($"- History Of Allergies: {record.HistoryOfAllergies}");
            sb.AppendLine($"- Eczema: {record.Eczema}");
            sb.AppendLine($"- Hay Fever: {record.HayFever}");
            sb.AppendLine($"- Gastroesophageal Reflux: {record.GastroesophagealReflux}");
            sb.AppendLine($"- Wheezing: {record.Wheezing}");
            sb.AppendLine($"- Shortness Of Breath: {record.ShortnessOfBreath}");
            sb.AppendLine($"- Chest Tightness: {record.ChestTightness}");
            sb.AppendLine($"- Coughing: {record.Coughing}");
            sb.AppendLine($"- Nighttime Symptoms: {record.NighttimeSymptoms}");
            sb.AppendLine($"- Exercise Induced: {record.ExerciseInduced}");
            sb.AppendLine($"The ML model, trained on the asthma dataset, predicts that this patient has a {record.Diagnosis * 100}% risk of asthma.");

            sb.AppendLine("Write a short response (no more than 100 words) in the tone of a doctor speaking to the patient.");
            sb.AppendLine("Begin with one of the following phrases based on the data: 'You are likely to have asthma.', 'You may possibly have asthma.', " +
                "or 'You are unlikely to have asthma.'");
            sb.AppendLine("After that, add 2–3 sentences with a brief recommendation, mentioning 2–3 key symptoms or risk factors, avoiding technical jargon, " +
                "and focus on what the patient should do next.");
            sb.AppendLine("Do not use headers such as 'Assessment:' or 'Recommendation:'. Write as a natural paragraph.");
            sb.AppendLine("Use the model’s result as a baseline, but adapt the response thoughtfully by considering the patient’s symptoms, lifestyle, " +
                "and history so that the recommendation varies when appropriate.");
            sb.AppendLine("The presence of symptoms such as wheezing, coughing, chest tightness, or shortness of breath does not automatically mean a patient has asthma. " +
                "These signs can also be linked to other conditions, such as heart disease, respiratory infections, or allergies. They should be considered in the " +
                "broader context of the patient’s history, lifestyle, and test results before making any diagnosis.");


            ChatClient client = new ChatClient(model: "gpt-3.5-turbo", apiKey: _openAIApiKey);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are an expert pulmonologist. Given patient data, assess asthma likelihood and recommend treatment only when appropriate. " +
                    "All patient information is important — not just the presence of symptoms. Consider lifestyle, environment, medical history, and overall health indicators." +
                    "Certain combinations of features may point to other conditions or reduce the likelihood of asthma."),
                new UserChatMessage(sb.ToString())
            };

            ChatCompletion response = await client.CompleteChatAsync(messages);

            return response.Content[0].Text;
        }

        public async Task<double?> GetPollenExposureAsync(double latitude, double longitude)
        {
            var url = $"https://pollen.googleapis.com/v1/forecast:lookup?location.latitude={latitude.ToString(CultureInfo.InvariantCulture)}" +
                $"&location.longitude={longitude.ToString(CultureInfo.InvariantCulture)}&days=1&key={_googleApiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var pollenTypeInfo = json.RootElement
                .GetProperty("dailyInfo")[0]
                .GetProperty("pollenTypeInfo");

            var pollenIndex = 0;
            foreach (var pollen in pollenTypeInfo.EnumerateArray())
            {
                if (pollen.TryGetProperty("indexInfo", out JsonElement indexInfo) &&
                    indexInfo.TryGetProperty("value", out JsonElement valueElement))
                {
                    var index = valueElement.GetInt32();
                    pollenIndex = index > pollenIndex ? index : pollenIndex;
                }
            }

            return pollenIndex * 2;
        }

        public async Task<double?> GetPollutionExposureAsync(double latitude, double longitude)
        {
            var url = $"https://airquality.googleapis.com/v1/currentConditions:lookup?key={_googleApiKey}";
            var payload = new
            {
                location = new
                {
                    latitude = latitude,
                    longitude = longitude
                }
            };

            var jsonPayload = JsonSerializer.Serialize(payload);
            var data = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, data);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var pollutionIndex = json.RootElement.GetProperty("indexes")[0].GetProperty("aqi").GetDouble();

            return pollutionIndex / 10;
        }

        public async Task<bool> InsertRecordAsync(Record record)
        {
            return await _repository.InsertRecordAsync(record);
        }

        public async Task<double?> PredictAsthmaAsync(Record record, User userProfile)
        {
            var jsonPayload = PrepareData(record, userProfile);
            var data = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://diplomski-u7za.onrender.com/predict", data);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            return json.RootElement.GetProperty("probability").GetDouble();
        }

        public async Task<bool> UpdateRecordAsync(Record record)
        {
            return await _repository.UpdateRecordAsync(record);
        }

        private int GetUserAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age)) age--;

            return age;
        }

        private string PrepareData(Record record, User user)
        {
            var age = GetUserAge((DateTime)user.DateOfBirth);

            var features = new List<double>
            {
                age,
                System.String.Equals(user.Gender, "Male") ? 0 : 1,  
                user.Ethnicity.Value,
                user.EducationLevel.Value,
                record.BMI,
                record.Smoking ? 1 : 0,
                record.PhysicalActivity,
                record.DietQuality,
                record.SleepQuality,
                record.PollutionExposure,
                record.PollenExposure,
                record.DustExposure,
                record.PetAllergy ? 1 : 0,
                record.FamilyHistoryOfAsthma ? 1 : 0,
                record.HistoryOfAllergies ? 1 : 0,
                record.Eczema ? 1 : 0,
                record.HayFever ? 1 : 0,
                record.GastroesophagealReflux ? 1 : 0,
                record.Wheezing ? 1 : 0,
                record.ShortnessOfBreath ? 1 : 0,
                record.ChestTightness ? 1 : 0,
                record.Coughing ? 1 : 0,
                record.NighttimeSymptoms ? 1 : 0,
                record.ExerciseInduced ? 1 : 0
            };

            var payload = new { features = features };

            return JsonSerializer.Serialize(payload);
        }
    }
}
