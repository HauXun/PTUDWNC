using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers;

public class BlogController : Controller
{
  private readonly IBlogRepository _blogRepository;

  public BlogController(IBlogRepository blogRepository)
  {
    _blogRepository = blogRepository;
  }

  public async Task<IActionResult> Index(
                            string title = null,
                            [FromQuery(Name = "p")] int pageNumber = 1,
                            [FromQuery(Name = "ps")] int pageSize = 10
                            )
  {
    // Tạo đối tượng chứa các điều kiện truy vấn
    var postQuery = new PostQuery
    {
      // Chỉ lấy những bài viết có trạng thái Published
      PublishedOnly = true,

      // Tìm kiếm bài viết theo tiêu đề
      Title = title
    };

    // Truy vấn các bài viết theo điều kiện đã tạo
    var postsList = await _blogRepository.GetPostByQueryAsync(postQuery, pageNumber, pageSize);

    // Lưu lại điều kiện truy vấn để hiển thị trong View
    ViewData["PostQuery"] = postQuery;

    return View(postsList);
  }

  public IActionResult About() => View();

  public IActionResult Contact() => View();

  public IActionResult Rss() => View("Nội dung sẽ được cập nhập");
}