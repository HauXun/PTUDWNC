using FluentValidation;
using MapsterMapper;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class SubscriberEndpoints
{
	public static WebApplication MapSubscriberEndpoints(this WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup("/api/subscribers");

		// Nested Map with defined specific route
		routeGroupBuilder.MapGet("/", GetSubscribers)
						 .WithName("GetSubscribers")
						 .Produces<PaginationResult<Subscriber>>();

		routeGroupBuilder.MapGet("/{id:int}", GetSubscriberDetails)
						 .WithName("GetSubscriberById")
						 .Produces<Subscriber>()
						 .Produces(404);

		routeGroupBuilder.MapGet("/email/{email}", GetSubscriberByEmailDetails)
						 .WithName("GetSubscriberByEmail")
						 .Produces<Subscriber>()
						 .Produces(404);

		routeGroupBuilder.MapPost("/", Subscribe)
						 .WithName("NewSubscriber")
						 .AddEndpointFilter<ValidatorFilter<SubscriberEditModel>>()
						 .Produces(204)
						 .Produces(409);

		routeGroupBuilder.MapPost("/unsub/{email}", Unsubscribe)
						 .WithName("NewUnsubscriber")
						 .AddEndpointFilter<ValidatorFilter<SubscriberEditModel>>()
						 .Produces(204)
						 .Produces(409);

		routeGroupBuilder.MapDelete("/{id:int}", DeleteSubscriber)
						 .WithName("DeleteSubscriber")
						 .Produces(204)
						 .Produces(404);

		routeGroupBuilder.MapPost("/{id:int}", BlockSubscriber)
						 .WithName("BlockSubscriber")
						 .AddEndpointFilter<ValidatorFilter<SubscriberEditModel>>()
						 .Produces(204)
						 .Produces(404);

		return app;
	}

	private static async Task<IResult> GetSubscribers([AsParameters] SubscriberFilterModel model, ISubscriberRepository subscriberRepository, IMapper mapper)
	{
		var subscriberQuery = mapper.Map<SubscriberQuery>(model);
		var subscriberList = await subscriberRepository.GetSubscriberByQueryAsync(subscriberQuery, model);

		var paginationResult = new PaginationResult<Subscriber>(subscriberList);

		return Results.Ok(paginationResult);
	}

	private static async Task<IResult> GetSubscriberDetails(int id, ISubscriberRepository subscriberRepository)
	{
		var subscriber = await subscriberRepository.GetCachedSubscriberByIdAsync(id);

		return subscriber == null ? Results.NotFound($"Không tìm thấy người đăng kí có mã số {id}") : Results.Ok(subscriber);
	}

	private static async Task<IResult> GetSubscriberByEmailDetails(string email, ISubscriberRepository subscriberRepository)
	{
		var subscriber = await subscriberRepository.GetCachedSubscriberByEmailAsync(email);

		return subscriber == null ? Results.NotFound($"Không tìm thấy người đăng kí có email {email}") : Results.Ok(subscriber);
	}

	private static async Task<IResult> Subscribe(SubscriberEditModel model, ISubscriberRepository subscriberRepository, IMapper mapper)
	{
		var subscriber = mapper.Map<Subscriber>(model);
		var subscription = await subscriberRepository.SubscribeAsync(subscriber.SubscribeEmail);
		if (!subscription)
			return Results.Conflict($"Đã xảy ra lỗi khi đăng ký với email {subscriber.SubscribeEmail}!");

		return Results.NoContent();
	}

	private static async Task<IResult> Unsubscribe(string email, ISubscriberRepository subscriberRepository)
	{
		var subscription = await subscriberRepository.UnsubscribeAsync(email, "Không có nhu cầu nữa", true);
		if (!subscription)
			return Results.Conflict($"Đã xảy ra lỗi khi huỷ đăng ký cho email {email}!");

		return Results.NoContent();
	}

	private static async Task<IResult> DeleteSubscriber(int id, ISubscriberRepository subscriberRepository)
	{
		return await subscriberRepository.DeleteSubscriberAsync(Convert.ToInt32(id)) ? Results.NoContent() : Results.NotFound($"Could not find subscriber with id = {id}");
	}

	private static async Task<IResult> BlockSubscriber(int id, SubscriberEditModel model, ISubscriberRepository subscriberRepository, IMapper mapper)
	{
		var subscriber = mapper.Map<Subscriber>(model);
		subscriber.Id = id;

		return await subscriberRepository.BlockSubscriberAsync(id, subscriber.CancelReason, subscriber.AdminNotes) ? Results.NoContent() : Results.NotFound($"Could not find subscriber with id = {id}");
	}
}
