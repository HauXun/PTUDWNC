using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface ITagRepository
{
	Task<IList<Tag>> GetTagListAsync(CancellationToken cancellationToken = default);

	// a. Tìm một thẻ (Tag) theo tên định danh (slug) 
	Task<Tag> GetTagBySlugAsync(string slug, CancellationToken cancellationToken = default);

	Task<Tag> GetCachedTagBySlugAsync(string slug, CancellationToken cancellationToken = default);

	Task<Tag> GetTagByIdAsync(int id, CancellationToken cancellationToken = default);

	Task<Tag> GetCachedTagByIdAsync(int tagId, CancellationToken cancellationToken = default);

	// Lấy danh sách từ khóa/ thẻ và phân trang theo
	// các tham số pagingParams
	Task<IPagedList<Tag>> GetTagByQueryAsync(TagQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

	Task<IPagedList<Tag>> GetTagByQueryAsync(TagQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);

	Task<IPagedList<T>> GetTagByQueryAsync<T>(TagQuery query, IPagingParams pagingParams, Func<IQueryable<Tag>, IQueryable<T>> mapper, CancellationToken cancellationToken = default);

	// c. Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó. Kết
	// quả trả về kiểu IList<TagItem>.
	Task<IList<TagItem>> GetTagListWithPostCountAsync(CancellationToken cancellationToken = default);

	Task<bool> AddOrUpdateTagAsync(Tag tag, CancellationToken cancellationToken = default);

	// d. Xóa một thẻ theo mã cho trước. 
	Task<bool> DeleteTagByIdAsync(int? id, CancellationToken cancellationToken = default);

	Task<bool> CheckTagSlugExisted(int id, string slug, CancellationToken cancellationToken = default);
}