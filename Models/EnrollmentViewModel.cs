using System.ComponentModel.DataAnnotations;

namespace OnlineCourseSystem.Models
{
    public class EnrollmentViewModel
    {
        [Required]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Please enter student name")]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; } = string.Empty;
    }
}
