using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class AuthorRepository : IAuthorRepository
{
	private readonly BlogDbContext _blogContext;
	private readonly IMemoryCache _memoryCache;

	public AuthorRepository(BlogDbContext dbContext, IMemoryCache memoryCache)
	{
		_blogContext = dbContext;
		_memoryCache = memoryCache;
	}

	public async Task<Author> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default)
	{
		return await _blogContext.Authors.FindAsync(id, cancellationToken);
	}

	public async Task<Author> GetCachedAuthorByIdAsync(int authorId)
	{
		return await _memoryCache.GetOrCreateAsync(
			$"author.by-id.{authorId}",
			async (entry) =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
				return await GetAuthorByIdAsync(authorId);
			});
	}

	public async Task<Author> GetAuthorBySlugAsync(string slug, CancellationToken cancellationToken)
	{
		return await _blogContext.Set<Author>()
								 .Where(a => a.UrlSlug.Equals(slug))
								 .FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<Author> GetCachedAuthorBySlugAsync(
		string slug, CancellationToken cancellationToken = default)
	{
		return await _memoryCache.GetOrCreateAsync(
			$"author.by-slug.{slug}",
			async (entry) =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
				return await GetAuthorBySlugAsync(slug, cancellationToken);
			});
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

	public async Task<IPagedList<AuthorItem>> GetPagedAuthorsAsync(
		IPagingParams pagingParams,
		string name = null,
		CancellationToken cancellationToken = default)
	{
		IQueryable<Author> authorQuery = _blogContext.Set<Author>().AsNoTracking();

		if (!string.IsNullOrWhiteSpace(name))
		{
			authorQuery = authorQuery.Where(x => x.FullName.Contains(name));
		}

		return await authorQuery.Select(a => new AuthorItem()
								{
									Id = a.Id,
									FullName = a.FullName,
									Email = a.Email,
									JoinedDate = a.JoinedDate,
									ImageUrl = a.ImageUrl,
									UrlSlug = a.UrlSlug,
									PostCount = a.Posts.Count(p => p.Published)
								})
								.ToPagedListAsync(pagingParams, cancellationToken);
	}

	public async Task<IPagedList<T>> GetPagedAuthorsAsync<T>(
		Func<IQueryable<Author>, IQueryable<T>> mapper,
		IPagingParams pagingParams,
		string name = null,
		CancellationToken cancellationToken = default)
	{
		var authorQuery = _blogContext.Set<Author>().AsNoTracking();

		if (!string.IsNullOrEmpty(name))
		{
			authorQuery = authorQuery.Where(x => x.FullName.Contains(name));
		}

		return await mapper(authorQuery)
			.ToPagedListAsync(pagingParams, cancellationToken);
	}

	public async Task<bool> AddOrUpdateAuthorAsync(Author author, CancellationToken cancellationToken = default)
	{
		if (author.Id > 0)
			_blogContext.Update(author);
		else
			_blogContext.Add(author);

		var result = await _blogContext.SaveChangesAsync(cancellationToken);

		return result > 0;
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

	public async Task<bool> CheckAuthorSlugExisted(int id, string slug, CancellationToken cancellationToken = default)
	{
		return await _blogContext.Set<Author>()
			.AnyAsync(x => x.Id != id && x.UrlSlug == slug, cancellationToken);
	}

	public async Task<IPagedList<Author>> GetAuthorByQueryAsync(AuthorQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
	{
		return await FilterAuthors(query).ToPagedListAsync(
								pageNumber,
								pageSize,
								nameof(AuthorQuery.FullName),
								"DESC",
								cancellationToken);
	}

	private IQueryable<Author> FilterAuthors(AuthorQuery query)
	{
		IQueryable<Author> categoryQuery = _blogContext.Set<Author>()
													   .Include(c => c.Posts);

		if (!string.IsNullOrWhiteSpace(query.UrlSlug))
		{
			categoryQuery = categoryQuery.Where(x => x.UrlSlug == query.UrlSlug);
		}

		if (!string.IsNullOrWhiteSpace(query.Email))
		{
			categoryQuery = categoryQuery.Where(x => x.Email.Contains(query.Email));
		}

		if (!string.IsNullOrWhiteSpace(query.Keyword))
		{
			categoryQuery = categoryQuery.Where(x => x.FullName.Contains(query.Keyword) ||
						 x.Notes.Contains(query.Keyword) ||
						 x.Posts.Any(p => p.Title.Contains(query.Keyword)));
		}

		return categoryQuery;
	}

	public async Task<bool> DeleteAuthorByIdAsync(int? id, CancellationToken cancellationToken = default)
	{
		var author = await _blogContext.Set<Author>().FindAsync(id);

		if (author is null) return await Task.FromResult(false);

		_blogContext.Set<Author>().Remove(author);
		var rowsCount = await _blogContext.SaveChangesAsync(cancellationToken);

		return rowsCount > 0;
	}

	public async Task<bool> SetImageUrlAsync(
		int authorId, string imageUrl,
		CancellationToken cancellationToken = default)
	{
		return await _blogContext.Authors
			.Where(x => x.Id == authorId)
			.ExecuteUpdateAsync(x =>
				x.SetProperty(a => a.ImageUrl, a => imageUrl),
				cancellationToken) > 0;
	}

}