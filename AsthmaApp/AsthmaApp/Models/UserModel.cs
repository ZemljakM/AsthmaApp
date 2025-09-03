namespace AsthmaApp.WebAPI.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid? EthnicityId { get; set; }
        public Guid? EducationLevelId { get; set; }
        public Guid? DoctorId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsDoctorEdited { get; set; }
    }
}
