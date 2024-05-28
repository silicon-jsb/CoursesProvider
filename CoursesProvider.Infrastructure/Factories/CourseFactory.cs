using CoursesProvider.Infrastructure.Data.Entities;
using CoursesProvider.Infrastructure.Models;

namespace CoursesProvider.Infrastructure.Factories;

public static class CourseFactory
{
	public static CourseEntity Create(CourseCreateRequest request)
	{
		return new CourseEntity
		{
			ImageUri = request.ImageUri,
			ImageHeaderUri = request.ImageHeaderUri,
			IsBestseller = request.IsBestseller,
			IsDigital = request.IsDigital,
			Categories = request.Categories,
			Title = request.Title,
			Ingress = request.Ingress,
			StarRating = request.StarRating,
			Reviews = request.Reviews,
			LikesInPercent = request.LikesInPercent,
			Likes = request.Likes,
			Hours = request.Hours,
			Authors = request.Authors?.Select(x => new AuthorEntity
			{
				Name = x.Name,
				AuthorImage = x.AuthorImage
			}).ToList(),
			Prices = request.Prices == null ? null : new PricesEntity
			{
				Currency = request.Prices.Currency,
				Price = request.Prices.Price,
				Discount = request.Prices.Discount
			},
			Content = request.Content == null ? null : new ContentEntity
			{
				Description = request.Content.Description,
				Includes = request.Content.Includes,
				ProgramDetails = request.Content.ProgramDetails?.Select(x => new ProgramDetailItemEntity
				{
					Id = x.Id,
					Title = x.Title,
					Description = x.Description
				}).ToList()
			}
		};
	}

	public static CourseEntity Create(CourseUpdateRequest request)
	{
		return new CourseEntity
		{
			Id = request.Id,
			ImageUri = request.ImageUri,
			ImageHeaderUri = request.ImageHeaderUri,
			IsBestseller = request.IsBestseller,
			IsDigital = request.IsDigital,
			Categories = request.Categories,
			Title = request.Title,
			Ingress = request.Ingress,
			StarRating = request.StarRating,
			Reviews = request.Reviews,
			LikesInPercent = request.LikesInPercent,
			Likes = request.Likes,
			Hours = request.Hours,
			Authors = request.Authors?.Select(x => new AuthorEntity
			{
				Name = x.Name,
				AuthorImage = x.AuthorImage
			}).ToList(),
			Prices = request.Prices == null ? null : new PricesEntity
			{
				Currency = request.Prices.Currency,
				Price = request.Prices.Price,
				Discount = request.Prices.Discount
			},
			Content = request.Content == null ? null : new ContentEntity
			{
				Description = request.Content.Description,
				Includes = request.Content.Includes,
				ProgramDetails = request.Content.ProgramDetails?.Select(x => new ProgramDetailItemEntity
				{
					Id = x.Id,
					Title = x.Title,
					Description = x.Description
				}).ToList()
			}
		};
	}

	public static Course Create(CourseEntity entity)
	{
		return new Course
		{
			Id = entity.Id,
			ImageUri = entity.ImageUri,
			ImageHeaderUri = entity.ImageHeaderUri,
			IsBestseller = entity.IsBestseller,
			IsDigital = entity.IsDigital,
			Categories = entity.Categories,
			Title = entity.Title,
			Ingress = entity.Ingress,
			StarRating = entity.StarRating,
			Reviews = entity.Reviews,
			LikesInPercent = entity.LikesInPercent,
			Likes = entity.Likes,
			Hours = entity.Hours,
			Authors = entity.Authors?.Select(x => new Author
			{
				Name = x.Name,
				AuthorImage = x.AuthorImage
			}).ToList(),
			Prices = entity.Prices == null ? null : new Prices
			{
				Currency = entity.Prices.Currency,
				Price = entity.Prices.Price,
				Discount = entity.Prices.Discount
			},
			Content = entity.Content == null ? null : new Content
			{
				Description = entity.Content.Description,
				Includes = entity.Content.Includes,
				ProgramDetails = entity.Content.ProgramDetails?.Select(x => new ProgramDetailItem
				{
					Id = x.Id,
					Title = x.Title,
					Description = x.Description
				}).ToList()
			}
		};
	}
}
