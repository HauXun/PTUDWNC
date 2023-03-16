using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Constants;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Extensions;

namespace TatBlog.Services.Blogs;

public class CategoryRepository : ICategoryRepository
{
  private readonly BlogDbContext _blogContext;

  public CategoryRepository(BlogDbContext dbContext)
  {
    _blogContext = dbContext;
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

  public async Task<Category> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Category>()
                            .Where(c => c.UrlSlug.Equals(slug))
                            .FirstOrDefaultAsync(cancellationToken);
  }

  public async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Category>().FindAsync(id);
  }

  public async Task AddOrUpdateCategoryAsync(Category category, CancellationToken cancellationToken = default)
  {
    if (category.Id > 0)
      _blogContext.Update(category);
    else
      _blogContext.Add(category);

    await _blogContext.SaveChangesAsync(cancellationToken);
  }

  public async Task DeleteCategoryByIdAsync(int? id, CancellationToken cancellationToken = default)
  {
    if (id == null || _blogContext.Categories == null)
    {
      Console.WriteLine("Không có chuyên mục nào");
      return;
    }

    var category = await _blogContext.Set<Category>().FindAsync(id);

    if (category != null)
    {
      _blogContext.Categories.Remove(category);
      await _blogContext.SaveChangesAsync(cancellationToken);

      Console.WriteLine($"Đã xóa chuyên mục với id {id}");
    }
  }

  public async Task<bool> CheckCategorySlugExisted(string slug, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Category>().AnyAsync(c => c.UrlSlug.Equals(slug), cancellationToken);
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
                                ShowOnMenu = x.ShowOnMenu,
                                PostCount = x.Posts.Count(p => p.Published)
                              });

    return await tagQuery.ToPagedListAsync(pagingParams, cancellationToken);
  }

  public async Task ChangeCategoryStatusAsync(int id, CancellationToken cancellationToken = default)
  {
    await _blogContext.Set<Category>()
                      .Where(x => x.Id == id)
                      .ExecuteUpdateAsync(c => c.SetProperty(x => x.ShowOnMenu, x => !x.ShowOnMenu), cancellationToken);
  }

  public async Task<IPagedList<Category>> GetCategoryByQueryAsync(CategoryQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
  {
    return await FilterCategories(query).ToPagedListAsync(
                            pageNumber,
                            pageSize,
                            nameof(Category.Name),
                            "DESC",
                            cancellationToken);
  }
  private IQueryable<Category> FilterCategories(CategoryQuery query)
  {
    IQueryable<Category> categoryQuery = _blogContext.Set<Category>()
                                              .Include(c => c.Posts);

    if (query.ShowOnMenu)
    {
      categoryQuery = categoryQuery.Where(x => x.ShowOnMenu);
    }

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
}