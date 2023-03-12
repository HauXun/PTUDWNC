using FluentValidation;
using TatBlog.Services.Blogs;
using TatBlog.WebApp.Areas.Admin.Models;

namespace TatBlog.WebApp.Validations;

public class PostValidator : AbstractValidator<PostEditModel>
{
  private readonly IBlogRepository _blogRepository;

  public PostValidator(IBlogRepository blogRepository)
  {
    _blogRepository = blogRepository;

    RuleFor(p => p.Title)
    .NotEmpty()
    .WithMessage("Tiêu đề của bài viết không được để trống")
    .MaximumLength(500)
    .WithMessage("Tiêu đề dài tối đa '{MaxLength}'");

    RuleFor(p => p.ShortDescription)
    .NotEmpty()
    .WithMessage("Giới thiệu về bài viết không được để trống");

    RuleFor(p => p.Description)
    .NotEmpty()
    .WithMessage("Mô tả về bài viết không được để trống");

    RuleFor(p => p.Meta)
    .NotEmpty()
    .WithMessage("Meta của bài viết không được để trống")
    .MaximumLength(1000)
    .WithMessage("Meta dài tối đa '{MaxLength}'");

    RuleFor(p => p.UrlSlug)
    .NotEmpty()
    .WithMessage("Slug của bài viết không được để trống")
    .MaximumLength(1000)
    .WithMessage("Slug dài tối đa '{MaxLength}'");

    RuleFor(p => p.UrlSlug)
    .MustAsync(async (postModel, slug, cancellationToken) => !await _blogRepository.IsPostSlugExistedAsync(postModel.Id, slug, cancellationToken))
    .WithMessage("Slug '{PropertyValue}' đã được sử dụng");

    RuleFor(p => p.CategoryId)
    .NotEmpty()
    .WithMessage("Bạn phải chọn chủ đề cho bài viết");

    RuleFor(p => p.AuthorId)
    .NotEmpty()
    .WithMessage("Bạn phải chọn tác giả của bài viết");

    RuleFor(p => p.SelectedTags)
    .Must(HasAtLeastOneTag)
    .WithMessage("bạn phải nhập ít nhất một thẻ");

    When(p => p.Id <= 0, () => {
      RuleFor(p => p.ImageFile)
      .Must(f => f is { Length: > 0})
      .WithMessage("Bạn phải chọn hình ảnh cho bài viết");
    })
    .Otherwise(() => {
      RuleFor(p => p.ImageFile)
      .MustAsync(SetImageIfNotExist)
      .WithMessage("Bạn phải chọn hình ảnh cho bài viết");
    });
  }

  // Kiểm tra xem người dùng đã nhập ít nhất 1 thẻ (tag)
  private bool HasAtLeastOneTag(PostEditModel postModel, string selectedTags)
  {
    return postModel.GetSelectedTags().Any();
  }

  // Kiểm tra xem bài viết đã có hình ảnh chưa
  // Nếu chưa có, bắt buộc người dùng phải chọn file
  private async Task<bool> SetImageIfNotExist(PostEditModel postModel, IFormFile imageFile, CancellationToken cancellationToken)
  {
    // Lấy thông tin bài viết từ CSDL
    var post = await _blogRepository.GetPostByIdAsync(postModel.Id, false, cancellationToken);

    // Nếu bài viết đã có hình ảnh => Không bắt buộc chọn file
    if (!string.IsNullOrWhiteSpace(post?.ImageUrl))
      return true;

    // Ngược lại (bài viết chưa có hình ảnh), kiểm tra xem
    // người dùng đã chọn file hay chưa. Nếu chưa thì báo lỗi
    return imageFile is { Length: > 0 };
  }
}
