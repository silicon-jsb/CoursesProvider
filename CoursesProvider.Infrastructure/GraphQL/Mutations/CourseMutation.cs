using CoursesProvider.Infrastructure.Models;
using CoursesProvider.Infrastructure.Services;
using System.Globalization;


namespace CoursesProvider.Infrastructure.GraphQL.Mutations;

public class CourseMutation(ICourseService courseService)
{
	private readonly ICourseService _courseService = courseService;


	[GraphQLName("createCourse")]
	public async Task<Course> CreateCourseAsync(CourseCreateRequest input)
	{
		return await _courseService.CreateCourseAsync(input);
	}


	[GraphQLName("updateCourse")]
	public async Task<Course> UpdateCourseAsync(CourseUpdateRequest input)
	{
		return await _courseService.UpdateCourseAsync(input);
	}

	[GraphQLName("deleteCourse")]
	public async Task<bool> DeleteCourseAsync(string id)
	{
		return await _courseService.DeleteCourseAsync(id);
	}
}
