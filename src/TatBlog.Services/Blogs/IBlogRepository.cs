using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface IBlogRepository
{
	// Tìm bài viết có tên định danh là 'slug
	// và được đăng vào tháng 'month' năm 'year'
	Task<Post> GetPostAsync(int year, int month, int day, string slug, CancellationToken cancellationToken = default);

	Task<Post> GetCachedPostAsync(int year, int month, int day, string slug, CancellationToken cancellationToken = default);

	// l. Tìm một bài viết theo mã số. 
	Task<Post> GetPostByIdAsync(int id, bool published = false, CancellationToken cancellationToken = default);

	Task<Post> GetCachedPostByIdAsync(int id, bool published = false, CancellationToken cancellationToken = default);

	// Tìm top (n) bài viết phổ biến được nhiều người xem nhất
	Task<IList<Post>> GetPopularArticlesAsync(int limit, CancellationToken cancellationToken = default);

	// o. Lấy ngẫu nhiên N bài viết. N là tham số đầu vào. 
	Task<IList<Post>> GetRandomPostAsync(int limit, CancellationToken cancellationToken = default);

	Task<IList<DateItem>> GetArchivesPostAsync(int limit, CancellationToken cancellationToken = default);

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

	Task<IPagedList<Post>> GetPostByQueryAsync(PostQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);

	// t.Tương tự câu trên nhưng yêu cầu trả về kiểu IPagedList<T>.Trong đó T
	// là kiểu dữ liệu của đối tượng mới được tạo từ đối tượng Post.Hàm này có
	// thêm một đầu vào là Func<IQueryable<Post>, IQueryable<T>> mapper
	// để ánh xạ các đối tượng Post thành các đối tượng T theo yêu cầu.
	Task<IPagedList<T>> GetPostByQueryAsync<T>(PostQuery query, IPagingParams pagingParams, Func<IQueryable<Post>, IQueryable<T>> mapper, CancellationToken cancellationToken = default);

	// m. Thêm hay cập nhật một bài viết. 
	Task<bool> AddOrUpdatePostAsync(Post post, IEnumerable<string> tags, CancellationToken cancellationToken = default);

	Task<bool> DeletePostByIdAsync(int? id, CancellationToken cancellationToken = default);

	// Kiểm tra xem tên định danh của bài viết đã có hay chưa
	Task<bool> IsPostSlugExistedAsync(int postId, string slug, CancellationToken cancellationToken = default);

	// Tăng số lượt xem của một bài viết
	Task IncreaseViewCountAsync(int postId, CancellationToken cancellationToken = default);

	// n. Chuyển đổi trạng thái Published của bài viết. 
	Task ChangePostStatusAsync(int id, CancellationToken cancellationToken = default);

	// k. Đếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào. Kết
	// quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số
	// bài viết.
	Task<IList<PostInMonthItem>> CountPostInMonthAsync(int monthCount, CancellationToken cancellationToken = default);
}