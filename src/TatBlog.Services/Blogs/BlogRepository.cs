using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class BlogRepository : IBlogRepository
{
	private readonly BlogDbContext _blogContext;
	private readonly ITagRepository _tagRepository;

	public BlogRepository(BlogDbContext dbContext, ITagRepository tagRepository)
	{
		_blogContext = dbContext;
		_tagRepository = tagRepository;
	}

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

			var tag = await _tagRepository.GetTagBySlugAsync(kv.Key, cancellationToken) ?? new Tag()
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

	public async Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default)
	{
		return await FilterPosts(query).ToPagedListAsync(
										pagingParams,
										cancellationToken);
	}

	public async Task<IPagedList<T>> GetPostByQueryAsync<T>(PostQuery query, IPagingParams pagingParams, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default)
	{
		IQueryable<T> result = mapper(FilterPosts(query));

		return await result.ToPagedListAsync(pagingParams, cancellationToken);
	}

	private IQueryable<Post> FilterPosts(PostQuery query)
	{
		IQueryable<Post> postsQuery = _blogContext.Set<Post>()
												  .Include(p => p.Author)
												  .Include(p => p.Category)
												  .Include(p => p.Tags);

		if (query.PublishedOnly)
		{
			postsQuery = postsQuery.Where(x => x.Published);
		}

		if (query.NotPublished)
		{
			postsQuery = postsQuery.Where(x => !x.Published);
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

	public async Task<bool> DeletePostByIdAsync(int? id, CancellationToken cancellationToken = default)
	{
		var post = await _blogContext.Set<Post>().FindAsync(id);

		if (post is null) return false;

		_blogContext.Set<Post>().Remove(post);
		var rowsCount = await _blogContext.SaveChangesAsync(cancellationToken);

		return rowsCount > 0;
	}
}
