using TatBlog.Core.Constants;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface IBlogRepository
{
  #region B

  // Tìm bài viết có tên định danh là 'slug
  // và được đăng vào tháng 'month' năm 'year'
  Task<Post> GetPostAsync(int year, int month, string slug, CancellationToken cancellationToken = default);

  // Tìm top (n) bài viết phổ biến được nhiều người xem nhất
  Task<IList<Post>> GetPopularArticlesAsync(int numPosts, CancellationToken cancellationToken = default);

  // Kiểm tra xem tên định danh của bài viết đã có hay chưa
  Task<bool> IsPostSlugExistedAsync(int postId, string slug, CancellationToken cancellationToken = default);

  // Tăng số lượt xem của một bài viết
  Task IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default);

  // Lấy danh sách chuyên mục và số lượng bài viết
  // nằm thuộc từng chuyên mục/ chủ đề
  Task<IList<CategoryItem>> GetCategoriesAsync(bool showOnMenu = false, CancellationToken cancellationToken = default);

  // Lấy danh sách từ khóa/ thẻ và phân trang theo
  // các tham số pagingParams
  Task<IPagedList<TagItem>> GetPagedTagsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default);

  #endregion

  #region 1.C

  // a. Tìm một thẻ (Tag) theo tên định danh (slug) 
  Task<Tag> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default);

  // c. Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó. Kết
  // quả trả về kiểu IList<TagItem>.
  Task<IList<TagItem>> GetTagListWithPostCountAsync(CancellationToken cancellationToken = default);

  // d. Xóa một thẻ theo mã cho trước. 
  Task DeleteTagByIdAsync(int? id, CancellationToken cancellationToken = default);

  // e. Tìm một chuyên mục (Category) theo tên định danh (slug)
  Task<Category> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default);

  // f. Tìm một chuyên mục theo mã số cho trước
  Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellationToken = default);

  // g. Thêm hoặc cập nhật một chuyên mục/chủ đề
  Task AddOrUpdateCategoryAsync(Category category, CancellationToken cancellationToken = default);

  // h. Xóa một chuyên mục theo mã số cho trước. 
  Task DeleteCategoryByIdAsync(int? id, CancellationToken cancellationToken = default);

  // i. Kiểm tra tên định danh (slug) của một chuyên mục đã tồn tại hay chưa. 
  Task<bool> CheckCategorySlugExisted(string slug);

  // j. Lấy và phân trang danh sách chuyên mục, kết quả trả về kiểu
  // IPagedList<CategoryItem>.
  Task<IPagedList<CategoryItem>> GetPagedCategoriesAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default);

  // k. Đếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào. Kết
  // quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số
  // bài viết.
  Task<IList<PostInMonthItem>> CountPostInMonthAsync(int monthCount, CancellationToken cancellationToken = default);

  // l. Tìm một bài viết theo mã số. 
  Task<Post> GetPostByIdAsync(int id, CancellationToken cancellationToken = default);

  // m. Thêm hay cập nhật một bài viết. 
  Task AddOrUpdatePostAsync(Post post, CancellationToken cancellationToken = default);

  // n. Chuyển đổi trạng thái Published của bài viết. 
  Task ChangePostStatusAsync(int id, CancellationToken cancellationToken = default);

  // o. Lấy ngẫu nhiên N bài viết. N là tham số đầu vào. 
  Task<IList<Post>> GetRandomPostAsync(int n, CancellationToken cancellationToken = default);

  // p.Tạo lớp PostQuery để lưu trữ các điều kiện tìm kiếm bài viết.Chẳng hạn:
  // mã tác giả, mã chuyên mục, tên ký hiệu chuyên mục, năm/tháng đăng bài,
  // từ khóa, … 
  // q. Tìm tất cả bài viết thỏa mãn điều kiện tìm kiếm được cho trong đối tượng
  // PostQuery(kết quả trả về kiểu IList<Post>).
  // r. Đếm số lượng bài viết thỏa mãn điều kiện tìm kiếm được cho trong đối
  // tượng PostQuery.
  // s. Tìm và phân trang các bài viết thỏa mãn điều kiện tìm kiếm được cho trong
  // đối tượng PostQuery(kết quả trả về kiểu IPagedList<Post>)
  Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

  // t.Tương tự câu trên nhưng yêu cầu trả về kiểu IPagedList<T>.Trong đó T
  // là kiểu dữ liệu của đối tượng mới được tạo từ đối tượng Post.Hàm này có
  // thêm một đầu vào là Func<IQueryable<Post>, IQueryable<T>> mapper
  // để ánh xạ các đối tượng Post thành các đối tượng T theo yêu cầu.
  Task<IPagedList<T>> GetPostByQueryAsync<T>(PostQuery query, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default);

  #endregion

  #region 2.C

  // b. Tìm một tác giả theo mã số. 
  Task<Author> GetAuthorByIdAsync(int id, CancellationToken cancellationToken);

  // c. Tìm một tác giả theo tên định danh (slug)
  Task<Author> GetAuthorBySlugAsync(string slug, CancellationToken cancellationToken);

  // d. Lấy và phân trang danh sách tác giả kèm theo số lượng bài viết của tác giả
  // đó.Kết quả trả về kiểu IPagedList<AuthorItem>
  Task<IPagedList<AuthorItem>> GetAuthorsAsync(IPagingParams pagingParams, CancellationToken cancellationToken = default);

  // e. Thêm hoặc cập nhật thông tin một tác giả.
  Task AddOrUpdateAuthorAsync(Author author, CancellationToken cancellationToken = default);

  // f. Tìm danh sách N tác giả có nhiều bài viết nhất. N là tham số đầu vào. 
  Task<IList<Author>> Find_N_MostPostByAuthorAsync(int n, CancellationToken cancellationToken = default);

  #endregion
}