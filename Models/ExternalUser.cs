namespace OnlineCourseSystem.Models
{
    public class ExternalUser
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Company Company { get; set; } = new();
    }

    public class Company
    {
        public string Name { get; set; } = string.Empty;
    }
}