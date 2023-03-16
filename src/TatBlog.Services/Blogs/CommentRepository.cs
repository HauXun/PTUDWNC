using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Constants;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class CommentRepository : ICommentRepository
{
  private readonly BlogDbContext _blogContext;

  public CommentRepository(BlogDbContext dbContext)
  {
    _blogContext = dbContext;
  }

  public async Task<bool> AddCommentAsync(Comment comment, CancellationToken cancellationToken = default)
  {
    _blogContext.Add(comment);

    var result = await _blogContext.SaveChangesAsync(cancellationToken);
    return result > 0;
  }

  public async Task ChangeCommentStatusAsync(int id, CancellationToken cancellationToken = default)
  {
    await _blogContext.Set<Comment>()
                      .Where(x => x.Id == id)
                      .ExecuteUpdateAsync(c => c.SetProperty(x => x.Censored, x => !x.Censored), cancellationToken);
  }

  public async Task<IPagedList<Comment>> GetCommentByPostIdAsync(int postId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
  {
    var commentQuery = _blogContext.Set<Comment>()
                                   .Where(c => c.PostID.Equals(postId));

    commentQuery = commentQuery.Where(c => c.Censored);

    return await commentQuery.ToPagedListAsync(pageNumber,
                                               pageSize,
                                               nameof(Comment.PostDate),
                                               "DESC",
                                               cancellationToken);
  }

  public async Task<IPagedList<Comment>> GetCommentByQueryAsync(CommentQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
  {
    return await FilterComments(query).ToPagedListAsync(
                                      pageNumber,
                                      pageSize,
                                      nameof(Comment.PostDate),
                                      "DESC",
                                      cancellationToken);
  }

  private IQueryable<Comment> FilterComments(CommentQuery query)
  {
    IQueryable<Comment> commentQuery = _blogContext.Set<Comment>()
                                                   .Include(c => c.Post);

    if (query.Censored)
    {
      commentQuery = commentQuery.Where(x => x.Censored);
    }

    if (!string.IsNullOrWhiteSpace(query.UserName))
    {
      commentQuery = commentQuery.Where(x => x.UserName.Contains(query.UserName));
    }

    if (!string.IsNullOrWhiteSpace(query.PostTitle))
    {
      commentQuery = commentQuery.Where(x => x.Post.Title.Contains(query.PostTitle));
    }

    if (!string.IsNullOrWhiteSpace(query.Keyword))
    {
      commentQuery = commentQuery.Where(x => x.Content.Contains(query.Keyword));
    }

    if (query.Year > 0)
    {
      commentQuery = commentQuery.Where(x => x.PostDate.Year == query.Year);
    }

    if (query.Month > 0)
    {
      commentQuery = commentQuery.Where(x => x.PostDate.Month == query.Month);
    }

    if (query.Day > 0)
    {
      commentQuery = commentQuery.Where(x => x.PostDate.Day == query.Day);
    }

    return commentQuery;
  }

  public async Task<bool> DeleteCommentByIdAsync(int? id, CancellationToken cancellationToken = default)
  {
    var comment = await _blogContext.Set<Comment>().FindAsync(id);

    if (comment is null) return await Task.FromResult(false);

    _blogContext.Set<Comment>().Remove(comment);
    var rowsCount = await _blogContext.SaveChangesAsync(cancellationToken);

    return rowsCount > 0;
  }
}