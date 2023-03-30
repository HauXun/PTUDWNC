using TatBlog.Services.Blogs;

namespace TatBlog.WebApi.Endpoints;

public static class DashboardEndpoints
{
	public static WebApplication MapDashboardEndpoints(this WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup("/api/dashboard");

		// Nested Map with defined specific route
		routeGroupBuilder.MapGet("/total/posts", GetTotalOfPosts)
						 .WithName("GetTotalOfPosts")
						 .Produces<int>();

		routeGroupBuilder.MapGet("/total/posts/unpublished", GetTotalOfUnpublishedPosts)
						 .WithName("GetTotalOfUnpublishedPosts")
						 .Produces<int>();

		routeGroupBuilder.MapGet("/total/categories", GetTotalOfCategories)
						 .WithName("GetTotalOfCategories")
						 .Produces<int>();

		routeGroupBuilder.MapGet("/total/authors", GetTotalOfAuthors)
						 .WithName("GetTotalOfAuthors")
						 .Produces<int>();

		routeGroupBuilder.MapGet("/total/comments/waiting", GetTotalOfWaitingComment)
						 .WithName("GetTotalOfWaitingComment")
						 .Produces<int>();

		routeGroupBuilder.MapGet("/total/subscribers", GetTotalOfSubscriber)
						 .WithName("GetTotalOfSubscriber")
						 .Produces<int>();

		routeGroupBuilder.MapGet("/total/subscribers/newest", GetTotalOfNewestSubscriberInDay)
						 .WithName("GetTotalOfNewestSubscriberInDay")
						 .Produces<int>();

		return app;
	}

	private static async Task<IResult> GetTotalOfPosts(IDashboardRepository dashboardRepository)
	{
		var totalPost = await dashboardRepository.GetTotalOfPostsAsync();

		return Results.Ok(totalPost);
	}

	private static async Task<IResult> GetTotalOfUnpublishedPosts(IDashboardRepository dashboardRepository)
	{
		var totalPost = await dashboardRepository.GetTotalOfUnpublishedPostsAsync();

		return Results.Ok(totalPost);
	}

	private static async Task<IResult> GetTotalOfCategories(IDashboardRepository dashboardRepository)
	{
		var totalCategory = await dashboardRepository.GetTotalOfCategoriesAsync();

		return Results.Ok(totalCategory);
	}

	private static async Task<IResult> GetTotalOfAuthors(IDashboardRepository dashboardRepository)
	{
		var totalAuthor = await dashboardRepository.GetTotalOfAuthorsAsync();

		return Results.Ok(totalAuthor);
	}

	private static async Task<IResult> GetTotalOfWaitingComment(IDashboardRepository dashboardRepository)
	{
		var total = await dashboardRepository.GetTotalOfWaitingCommentAsync();

		return Results.Ok(total);
	}

	private static async Task<IResult> GetTotalOfSubscriber(IDashboardRepository dashboardRepository)
	{
		var totalSubscriber = await dashboardRepository.GetTotalOfSubscriberAsync();

		return Results.Ok(totalSubscriber);
	}

	private static async Task<IResult> GetTotalOfNewestSubscriberInDay(IDashboardRepository dashboardRepository)
	{
		var total = await dashboardRepository.GetTotalOfNewestSubscriberInDayAsync();

		return Results.Ok(total);
	}
}
