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

  public async Task<Post> GetPostAsync(int year, int month, int day, string slug, CancellationToken cancellationToken = default)
  {
    IQueryable<Post> postsQuery = _blogContext.Set<Post>()
                                              .Include(x => x.Category)
                                              .Include(x => x.Author)
                                              .Include(x => x.Tags);

    if (year > 0)
    {
      postsQuery = postsQuery.Where(x => x.PostedDate.Year == year);
    }

    if (month > 0)
    {
      postsQuery = postsQuery.Where(x => x.PostedDate.Month == month);
    }

    if (day > 0)
    {
      postsQuery = postsQuery.Where(x => x.PostedDate.Day == day);
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

  public async Task<Post> GetPostByIdAsync(int id, bool published = false, CancellationToken cancellationToken = default)
  {
    IQueryable<Post> postQuery = _blogContext.Set<Post>()
                             .Include(p => p.Category)
                             .Include(p => p.Author)
                             .Include(p => p.Tags);

    if (published)
    {
      postQuery = postQuery.Where(x => x.Published);
    }

    return await postQuery.Where(p => p.Id.Equals(id))
                          .FirstOrDefaultAsync(cancellationToken);
  }

  public async Task AddOrUpdatePostAsync(Post post, IEnumerable<string> tags, CancellationToken cancellationToken = default)
  {
    if (post.Id > 0)
		{
			await _blogContext.Entry(post).Collection(x => x.Tags).LoadAsync(cancellationToken);
		}
		else
		{
			post.Tags = new List<Tag>();
		}

		var validTags = tags.Where(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => new
			{
				Name = x,
				Slug = x.GenerateSlug()
			})
			.GroupBy(x => x.Slug)
			.ToDictionary(g => g.Key, g => g.First().Name);

		foreach (var kv in validTags)
		{
			if (post.Tags.Any(x => string.Compare(x.UrlSlug, kv.Key, StringComparison.InvariantCultureIgnoreCase) == 0)) continue;

			var tag = await GetTagBySlugAsync(kv.Key, cancellationToken) ?? new Tag()
			{
				Name = kv.Value,
				Description = kv.Value,
				UrlSlug = kv.Key
			};

			post.Tags.Add(tag);
		}

		post.Tags = post.Tags.Where(t => validTags.ContainsKey(t.UrlSlug)).ToList();

		if (post.Id > 0)
			_blogContext.Update(post);
		else
			_blogContext.Add(post);

		await _blogContext.SaveChangesAsync(cancellationToken);
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

		if (query.PublishedOnly)
		{
			postsQuery = postsQuery.Where(x => x.Published == query.PublishedOnly);
		}

		if (query.CategoryId > 0)
		{
			postsQuery = postsQuery.Where(x => x.CategoryId == query.CategoryId);
		}
		if (query.AuthorId > 0)
		{
			postsQuery = postsQuery.Where(x => x.AuthorId == query.AuthorId);
		}

		if (!string.IsNullOrWhiteSpace(query.AuthorSlug))
		{
			postsQuery = postsQuery.Where(x => x.Author.UrlSlug == query.AuthorSlug);
		}

		if (!string.IsNullOrWhiteSpace(query.CategorySlug))
		{
			postsQuery = postsQuery.Where(x => x.Category.UrlSlug == query.CategorySlug);
		}

		if (!string.IsNullOrWhiteSpace(query.TagSlug))
		{
			postsQuery = postsQuery.Where(x => x.Tags.Any(t => t.UrlSlug == query.TagSlug));
		}

		if (!string.IsNullOrWhiteSpace(query.PostSlug))
		{
			postsQuery = postsQuery.Where(x => x.UrlSlug == query.PostSlug);
		}

		if (!string.IsNullOrWhiteSpace(query.Keyword))
		{
			postsQuery = postsQuery.Where(x => x.Title.Contains(query.Keyword) ||
									 x.ShortDescription.Contains(query.Keyword) ||
									 x.Description.Contains(query.Keyword) ||
									 x.Category.Name.Contains(query.Keyword) ||
									 x.Tags.Any(t => t.Name.Contains(query.Keyword)));
		}

		if (query.Year > 0)
		{
			postsQuery = postsQuery.Where(x => x.PostedDate.Year == query.Year);
		}

		if (query.Month > 0)
		{
			postsQuery = postsQuery.Where(x => x.PostedDate.Month == query.Month);
		}

		if (query.Day > 0)
		{
			postsQuery = postsQuery.Where(x => x.PostedDate.Day == query.Day);
		}

    query.GetTagListAsync();
    if (query.SelectedTag != null && query.SelectedTag.Count() > 0)
    {
      postsQuery = postsQuery.Where(p => query.SelectedTag.Intersect(p.Tags.Select(t => t.Name)).Count() > 0);
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

  public async Task<IList<AuthorItem>> GetAuthorsAsync(CancellationToken cancellationToken = default)
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

    return await tagQuery.ToListAsync(cancellationToken);
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

  public async Task<IList<Tag>> GetTagListAsync(CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Tag>()
                              .Select(x => new Tag()
                              {
                                Id = x.Id,
                                Name = x.Name,
                                UrlSlug = x.UrlSlug,
                                Description = x.Description,
                              }).ToListAsync(cancellationToken);
  }
}
