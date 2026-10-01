using System.ComponentModel.DataAnnotations;

namespace OnlineCourseSystem.Models
{
    public class Course
    {
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public string Duration { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Currency)]
        public decimal Fees { get; set; }

        [Range(0, 100)]
        [Display(Name = "Discount (%)")]
        public decimal DiscountPercentage { get; set; }
    }
}