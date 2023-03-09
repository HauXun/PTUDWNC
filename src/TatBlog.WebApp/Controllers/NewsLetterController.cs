using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TatBlog.Services.Blogs;

namespace TatBlog.WebApp.Controllers;

public class NewsLetterController: Controller
{
  private readonly IBlogRepository _blogRepository;

  public NewsLetterController(IBlogRepository blogRepository)
  {
    _blogRepository = blogRepository;
  }

  public NewsLetterController()
  {
    
  }

  [Required(ErrorMessage = "Phải nhập {0}")]
  [EmailAddress(ErrorMessage = "Sai định dạng {0}")]
  [Display(Name = "Email")]
  public string Email { get; set; }

  public async Task<IActionResult> Subscribe(string email)
  {

    return RedirectToPage("/");
  }

  public async Task<IActionResult> Unsubscribe(string email)
  {

    return RedirectToPage("/");
  }
}