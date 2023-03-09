using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Components;

public class Archives : ViewComponent
{
  private readonly IBlogRepository _blogRepository;
  private readonly BlogDbContext _context;

  public Archives(IBlogRepository blogRepository, BlogDbContext context)
  {
    _blogRepository = blogRepository;
    _context = context;
  }

  public async Task<IViewComponentResult> InvokeAsync()
  {
    var last12months = (from r in Enumerable.Range(1,12) select DateTime.Now.AddMonths(0 - r))
                    .Select(x => new DateItem
                    {
                      Month = x.Month,
                      Year = x.Year
                    }).ToList();

    var postDate = await _context.Set<Post>().GroupBy(p => new 
                                                    {
                                                      p.PostedDate.Month, 
                                                      p.PostedDate.Year
                                                    })
                                                    .Select(p => new DateItem
                                                    {
                                                      Month = p.Key.Month,
                                                      Year = p.Key.Year,
                                                      PostCount = p.Count()
                                                    }).ToListAsync();

    ViewData["last12months"] = last12months;
    ViewData["postDate"] = postDate;
    
    return View();
  }
}
