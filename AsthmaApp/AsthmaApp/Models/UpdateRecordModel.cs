namespace AsthmaApp.WebAPI.Models
{
    public class UpdateRecordModel
    {
        public Guid Id { get; set; }
        public string? Recommendation { get; set; }
        public double? FEV1 { get; set; }
        public double? FVC { get; set; }
    }
}
