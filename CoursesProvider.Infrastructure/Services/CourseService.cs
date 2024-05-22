using CoursesProvider.Infrastructure.Data.Contexts;
using CoursesProvider.Infrastructure.Data.Entities;
using CoursesProvider.Infrastructure.Factories;
using CoursesProvider.Infrastructure.Handlers;
using CoursesProvider.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CoursesProvider.Infrastructure.Services;


public interface ICourseService
{
	Task<Course> CreateCourseAsync(CourseCreateRequest request);
	Task<Course> GetCourseByIdAsync(string id);
	Task<IEnumerable<Course>> GetCoursesAsync();
	Task<Course> UpdateCourseAsync(CourseUpdateRequest request);
	Task<bool> DeleteCourseAsync(string id);
}
public class CourseService(IDbContextFactory<DataContext> contextFactory, ServiceBusHandler serviceBusHandler) : ICourseService
{

    private readonly IDbContextFactory<DataContext> _contextFactory = contextFactory;
    private readonly ServiceBusHandler _serviceBusHandler;




    public async Task<Course> CreateCourseAsync(CourseCreateRequest request)
    {
        await using var context = _contextFactory.CreateDbContext();

        var courseEntity = CourseFactory.Create(request);
        context.Courses.Add(courseEntity);
        await context.SaveChangesAsync();

        // Send a message to Service Bus
        await _serviceBusHandler.PublishAsync(JsonConvert.SerializeObject(courseEntity), "CourseCreated");



        return CourseFactory.Create(courseEntity);
       


    }


    public async Task<bool> DeleteCourseAsync(string id)
    {
        await using var context = _contextFactory.CreateDbContext();
        var courseEntity = await context.Courses.FirstOrDefaultAsync(x => x.Id == id);
        if (courseEntity == null) return false;

        context.Courses.Remove(courseEntity);
        await context.SaveChangesAsync();

        // Send a message to Service Bus
        await _serviceBusHandler.PublishAsync(JsonConvert.SerializeObject(courseEntity), "CourseDeleted");

        return true;
    }



    public async Task<Course> GetCourseByIdAsync(string id)
    {
        await using var context = _contextFactory.CreateDbContext();
        var courseEntity = await context.Courses.FirstOrDefaultAsync(x => x.Id == id);

        return courseEntity == null ? null! : CourseFactory.Create(courseEntity);
    }

    public async Task<IEnumerable<Course>> GetCoursesAsync()
    {
        await using var context = _contextFactory.CreateDbContext();
        var courseEntities = await context.Courses.ToListAsync();

        return courseEntities.Select(CourseFactory.Create);
    }


    public async Task<Course> UpdateCourseAsync(CourseUpdateRequest request)
    {
        await using var context = _contextFactory.CreateDbContext();
        var existingCourse = await context.Courses.FirstOrDefaultAsync(x => x.Id == request.Id);
        if (existingCourse == null) return null!;

        var updatedCourseEntity = CourseFactory.Update(request);
        updatedCourseEntity.Id = existingCourse.Id;
        // Create a copy of updatedCourseEntity to send to Service Bus
        var updatedCourseEntityCopy = new CourseEntity
        {
            Id = updatedCourseEntity.Id,
            ImageUri = updatedCourseEntity.ImageUri,
            ImageHeaderUri = updatedCourseEntity.ImageHeaderUri,
            IsBestseller = updatedCourseEntity.IsBestseller,
            IsDigital = updatedCourseEntity.IsDigital,
            Categories = updatedCourseEntity.Categories,
            Title = updatedCourseEntity.Title,
            Ingress = updatedCourseEntity.Ingress,
            StarRating = updatedCourseEntity.StarRating,
            Reviews = updatedCourseEntity.Reviews,
            Likes = updatedCourseEntity.Likes,
            LikesInPercent = updatedCourseEntity.LikesInPercent,
            Hours = updatedCourseEntity.Hours,
            Authors = updatedCourseEntity.Authors?.Select(a => new AuthorEntity
            {
                Name = a.Name,
                AuthorImage = a.AuthorImage
            }).ToList(),
            Prices = updatedCourseEntity.Prices == null ? null : new PricesEntity
            {
                Currency = updatedCourseEntity.Prices.Currency,
                Price = updatedCourseEntity.Prices.Price,
                Discount = updatedCourseEntity.Prices.Discount
            },
            Content = updatedCourseEntity.Content == null ? null : new ContentEntity
            {
                Description = updatedCourseEntity.Content.Description,
                Includes = updatedCourseEntity.Content.Includes,
                ProgramDetails = updatedCourseEntity.Content.ProgramDetails?.Select(pd => new ProgramDetailItemEntity
                {
                    Id = pd.Id,
                    Title = pd.Title,
                    Description = pd.Description
                }).ToList()
            }
        };

        context.Entry(existingCourse).CurrentValues.SetValues(updatedCourseEntity);

        await context.SaveChangesAsync();

        // Send a message to Service Bus
        await _serviceBusHandler.PublishAsync(JsonConvert.SerializeObject(updatedCourseEntityCopy), "CourseUpdated");

        return CourseFactory.Create(existingCourse);
    }
}
