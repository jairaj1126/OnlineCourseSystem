using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OnlineCourseSystem.Extensions;
using OnlineCourseSystem.Models;
using System.Text.Json;

namespace OnlineCourseSystem.Controllers
{
    public class CourseController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string CourseCacheKey = "CourseList_CacheKey";

        public CourseController(IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
        {
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
        }

        // Mock repository
        private static readonly List<Course> SeedCourses = new()
        {
            new Course { CourseId = 101, CourseName = "ASP.NET Core MVC Mastery", Duration = "8 Weeks", Fees = 5000m, DiscountPercentage = 10m },
            new Course { CourseId = 102, CourseName = "Cloud Computing with Azure", Duration = "6 Weeks", Fees = 6500m, DiscountPercentage = 15m },
            new Course { CourseId = 103, CourseName = "Modern React & Next.js", Duration = "10 Weeks", Fees = 4500m, DiscountPercentage = 5m }
        };

        // 1 & 4. Index: Caches Course List using IMemoryCache with a 5-minute expiration
        public IActionResult Index()
        {
            if (!_memoryCache.TryGetValue(CourseCacheKey, out List<Course>? courses))
            {
                courses = SeedCourses;

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                    .SetPriority(CacheItemPriority.Normal);

                _memoryCache.Set(CourseCacheKey, courses, cacheEntryOptions);
            }

            return View(courses);
        }

        // 1, 3 & 4. Details: Query string (id), Response Caching (60s), Stores Course in Session
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "id" })]
        public IActionResult Details(int id) // 'id' mapped from Query String (?id=101) or Route Data
        {
            var course = SeedCourses.FirstOrDefault(c => c.CourseId == id);
            if (course == null) return NotFound();

            // State Management: Store selected course into Session
            HttpContext.Session.SetObjectAsJson("SelectedCourse", course);

            return View(course);
        }

        // 3. State Management: Enrollment Form with Cookie & Hidden Field
        [HttpGet]
        public IActionResult Enroll(int courseId)
        {
            var model = new EnrollmentViewModel { CourseId = courseId };

            // Read StudentName from Cookie if present
            if (Request.Cookies.TryGetValue("StudentName", out string? savedStudentName))
            {
                model.StudentName = savedStudentName;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Enroll(EnrollmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // State Management: Write StudentName to Cookie (expires in 7 days)
            CookieOptions cookieOptions = new()
            {
                Expires = DateTime.UtcNow.AddDays(7),
                HttpOnly = true,
                IsEssential = true
            };
            Response.Cookies.Append("StudentName", model.StudentName, cookieOptions);

            // Retrieve the selected course from Session
            var selectedCourse = HttpContext.Session.GetObjectFromJson<Course>("SelectedCourse");

            ViewBag.Message = $"Enrollment successful for {model.StudentName}!";
            ViewBag.SelectedCourse = selectedCourse;

            return View("EnrollmentSuccess", model);
        }

        // 2. Asynchronous Web Request to retrieve external users
        public async Task<IActionResult> Users()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://jsonplaceholder.typicode.com/users");

            if (!response.IsSuccessStatusCode)
            {
                return View("Error");
            }

            using var contentStream = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var users = await JsonSerializer.DeserializeAsync<List<ExternalUser>>(contentStream, options);

            return View(users ?? new List<ExternalUser>());
        }
    }
}