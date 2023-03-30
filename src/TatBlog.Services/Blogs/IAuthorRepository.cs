using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface IAuthorRepository
{
	// d. Lấy và phân trang danh sách tác giả kèm theo số lượng bài viết của tác giả
	// đó.Kết quả trả về kiểu IPagedList<AuthorItem>
	Task<IList<AuthorItem>> GetAuthorsAsync(CancellationToken cancellationToken = default);
	// b. Tìm một tác giả theo mã số. 
	Task<Author> GetAuthorByIdAsync(int id, CancellationToken cancellationToken = default);

	Task<Author> GetCachedAuthorByIdAsync(int authorId);

	// c. Tìm một tác giả theo tên định danh (slug)
	Task<Author> GetAuthorBySlugAsync(string slug, CancellationToken cancellationToken = default);

	Task<Author> GetCachedAuthorBySlugAsync(
		string slug, CancellationToken cancellationToken = default);

	Task<IPagedList<Author>> GetAuthorByQueryAsync(AuthorQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

	Task<IPagedList<Author>> GetAuthorByQueryAsync(AuthorQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);
	
	Task<IPagedList<T>> GetAuthorByQueryAsync<T>(AuthorQuery query, IPagingParams pagingParams, Func<IQueryable<Author>, IQueryable<T>> mapper, CancellationToken cancellationToken = default);

	// e. Thêm hoặc cập nhật thông tin một tác giả.
	Task<bool> AddOrUpdateAuthorAsync(Author author, CancellationToken cancellationToken = default);

	Task<bool> DeleteAuthorByIdAsync(int? id, CancellationToken cancellationToken = default);

	// f. Tìm danh sách N tác giả có nhiều bài viết nhất. N là tham số đầu vào. 
	Task<IList<Author>> Find_N_MostPostByAuthorAsync(int n, CancellationToken cancellationToken = default);

	Task<bool> CheckAuthorSlugExisted(int id, string slug, CancellationToken cancellationToken = default);

	Task<bool> SetImageUrlAsync(
		int authorId, string imageUrl,
		CancellationToken cancellationToken = default);
}