namespace AsthmaApp.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? OIB { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Gender { get; set; }
        public Guid RoleId { get; set; }
        public Guid? EthnicityId { get; set; }
        public Guid? EducationLevelId { get; set; }
        public Guid? DoctorId { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedByUserId { get; set; }
        public Guid UpdatedByUserId { get; set; }

        public Role Role { get; set; }
        public Ethnicity? Ethnicity { get; set; }
        public EducationLevel? EducationLevel { get; set; }
        public User? Doctor { get; set; }
        public List<Record> Records { get; set; }
    }
}
