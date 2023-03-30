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

public static class AuthorEndpoints
{
	public static WebApplication MapAuthorEndpoints(this WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup("/api/authors");

		// Nested Map with defined specific route
		routeGroupBuilder.MapGet("/", GetAuthors)
						 .WithName("GetAuthors")
						 .Produces<PaginationResult<AuthorItem>>();

		routeGroupBuilder.MapGet("/{id:int}", GetAuthorDetails)
						 .WithName("GetAuthorById")
						 .Produces<AuthorItem>()
						 .Produces(404);

		routeGroupBuilder.MapGet("/{slug::regex(^[a-z0-9_-]+$)}/posts", GetPostByAuthorSlug)
						 .WithName("GetPostByAuthorSlug")
						 .Produces<PaginationResult<PostDto>>();

		routeGroupBuilder.MapPost("/", AddAuthor)
						 .WithName("AddNewAuthor")
						 .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
						 .Produces(201)
						 .Produces(400)
						 .Produces(409);

		routeGroupBuilder.MapPut("/{id:int}", UpdateAuthor)
						 .WithName("UpdateAuthor")
						 .AddEndpointFilter<ValidatorFilter<AuthorEditModel>>()
						 .Produces(204)
						 .Produces(400)
						 .Produces(409);

		routeGroupBuilder.MapDelete("/{id:int}", DeleteAuthor)
						 .WithName("DeleteAuthor")
						 .Produces(204)
						 .Produces(404);

		routeGroupBuilder.MapPost("/{id:int}/avatar", SetAuthorPicture)
						 .WithName("SetAuthorPicture")
						 .Accepts<IFormFile>("multipart/formdata")
						 .Produces<string>()
						 .Produces(400);

		routeGroupBuilder.MapGet("/best/{limit:int}", GetBestAuthors)
						 .WithName("GetBestAuthors")
						 .Produces<IList<Author>>();

		return app;
	}

	private static async Task<IResult> GetAuthors([AsParameters] AuthorFilterModel model, IAuthorRepository authorRepository, IMapper mapper)
	{
		var authorQuery = mapper.Map<AuthorQuery>(model);
		var authorList = await authorRepository.GetAuthorByQueryAsync(authorQuery, model, author => author.ProjectToType<AuthorItem>());

		var paginationResult = new PaginationResult<AuthorItem>(authorList);

		return Results.Ok(paginationResult);
	}

	private static async Task<IResult> GetAuthorDetails(int id, IAuthorRepository authorRepository, IMapper mapper)
	{
		var author = await authorRepository.GetCachedAuthorByIdAsync(id);

		return author == null ? Results.NotFound($"Không tìm thấy tác giả có mã số {id}") : Results.Ok(mapper.Map<AuthorItem>(author));
	}

	private static async Task<IResult> GetPostByAuthorId(int id, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery
		{
			AuthorId = id,
			PublishedOnly = true
		};

		var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

		var paginationResult = new PaginationResult<PostDto>(postsList);

		return Results.Ok(paginationResult);
	}

	private static async Task<IResult> GetPostByAuthorSlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery
		{
			AuthorSlug = slug,
			PublishedOnly = true
		};

		var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

		var paginationResult = new PaginationResult<PostDto>(postsList);

		return Results.Ok(paginationResult);
	}

	private static async Task<IResult> AddAuthor(AuthorEditModel model, IAuthorRepository authorRepository, IMapper mapper)
	{
		if (await authorRepository.CheckAuthorSlugExisted(0, model.UrlSlug))
		{
			return Results.Conflict($"Slug '{model.UrlSlug}' đã được sử dụng");
		}

		var author = mapper.Map<Author>(model);
		await authorRepository.AddOrUpdateAuthorAsync(author);

		return Results.CreatedAtRoute("GetAuthorById", new { author.Id }, mapper.Map<AuthorItem>(author));
	}

	private static async Task<IResult> UpdateAuthor(int id, AuthorEditModel model, IAuthorRepository authorRepository, IMapper mapper)
	{
		if (await authorRepository.CheckAuthorSlugExisted(id, model.UrlSlug))
		{
			return Results.Conflict($"Slug '{model.UrlSlug}' đã được sử dụng");
		}

		var author = mapper.Map<Author>(model);
		author.Id = id;

		return await authorRepository.AddOrUpdateAuthorAsync(author) ? Results.NoContent() : Results.NotFound();
	}

	private static async Task<IResult> DeleteAuthor(int id, IAuthorRepository authorRepository)
	{
		return await authorRepository.DeleteAuthorByIdAsync(id) ? Results.NoContent() : Results.NotFound($"Could not find author with id = {id}");
	}

	private static async Task<IResult> SetAuthorPicture(int id, IFormFile imageFile, IAuthorRepository authorRepository, IMediaManager mediaManager)
	{
		var imageUrl = await mediaManager.SaveFileAsync(imageFile.OpenReadStream(), imageFile.FileName, imageFile.ContentType);

		if (string.IsNullOrWhiteSpace(imageUrl))
		{
			return Results.BadRequest("Không lưu được tập tin");
		}

		await mediaManager.DeleteFileAsync(imageUrl);
		await authorRepository.SetImageUrlAsync(id, imageUrl);

		return Results.Ok(imageUrl);
	}

	private static async Task<IResult> GetBestAuthors(int limit, IAuthorRepository authorRepository)
	{
		var authors = await authorRepository.Find_N_MostPostByAuthorAsync(limit);

		return Results.Ok(authors);
	}
}
