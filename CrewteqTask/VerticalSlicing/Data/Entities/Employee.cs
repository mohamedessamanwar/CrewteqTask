namespace CrewteqTask.VerticalSlicing.Data.Entities
{
    public class Employee : BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FirstName { get; set; } = ""; // Required
        public string LastName { get; set; } = "";  // Required
        public string Email { get; set; } = "";     // Unique
        public bool IsActive { get; set; } = true;
    }
}

