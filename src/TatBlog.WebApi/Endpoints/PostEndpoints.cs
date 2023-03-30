﻿using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class PostEndpoints
{
	public static WebApplication MapPostEndpoints(this WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup("/api/posts");

		// Nested Map with defined specific route
		routeGroupBuilder.MapGet("/", GetPosts)
						 .WithName("GetPosts")
						 .Produces<ApiResponse<PaginationResult<PostItem>>>();

		routeGroupBuilder.MapGet("/featured/{limit:int}", GetFeaturedPost)
						 .WithName("GetFeaturedPost")
						 .Produces<ApiResponse<IList<Post>>>();

		routeGroupBuilder.MapGet("/random/{limit:int}", GetRandomPost)
						 .WithName("GetRandomPost")
						 .Produces<ApiResponse<IList<Post>>>();

		routeGroupBuilder.MapGet("/archives/{limit:int}", GetArchivesPost)
						 .WithName("GetArchivesPost")
						 .Produces<ApiResponse<IList<DateItem>>>();

		routeGroupBuilder.MapGet("/{id:int}", GetPostDetails)
						 .WithName("GetPostById")
						 .Produces<ApiResponse<PostDto>>();

		routeGroupBuilder.MapGet("/byslug/{slug::regex(^[a-z0-9_-]+$)}", GetPostBySlug)
						 .WithName("GetPostBySlug")
						 .Produces<ApiResponse<PaginationResult<PostDto>>>();

		routeGroupBuilder.MapPost("/", AddPost)
						 .WithName("AddNewPost")
						 .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
						 .Produces(401)
						 .Produces<ApiResponse<PostItem>>();

		routeGroupBuilder.MapPut("/{id:int}", UpdatePost)
						 .WithName("UpdatePost")
						 .AddEndpointFilter<ValidatorFilter<PostEditModel>>()
						 .Produces(401)
						 .Produces<ApiResponse<string>>();

		routeGroupBuilder.MapDelete("/{id:int}", DeletePost)
						 .WithName("DeletePost")
						 .Produces(401)
						 .Produces<ApiResponse<string>>();

		routeGroupBuilder.MapPost("/{id:int}/picture", SetPostPicture)
						 .WithName("SetPostPicture")
						 .Accepts<IFormFile>("multipart/formdata")
						 .Produces(401)
						 .Produces<string>();

		routeGroupBuilder.MapGet("/{id:int}/comments", GetCommentOfPost)
						 .WithName("GetCommentOfPost")
						 .Produces<ApiResponse<PaginationResult<Comment>>>();

		return app;
	}

	private static async Task<IResult> GetPosts([AsParameters] PostFilterModel model, IBlogRepository blogRepository, IMapper mapper)
	{
		var postQuery = mapper.Map<PostQuery>(model);
		var postList = await blogRepository.GetPostByQueryAsync(postQuery, model, post => post.ProjectToType<PostItem>());

		var paginationResult = new PaginationResult<PostItem>(postList);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }

	private static async Task<IResult> GetFeaturedPost(int limit, IBlogRepository blogRepository)
	{
		var posts = await blogRepository.GetPopularArticlesAsync(limit);

        return Results.Ok(ApiResponse.Success(posts));
    }

	private static async Task<IResult> GetRandomPost(int limit, IBlogRepository blogRepository)
	{
		var posts = await blogRepository.GetRandomPostAsync(limit);

        return Results.Ok(ApiResponse.Success(posts));
    }

	private static async Task<IResult> GetArchivesPost(int limit, IBlogRepository blogRepository)
	{
		var postDate = await blogRepository.GetArchivesPostAsync(limit);

        return Results.Ok(ApiResponse.Success(postDate));
    }

	private static async Task<IResult> GetPostDetails(int id, IBlogRepository blogRepository, IMapper mapper)
	{
		var post = await blogRepository.GetCachedPostByIdAsync(id);

		return post == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy bài viết có mã số {id}")) : Results.Ok(ApiResponse.Success(mapper.Map<PostItem>(post)));
	}

	private static async Task<IResult> GetPostBySlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery
		{
			PostSlug = slug,
			PublishedOnly = true
		};

		var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

		var post = postsList.FirstOrDefault();

        return Results.Ok(ApiResponse.Success(post));
    }

	private static async Task<IResult> AddPost(PostEditModel model, IBlogRepository blogRepository, IMapper mapper)
	{
		if (await blogRepository.IsPostSlugExistedAsync(0, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug '{model.UrlSlug}' đã được sử dụng"));
        }

		var post = mapper.Map<Post>(model);
		await blogRepository.AddOrUpdatePostAsync(post, model.GetSelectedTags());

        return Results.Ok(ApiResponse.Success(mapper.Map<PostItem>(post), HttpStatusCode.Created));
    }

	private static async Task<IResult> UpdatePost(int id, PostEditModel model, IBlogRepository blogRepository, IMapper mapper)
	{
		if (await blogRepository.IsPostSlugExistedAsync(id, model.UrlSlug))
        {
            return Results.Ok(ApiResponse.Fail(HttpStatusCode.Conflict, $"Slug '{model.UrlSlug}' đã được sử dụng"));
        }

		var post = mapper.Map<Post>(model);
		post.Id = id;

        return await blogRepository.AddOrUpdatePostAsync(post, model.GetSelectedTags()) ? Results.Ok(ApiResponse.Success("Post is updated", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, "Could not found post"));
    }

	private static async Task<IResult> DeletePost(int id, IBlogRepository blogRepository)
    {
        return await blogRepository.DeletePostByIdAsync(id) ? Results.Ok(ApiResponse.Success("Post is deleted", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find post with id = {id}"));
    }

	private static async Task<IResult> SetPostPicture(int id, IFormFile imageFile, IBlogRepository blogRepository, IMediaManager mediaManager)
	{
		var post = await blogRepository.GetCachedPostByIdAsync(id);
		string newImagePath = string.Empty;

		// Nếu người dùng có upload hình ảnh minh họa cho bài viết
		if (imageFile?.Length > 0)
		{
			// Thực hiện việc lưu tập tin vào thư mực uploads
			newImagePath = await mediaManager.SaveFileAsync(imageFile.OpenReadStream(), imageFile.FileName, imageFile.ContentType);

			if (string.IsNullOrWhiteSpace(newImagePath))
			{
                return Results.Ok(ApiResponse.Fail(HttpStatusCode.BadRequest, "Không lưu được tập tin"));
            }

			// Nếu lưu thành công, xóa tập tin hình ảnh cũ (nếu có)
			await mediaManager.DeleteFileAsync(post.ImageUrl);
			post.ImageUrl = newImagePath;
		}

        return Results.Ok(ApiResponse.Success(newImagePath));
    }

	private static async Task<IResult> GetCommentOfPost(int id, ICommentRepository commentRepository)
	{
		var comments = await commentRepository.GetCommentByPostIdAsync(id);

		var paginationResult = new PaginationResult<Comment>(comments);

        return Results.Ok(ApiResponse.Success(paginationResult));
    }
}
