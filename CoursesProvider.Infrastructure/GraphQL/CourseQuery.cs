using CoursesProvider.Infrastructure.Models;
using CoursesProvider.Infrastructure.Services;

namespace CoursesProvider.Infrastructure.GraphQL;

//Hämtas ut från systemet
public class CourseQuery(ICourseService courseService)
{
	private readonly ICourseService _courseService = courseService;

	[GraphQLName("getCourses")]
	public async Task<IEnumerable<Course>> GetCoursesAsync()
	{
		return await _courseService.GetCoursesAsync();
	}

	[GraphQLName("getCourseById")]

	public async Task<Course> GetCourseAsync(string id)
	{
		return await _courseService.GetCourseByIdAsync(id);
	}
}
