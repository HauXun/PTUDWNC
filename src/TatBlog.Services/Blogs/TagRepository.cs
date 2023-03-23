using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class TagRepository : ITagRepository
{
  private readonly BlogDbContext _blogContext;
  
  public TagRepository(BlogDbContext dbContext)
  {
    _blogContext = dbContext;
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

  public async Task<Tag> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Tag>()
                            .Where(t => t.UrlSlug.Equals(slug))
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

  public async Task<bool> CheckTagSlugExisted(string slug, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Tag>().AnyAsync(t => t.UrlSlug.Equals(slug), cancellationToken);
  }

  public async Task<IPagedList<Tag>> GetTagByQueryAsync(TagQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
  {
    return await FilterTags(query).ToPagedListAsync(
                            pageNumber,
                            pageSize,
                            nameof(Tag.Name),
                            "DESC",
                            cancellationToken);
  }

  private IQueryable<Tag> FilterTags(TagQuery query)
  {
    IQueryable<Tag> categoryQuery = _blogContext.Set<Tag>()
                                                   .Include(c => c.Posts);

    if (!string.IsNullOrWhiteSpace(query.UrlSlug))
    {
      categoryQuery = categoryQuery.Where(x => x.UrlSlug == query.UrlSlug);
    }

    if (!string.IsNullOrWhiteSpace(query.Keyword))
    {
      categoryQuery = categoryQuery.Where(x => x.Name.Contains(query.Keyword) ||
                   x.Description.Contains(query.Keyword) ||
                   x.Posts.Any(p => p.Title.Contains(query.Keyword)));
    }

    return categoryQuery;
  }

  public async Task<Tag> GetTagByIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Tags.FindAsync(id, cancellationToken);
  }

  public async Task AddOrUpdateTagAsync(Tag tag, CancellationToken cancellationToken = default)
  {
    if (tag.Id > 0)
      _blogContext.Update(tag);
    else
      _blogContext.Add(tag);

    await _blogContext.SaveChangesAsync(cancellationToken);
  }
}