using OnlineCourseSystem.Models;

namespace OnlineCourseSystem.Extensions
{
    public static class CourseExtensions
    {
        public static decimal CalculateFinalFees(this Course course)
        {
            if (course == null) return 0m;
            decimal discountAmount = (course.Fees * course.DiscountPercentage) / 100m;
            return course.Fees - discountAmount;
        }
    }
}