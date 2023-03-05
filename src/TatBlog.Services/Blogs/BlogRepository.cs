using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Constants;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class BlogRepository : IBlogRepository
{
  private readonly BlogDbContext _blogContext;
  public BlogRepository(BlogDbContext dbContext)
  {
    _blogContext = dbContext;
  }

  #region B

  public async Task<Post> GetPostAsync(int year, int month, string slug, CancellationToken cancellationToken = default)
  {
    IQueryable<Post> postsQuery = _blogContext.Set<Post>()
                                              .Include(x => x.Category)
                                              .Include(x => x.Author);

    if (year > 0)
    {
      postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
    }

    if (month > 0)
    {
      postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
    }

    if (!string.IsNullOrWhiteSpace(slug))
    {
      postsQuery = postsQuery.Where(x => x.UrlSlug == slug);
    }

    return await postsQuery.FirstOrDefaultAsync(cancellationToken);
  }

  public async Task<IList<Post>> GetPopularArticlesAsync(int numPosts, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Post>()
                             .Include(x => x.Author)
                             .Include(x => x.Category)
                             .OrderByDescending(p => p.ViewCount)
                             .Take(numPosts)
                             .ToListAsync(cancellationToken);
  }

  public async Task<bool> IsPostSlugExistedAsync(int postId, string slug, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Post>()
                             .AnyAsync(x => x.Id != postId && x.UrlSlug == slug, cancellationToken);
  }

  public async Task IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default)
  {
    await _blogContext.Set<Post>()
                      .Where(x => x.Id == postId)
                      .ExecuteUpdateAsync(p => p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1), cancellationToken);
  }

  public async Task<IList<CategoryItem>> GetCategoriesAsync(bool showOnMenu = false, CancellationToken cancellationToken = default)
  {
    IQueryable<Category> categories = _blogContext.Set<Category>();

    if (showOnMenu)
    {
      categories = categories.Where(x => x.ShowOnMenu);
    }

    return await categories.OrderByDescending(x => x.Name)
                          .Select(x => new CategoryItem()
                          {
                            Id = x.Id,
                            Name = x.Name,
                            UrlSlug = x.UrlSlug,
                            Description = x.Description,
                            ShowOnMenu = x.ShowOnMenu,
                            PostCount = x.Posts.Count(p => p.Published)
                          }).ToListAsync(cancellationToken);
  }

  public async Task<IPagedList<TagItem>> GetPagedTagsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default)
  {
    var tagQuery = _blogContext.Set<Tag>()
                              .Select(x => new TagItem()
                              {
                                Id = x.Id,
                                Name = x.Name,
                                UrlSlug = x.UrlSlug,
                                Description = x.Description,
                                PostCount = x.Posts.Count(p => p.Published)
                              });

    return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
  }

  #endregion

  #region 1.C

  public async Task<Tag> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Tag>()
                            .Where(t => t.UrlSlug.Contains(slug))
                            .FirstOrDefaultAsync(cancellationToken);
  }

  public async Task<IList<TagItem>> GetTagListWithPostCountAsync(CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Tag>()
                              .Select(x => new TagItem()
                              {
                                Id = x.Id,
                                Name = x.Name,
                                UrlSlug = x.UrlSlug,
                                Description = x.Description,
                                PostCount = x.Posts.Count()
                              }).ToListAsync(cancellationToken);
  }
  public async Task DeleteTagByIdAsync(int? id, CancellationToken cancellationToken = default)
  {
    if (id == null || _blogContext.Tags == null)
    {
      Console.WriteLine("Không có tag nào");
      return;
    }

    var tag = await _blogContext.Set<Tag>().FindAsync(id);

    if (tag != null)
    {
      Tag tagContext = tag;
      _blogContext.Tags.Remove(tagContext);
      await _blogContext.SaveChangesAsync(cancellationToken);

      Console.WriteLine($"Đã xóa tag với id {id}");
    }
  }

  public async Task<Category> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Categories.FindAsync(slug, cancellationToken);

    // return await _blogContext.Set<Category>()
    //                         .Where(c => c.UrlSlug.Contains(slug))
    //                         .FirstOrDefaultAsync(cancellationToken);
  }

  public async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Category>().FindAsync(id);
  }

  public async Task AddOrUpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
  {
    if (category?.Id == null || _blogContext.Categories == null)
    {
      await _blogContext.Categories.AddAsync(category, cancellationToken);
      await _blogContext.SaveChangesAsync(cancellationToken);
      return;
    }

    var cat = await _blogContext.Categories.FirstOrDefaultAsync(m => m.Id == category.Id);
    if (cat == null)
    {
      Console.WriteLine("Không có category nào để sửa");
      return;
    }

    cat.Name = category.Name;
    cat.Description = category.Description;
    cat.UrlSlug = category.UrlSlug;
    cat.ShowOnMenu = category.ShowOnMenu;

    _blogContext.Attach(cat).State = EntityState.Modified;
    await _blogContext.SaveChangesAsync(cancellationToken);
  }

  public async Task DeleteCategoryByIdAsync(int? id, CancellationToken cancellationToken = default)
  {
    if (id == null || _blogContext.Tags == null)
    {
      Console.WriteLine("Không có chuyên mục nào");
      return;
    }

    var tag = await _blogContext.Set<Tag>().FindAsync(id);

    if (tag != null)
    {
      _blogContext.Tags.Remove(tag);
      await _blogContext.SaveChangesAsync(cancellationToken);

      Console.WriteLine($"Đã xóa chuyên mục với id {id}");
    }
  }

  public async Task<bool> CheckCategorySlugExisted(string slug)
  {
    return await _blogContext.Set<Category>().AnyAsync(c => c.UrlSlug.Equals(slug));
  }

  public async Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default)
  {
    var tagQuery = _blogContext.Set<Category>()
                              .Select(x => new CategoryItem()
                              {
                                Id = x.Id,
                                Name = x.Name,
                                UrlSlug = x.UrlSlug,
                                Description = x.Description,
                                PostCount = x.Posts.Count(p => p.Published)
                              });

    return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
  }

  public async Task<IList<PostInMonthItem>> CountPostInMonthAsync(int monthCount, CancellationToken cancellationToken = default)
  {
    IQueryable<Post> postsQuery = _blogContext.Set<Post>()
                                                  .OrderByDescending(p => p.PostedDate);

    var topDate = await postsQuery.Select(p => p.PostedDate).FirstOrDefaultAsync();
    var subDate = topDate.AddMonths(-monthCount);
    postsQuery = postsQuery.Where(x => x.PostedDate >= subDate);

    var result = from p in postsQuery
                 group p by new
                 {
                   p.PostedDate.Year,
                   p.PostedDate.Month
                 } into postCount
                 select new PostInMonthItem
                 {
                   Count = postCount.Count(),
                   Year = postCount.Key.Year.ToString(),
                   Month = postCount.Key.Month.ToString()
                 };

    return await result.ToListAsync(cancellationToken);
  }

  public async Task<Post> GetPostByIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Post>().FindAsync(id);
  }

  public async Task AddOrUpdatePostAsync(Post post, CancellationToken cancellationToken = default)
  {
    if (post?.Id == null || _blogContext.Posts == null)
    {
      await _blogContext.Posts.AddAsync(post, cancellationToken);
      await _blogContext.SaveChangesAsync(cancellationToken);
      return;
    }

    var postGet = await _blogContext.Posts.FirstOrDefaultAsync(m => m.Id == post.Id);
    if (postGet == null)
    {
      Console.WriteLine("Không có post nào để sửa");
      return;
    }

    postGet.Title = post.Title;
    postGet.Description = post.Description;
    postGet.UrlSlug = post.UrlSlug;
    postGet.Published = post.Published;

    _blogContext.Attach(postGet).State = EntityState.Modified;
    await _blogContext.SaveChangesAsync();
  }

  public async Task ChangePostStatusAsync(int id, CancellationToken cancellationToken = default)
  {
    var post = await _blogContext.Posts.FindAsync(id);

    post.Published = !post.Published;

    _blogContext.Attach(post).State = EntityState.Modified;
    await _blogContext.SaveChangesAsync();
  }

  public async Task<IList<Post>> GetRandomPostAsync(int n, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Post>().OrderBy(p => Guid.NewGuid()).Take(n).ToListAsync(cancellationToken);
  }

  public async Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
  {
    return await FilterPosts(query).ToPagedListAsync(
                            pageNumber,
                            pageSize,
                            nameof(Post.PostedDate),
                            "DESC",
                            cancellationToken);
  }

  public async Task<IPagedList<T>> GetPostByQueryAsync<T>(PostQuery query, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default)
  {
    IQueryable<T> result = mapper(FilterPosts(query));

    return await result.ToPagedListAsync();
  }

  private IQueryable<Post> FilterPosts(PostQuery query)
  {
    IQueryable<Post> postsQuery = _blogContext.Set<Post>()
                                              .Include(p => p.Author)
                                              .Include(p => p.Category)
                                              .Include(p => p.Tags);

    if (!string.IsNullOrEmpty(query.Title))
    {
      postsQuery = postsQuery.Where(p => p.Title.Contains(query.Title));
    }
    if (!string.IsNullOrEmpty(query.PostSlug))
    {
      postsQuery = postsQuery.Where(p => p.UrlSlug.Contains(query.PostSlug));
    }
    if (query.PublishedOnly != null)
    {
      postsQuery = postsQuery.Where(p => p.Published.Equals(query.PublishedOnly));
    }
    if (!string.IsNullOrEmpty(query.Year))
    {
      postsQuery = postsQuery.Where(p => p.PostedDate.Date.Year.ToString().Equals(query.Year));
    }
    if (!string.IsNullOrEmpty(query.Month))
    {
      postsQuery = postsQuery.Where(p => p.PostedDate.Date.Month.ToString().Equals(query.Month));
    }
    if (!string.IsNullOrEmpty(query.Day))
    {
      postsQuery = postsQuery.Where(p => p.PostedDate.Date.Day.ToString().Equals(query.Day));
    }
    if (!string.IsNullOrEmpty(query.AuthorId))
    {
      postsQuery = postsQuery.Where(p => p.AuthorId.ToString().Equals(query.AuthorId));
    }
    if (!string.IsNullOrEmpty(query.CategoryId))
    {
      postsQuery = postsQuery.Where(p => p.CategoryId.ToString().Equals(query.CategoryId));
    }

    query.GetTagListAsync();
    if (query.SelectedTag != null && query.SelectedTag.Count() > 0)
    {
      var sameTag = query.SelectedTag.Intersect(query.SelectedTag);
      postsQuery = postsQuery.Where(p => query.SelectedTag.Any(t => sameTag.Contains(t)));
    }

    return postsQuery;
  }

  #endregion

  #region 2.C

  public async Task<Author> GetAuthorByIdAsync(int id, CancellationToken cancellationToken)
  {
    return await _blogContext.Authors.FindAsync(id, cancellationToken);
  }

  public async Task<Author> GetAuthorBySlugAsync(string slug, CancellationToken cancellationToken)
  {
    return await _blogContext.Authors.FindAsync(slug, cancellationToken);
  }

  public async Task<IPagedList<AuthorItem>> GetAuthorsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default)
  {
    var tagQuery = _blogContext.Set<Author>()
                              .Select(x => new AuthorItem()
                              {
                                Id = x.Id,
                                FullName = x.FullName,
                                UrlSlug = x.UrlSlug,
                                ImageUrl = x.ImageUrl,
                                JoinedDate = x.JoinedDate,
                                Notes = x.Notes,
                                PostCount = x.Posts.Count(p => p.Published)
                              });

    return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
  }

  public async Task AddOrUpdateAuthorAsync(Author author, CancellationToken cancellationToken = default)
  {
    if (author?.Id == null || _blogContext.Authors == null)
    {
      await _blogContext.Authors.AddAsync(author, cancellationToken);
      await _blogContext.SaveChangesAsync(cancellationToken);
      return;
    }

    var aut = await _blogContext.Authors.FirstOrDefaultAsync(a => a.Id == author.Id);
    if (aut == null)
    {
      Console.WriteLine("Không có author nào để sửa");
      return;
    }

    aut.FullName = author.FullName;
    aut.UrlSlug = author.UrlSlug;
    aut.JoinedDate = author.JoinedDate;
    aut.Email = author.Email;
    aut.Notes = author.Notes;

    _blogContext.Attach(aut).State = EntityState.Modified;
    await _blogContext.SaveChangesAsync();
  }

  public async Task<IList<Author>> Find_N_MostPostByAuthorAsync(int n, CancellationToken cancellationToken = default)
  {
    // var groupBy = (from author in _blogContext.Authors.ToList()
    //                join post in _blogContext.Posts.ToList()
    //                on author.Id equals post.AuthorId
    //                group author by post.AuthorId
    //                into newGroup
    //                select new
    //                {
    //                  AuthorId = newGroup.Key,
    //                  Count = newGroup.Count()
    //                }).OrderByDescending(x => x.Count).ToList();

    IQueryable<Author> authorsQuery = _blogContext.Set<Author>();
    IQueryable<Post> postsQuery = _blogContext.Set<Post>();

    return await authorsQuery.Join(postsQuery, a => a.Id, p => p.AuthorId,
                                  (author, post) => new
                                  {
                                    author.Id
                                  })
                             .GroupBy(x => x.Id)
                             .Select(x => new
                             {
                               AuthorId = x.Key,
                               Count = x.Count()
                             })
                             .OrderByDescending(x => x.Count)
                             .Take(n)
                             .Join(authorsQuery, a => a.AuthorId, a2 => a2.Id,
                              (preQuery, author) => new Author
                              {
                                Id = author.Id,
                                FullName = author.FullName,
                                UrlSlug = author.UrlSlug,
                                ImageUrl = author.ImageUrl,
                                JoinedDate = author.JoinedDate,
                                Notes = author.Notes,
                              }).ToListAsync();
  }

  #endregion
}
