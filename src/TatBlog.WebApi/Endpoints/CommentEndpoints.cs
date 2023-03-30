using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class CommentEndpoints
{
	public static WebApplication MapCommentEndpoints(this WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup("/api/comments");

		// Nested Map with defined specific route
		routeGroupBuilder.MapGet("/", GetComments)
						 .WithName("GetComments")
						 .Produces<PaginationResult<Comment>>();

		routeGroupBuilder.MapGet("/{id:int}", GetCommentByPostId)
						 .WithName("GetCommentByPostId")
						 .Produces<PaginationResult<Comment>>();

		routeGroupBuilder.MapPost("/", AddComment)
						 .WithName("AddNewComment")
						 .AddEndpointFilter<ValidatorFilter<CommentEditModel>>()
						 .Produces(201)
						 .Produces(400)
						 .Produces(409);

		routeGroupBuilder.MapDelete("/{id:int}", DeleteComment)
						 .WithName("DeleteComment")
						 .Produces(204)
						 .Produces(404);

		routeGroupBuilder.MapPost("/toggle/{id:int}", ChangeCommentStatus)
						 .WithName("ChangeCommentStatus")
						 .Accepts<IFormFile>("multipart/formdata")
						 .Produces<string>()
						 .Produces(400);

		return app;
	}

	private static async Task<IResult> GetComments([AsParameters] CommentFilterModel model, ICommentRepository commentRepository, IMapper mapper)
	{
		var commentQuery = mapper.Map<CommentQuery>(model);
		var commentList = await commentRepository.GetCommentByQueryAsync(commentQuery, model);

		var paginationResult = new PaginationResult<Comment>(commentList);

		return Results.Ok(paginationResult);
	}

	private static async Task<IResult> GetCommentByPostId(int id, ICommentRepository commentRepository)
	{
		var commentList = await commentRepository.GetCommentByPostIdAsync(id);

		var paginationResult = new PaginationResult<Comment>(commentList);

		return Results.Ok(paginationResult);
	}

	private static async Task<IResult> AddComment(CommentEditModel model, ICommentRepository commentRepository, IMapper mapper)
	{
		var comment = mapper.Map<Comment>(model);
		await commentRepository.AddCommentAsync(comment);

		return Results.CreatedAtRoute("GetCommentById", new { comment.Id }, mapper.Map<Comment>(comment));
	}

	private static async Task<IResult> DeleteComment(int id, ICommentRepository commentRepository)
	{
		return await commentRepository.DeleteCommentByIdAsync(id) ? Results.NoContent() : Results.NotFound($"Could not find comment with id = {id}");
	}

	private static async Task<IResult> ChangeCommentStatus(int id, ICommentRepository commentRepository)
	{
		await commentRepository.ChangeCommentStatusAsync(id);

		return Results.NoContent();
	}
}
