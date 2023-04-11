using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System.Net;
using TatBlog.Core.Collections;
using TatBlog.Core.Contracts;
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
             .Produces<ApiResponse<PaginationResult<CommentDto>>>();

    routeGroupBuilder.MapGet("/{id:int}", GetCommentByPostId)
             .WithName("GetCommentByPostId")
             .Produces<ApiResponse<PaginationResult<CommentDto>>>();

    routeGroupBuilder.MapPost("/", AddComment)
             .WithName("AddNewComment")
             .AddEndpointFilter<ValidatorFilter<CommentEditModel>>()
             .Produces(401)
             .Produces<ApiResponse<CommentDto>>();

    routeGroupBuilder.MapDelete("/{id:int}", DeleteComment)
             .WithName("DeleteComment")
             .Produces(401)
             .Produces<ApiResponse<string>>();

    routeGroupBuilder.MapPost("/toggle/{id:int}", ChangeCommentStatus)
             .WithName("ChangeCommentStatus")
             .Produces(401)
             .Produces<ApiResponse<string>>();

    routeGroupBuilder.MapGet("/get-filter", GetFilter)
                     .WithName("GetCommentFilter")
                     .Produces<ApiResponse<CommentFilterModel>>();

    return app;
  }

  private static async Task<IResult> GetComments([AsParameters] CommentFilterModel model, ICommentRepository commentRepository, IMapper mapper)
  {
    var commentQuery = mapper.Map<CommentQuery>(model);
    var commentList = await commentRepository.GetCommentByQueryAsync(commentQuery, model, comment => comment.ProjectToType<CommentDto>());

    var paginationResult = new PaginationResult<CommentDto>(commentList);

    return Results.Ok(ApiResponse.Success(paginationResult));
  }

  private static async Task<IResult> GetCommentByPostId(int id, [AsParameters] PagingModel pagingModel, ICommentRepository commentRepository, IMapper mapper)
  {
    var commentList = await commentRepository.GetCommentByPostIdAsync(id);

    var commentsDto = commentList.Select(c => mapper.Map<CommentDto>(c)).ToList();

    var paginationResult = new PaginationResult<CommentDto>(new PagedList<CommentDto>(commentsDto, pagingModel.PageNumber, pagingModel.PageSize, commentsDto.Count()));

    return Results.Ok(ApiResponse.Success(paginationResult));
  }

  private static async Task<IResult> AddComment(CommentEditModel model, ICommentRepository commentRepository, IMapper mapper)
  {
    var comment = mapper.Map<Comment>(model);
    await commentRepository.AddCommentAsync(comment);

    return Results.Ok(ApiResponse.Success(mapper.Map<CommentDto>(comment), HttpStatusCode.Created));
  }

  private static async Task<IResult> DeleteComment(int id, ICommentRepository commentRepository)
  {
    return await commentRepository.DeleteCommentByIdAsync(id) ? Results.Ok(ApiResponse.Success("Comment is deleted", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find comment with id = {id}"));
  }

  private static async Task<IResult> ChangeCommentStatus(int id, ICommentRepository commentRepository)
  {
    return await commentRepository.ChangeCommentStatusAsync(id) ? Results.Ok(ApiResponse.Success("Comment is switch consored", HttpStatusCode.NoContent)) : Results.Ok(ApiResponse.Fail(HttpStatusCode.NotFound, $"Could not find comment with id = {id}"));
  }

  private static async Task<IResult> GetFilter(IAuthorRepository authorRepository, ICategoryRepository categoryRepository)
  {
    return Results.Ok(ApiResponse.Success(await Task.FromResult(new CommentFilterModel())));
  }
}
