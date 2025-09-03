namespace AsthmaApp.WebAPI.Models
{
    public class RecordModel
    {
        public double Weight { get; set; }
        public double Height { get; set; }
        public bool Smoking { get; set; }
        public bool FamilyHistoryOfAsthma { get; set; }
        public bool HistoryOfAllergies { get; set; }
        public bool Eczema { get; set; }
        public bool HayFever { get; set; }
        public bool GastroesophagealReflux { get; set; }
        public bool PetAllergy { get; set; }
        public bool Wheezing { get; set; }
        public bool ShortnessOfBreath { get; set; }
        public bool ChestTightness { get; set; }
        public bool Coughing { get; set; }
        public bool NighttimeSymptoms { get; set; }
        public bool ExerciseInduced { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public double DietQuality { get; set; }
        public double SleepQuality { get; set; }
        public double PhysicalActivity { get; set; }
        public double DustExposure { get; set; }
    }
}
