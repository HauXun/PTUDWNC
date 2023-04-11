using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Extensions;
using TatBlog.WebApi.Filters;
using TatBlog.WebApi.Models;

namespace TatBlog.WebApi.Endpoints;

public static class CategoryEndpoints
{
	public static WebApplication MapCategoryEndpoints(this WebApplication app)
	{
		var routeGroupBuilder = app.MapGroup("/api/categories");

		// Nested Map with defined specific route
		routeGroupBuilder.MapGet("/", GetCategories)
						 .WithName("GetCategories")
						 .Produces<ApiResponse<PaginationResult<CategoryItem>>>();

		routeGroupBuilder.MapGet("/{id:int}", GetCategoryDetails)
						 .WithName("GetCategoryById")
						 .Produces<ApiResponse<CategoryItem>>();

		routeGroupBuilder.MapGet("/{slug::regex(^[a-z0-9_-]+$)}/posts", GetPostByCategorySlug)
						 .WithName("GetPostByCategorySlug")
						 .Produces<ApiResponse<PaginationResult<PostDto>>>();

		routeGroupBuilder.MapPost("/", AddCategory)
						 .WithName("AddNewCategory")
						 .Accepts<CategoryEditModel>("multipart/form-data")
                         //.AddEndpointFilter<ValidatorFilter<CategoryEditModel>>()
                         .Produces<ApiResponse<CategoryItem>>();

		routeGroupBuilder.MapDelete("/{id:int}", DeleteCategory)
						 .WithName("DeleteCategory")
						 .Produces<ApiResponse<string>>();

        routeGroupBuilder.MapPost("/show/switch/{id:int}", SwitchShowOn)
						 .WithName("SwitchShowOn")
						 .Produces(401)
						 .Produces<ApiResponse<string>>();

        return app;
	}

	private static async Task<IResult> GetCategories([AsParameters] CategoryFilterModel model, ICategoryRepository categoryRepository, IMapper mapper)
	{
		var categoryQuery = mapper.Map<CategoryQuery>(model);
		var categoryList = await categoryRepository.GetCategoryByQueryAsync(categoryQuery, model, category => category.ProjectToType<CategoryItem>());

		var paginationResult = new PaginationResult<CategoryItem>(categoryList);

		return Results.Ok(ApiResponse.Success(paginationResult));
	}

	private static async Task<IResult> GetCategoryDetails(int id, ICategoryRepository categoryRepository, IMapper mapper)
	{
		var category = await categoryRepository.GetCachedCategoryByIdAsync(id);

		return category == null ? Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Không tìm thấy chuyên mục có mã số {id}")) : Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category)));
	}

	private static async Task<IResult> GetPostByCategoryId(int id, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery
		{
			CategoryId = id,
			PublishedOnly = true
		};

		var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

		var paginationResult = new PaginationResult<PostDto>(postsList);

		return Results.Ok(ApiResponse.Success(paginationResult));
	}

	private static async Task<IResult> GetPostByCategorySlug([FromRoute] string slug, [AsParameters] PagingModel pagingModel, IBlogRepository blogRepository)
	{
		var postQuery = new PostQuery
		{
			CategorySlug = slug,
		};

		var postsList = await blogRepository.GetPostByQueryAsync(postQuery, pagingModel, posts => posts.ProjectToType<PostDto>());

		var paginationResult = new PaginationResult<PostDto>(postsList);

		return Results.Ok(ApiResponse.Success(paginationResult));
	}

	private static async Task<IResult> AddCategory(HttpContext context, ICategoryRepository categoryRepository, IMapper mapper)
    {
        var model = await CategoryEditModel.BindAsync(context);
        var slug = model.Name.GenerateSlug();

        if (await categoryRepository.CheckCategorySlugExisted(model.Id, slug))
		{
			return Results.Conflict($"Slug '{slug}' đã được sử dụng");
		}

        var category = model.Id > 0 ? await categoryRepository.GetCategoryByIdAsync(model.Id) : null;
        if (category == null)
        {
            category = new Category();
        }
        category.Name = model.Name;
        category.Description = model.Description;
        category.ShowOnMenu = model.ShowOnMenu;
        category.UrlSlug = slug;

        await categoryRepository.AddOrUpdateCategoryAsync(category);

        return Results.Ok(ApiResponse.Success(mapper.Map<CategoryItem>(category), HttpStatusCode.Created));
    }

	private static async Task<IResult> DeleteCategory(int id, ICategoryRepository categoryRepository)
    {
        return await categoryRepository.DeleteCategoryByIdAsync(id) ? Results.Ok(ApiResponse.Success("Category is deleted", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find category with id = {id}"));
    }

    private static async Task<IResult> SwitchShowOn(int id, ICategoryRepository categoryRepository)
    {
        return await categoryRepository.ChangeCategoryStatusAsync(id) ? Results.Ok(ApiResponse.Success("Category is switched show", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find category with id = {id}"));
    }
}
