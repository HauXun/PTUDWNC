using FluentValidation;
using FluentValidation.AspNetCore;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Areas.Admin.Controllers;

public class PostsController : Controller
{
  private readonly ILogger<PostsController> _logger;
  private readonly IBlogRepository _blogRepository;
  private readonly IMediaManager _mediaManager;
  private readonly IMapper _mapper;

  public PostsController(IBlogRepository blogRepository, IMapper mapper, IMediaManager mediaManager = null, ILogger<PostsController> logger = null)
  {
    _blogRepository = blogRepository;
    _mapper = mapper;
    _mediaManager = mediaManager;
    _logger = logger;
  }

  public async Task<IActionResult> Index(PostFilterModel model)
  {
    _logger.LogInformation("Tạo điều kiện truy vấn");

    // Sử dụng Mapster để tạo đối tượng PostQuery
    // từ đối tượng PostFilterModel
    var postQuery = _mapper.Map<PostQuery>(model);

    // var postQuery = new PostQuery{
    //   Keyword = model.Keyword,
    //   CategoryId = model.CategoryId,
    //   AuthorId = model.AuthorId,
    //   Year = model.Year,
    //   Month = model.Month,
    // };

    _logger.LogInformation("Lấy danh sách bài viết từ CSDL");

    ViewData["PostsList"] = await _blogRepository.GetPostByQueryAsync(postQuery);

    _logger.LogInformation("Chuẩn bị dữ liệu cho ViewModel");

    await PopulatePostFilterModelAsync(model);

    return View(model);
  }

  private async Task PopulatePostFilterModelAsync(PostFilterModel model)
  {
    var authors = await _blogRepository.GetAuthorsAsync();
    var categories = await _blogRepository.GetCategoriesAsync();

    model.AuthorList = authors.Select(a => new SelectListItem{
      Text = a.FullName,
      Value = a.Id.ToString()
    });

    model.CategoryList = categories.Select(c => new SelectListItem{
      Text = c.Name,
      Value = c.Id.ToString()
    });
  }

  [HttpGet]
  public async Task<ActionResult> Edit(int id = 0)
  {
    // ID = 0 -> Thêm bài viết mới
    // ID > 0 : Đọc dữ liệu của bài viết từ CSDL
    var post = id > 0 ? await _blogRepository.GetPostByIdAsync(id, true) : null;

    // Tạo view model từ dữ liệu của bài viết
    var model = post == null ? new PostEditModel() : _mapper.Map<PostEditModel>(post);

    // Gán các giá trị khác nhau cho view model
    await PopulatePostEditModelAsync(model);

    return View(model);
  }

  private async Task PopulatePostEditModelAsync(PostEditModel model)
  {
    var authors = await _blogRepository.GetAuthorsAsync();
    var categories = await _blogRepository.GetCategoriesAsync();

    model.AuthorList = authors.Select(a => new SelectListItem{
      Text = a.FullName,
      Value = a.Id.ToString()
    });

    model.CategoryList = categories.Select(c => new SelectListItem{
      Text = c.Name,
      Value = c.Id.ToString()
    });
  }

  [HttpPost]
  public async Task<ActionResult> Edit(IValidator<PostEditModel> postValidator, PostEditModel model)
  {
    var validator = HttpContext.RequestServices.GetService(typeof(IValidator<PostEditModel>)); 
    var validationResult = await postValidator.ValidateAsync(model);

    if (!validationResult.IsValid)
    {
      validationResult.AddToModelState(ModelState);
    }
    
    if (!ModelState.IsValid)
    {
      await PopulatePostEditModelAsync(model);
      return View(model);
    }

    var post = model.Id > 0 ? await _blogRepository.GetPostByIdAsync(model.Id) : null;

    if (post == null)
    {
      post = _mapper.Map<Post>(model);

      post.Id = 0;
      post.PostedDate = DateTime.Now;
    }
    else
    {
      _mapper.Map(model, post);

      post.Category = null;
      post.ModifiedDate = DateTime.Now;
    }

    // Nếu người dùng có upload hình ảnh minh họa cho bài viết
    if (model.ImageFile?.Length > 0)
    {
      // Thực hiện việc lưu tập tin vào thư mực uploads
      var newImagePath = await _mediaManager.SaveFileAsync(model.ImageFile.OpenReadStream(), model.ImageFile.FileName, model.ImageFile.ContentType);

      // Nếu lưu thành công, xóa tập tin hình ảnh cũ (nếu có)
      if (!string.IsNullOrWhiteSpace(newImagePath))
      {
        await _mediaManager.DeleteFileAsync(post.ImageUrl);
        post.ImageUrl = newImagePath;
      }
    }

    await _blogRepository.AddOrUpdatePostAsync(post, model.GetSelectedTags());

    return RedirectToAction(nameof(Index));
  }

  [HttpPost]
  public async Task<ActionResult> VerifyPostSlug(int id, string urlSlug)
  {
    var slugExisted = await _blogRepository.IsPostSlugExistedAsync(id, urlSlug);

    return slugExisted ? Json($"Slug '{urlSlug}' đã được sử dụng") : Json(true);
  }
}
