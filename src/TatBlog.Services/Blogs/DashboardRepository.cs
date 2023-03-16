using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Services.Blogs;

public class DashboardRepository : IDashboardRepository
{
  private readonly BlogDbContext _blogContext;

  public DashboardRepository(BlogDbContext dbContext)
  {
    _blogContext = dbContext;
  }

  public async Task<int> GetTotalOfAuthorsAsync()
  {
    return await _blogContext.Set<Author>().CountAsync();
  }

  public async Task<int> GetTotalOfCategoriesAsync()
  {
    return await _blogContext.Set<Category>().CountAsync();
  }

  public async Task<int> GetTotalOfNewestSubscriberInDayAsync()
  {
    return await _blogContext.Set<Subscriber>().CountAsync(s => s.SubDated.Day.Equals(DateTime.Now.Day));
  }

  public async Task<int> GetTotalOfPostsAsync()
  {
    return await _blogContext.Set<Post>().CountAsync();
  }

  public async Task<int> GetTotalOfSubscriberAsync()
  {
    return await _blogContext.Set<Subscriber>().CountAsync();
  }

  public async Task<int> GetTotalOfUnpublishedPostsAsync()
  {
    return await _blogContext.Set<Post>().CountAsync(p => !p.Published);
  }

  public async Task<int> GetTotalOfWaitingCommentAsync()
  {
    return await _blogContext.Set<Comment>().CountAsync(c => !c.Censored);
  }
}