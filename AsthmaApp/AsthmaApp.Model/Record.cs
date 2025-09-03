using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsthmaApp.Models
{
    public class Record
    {
        public Guid Id { get; set; }
        public double BMI { get; set; }
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
        public double DietQuality { get; set; }
        public double SleepQuality { get; set; }
        public double PhysicalActivity { get; set; }
        public double DustExposure { get; set; }
        public double PollenExposure { get; set; }
        public double PollutionExposure { get; set; }
        public double FEV1 { get; set; }
        public double FVC { get; set; }
        public Guid LocationId { get; set; }
        public double Diagnosis { get; set; }
        public bool IsApproved { get; set; }
        public Guid UserId { get; set; }
        public string Recommendation { get; set; }
        public bool IsActive { get; set; } 
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid UpdatedByUserId { get; set; }

        public Location Location { get; set; }
    }
}
