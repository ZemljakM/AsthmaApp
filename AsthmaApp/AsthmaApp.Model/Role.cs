namespace AsthmaApp.Models
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public List<User> Users { get; set; }
    }
}
