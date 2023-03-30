using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface ISubscriberRepository
{
	// Tìm người theo dõi bằng ID: GetSubscriberByIdAsync(id)
	Task<Subscriber> GetSubscriberByIdAsync(int id, CancellationToken cancellationToken = default);

	Task<Subscriber> GetCachedSubscriberByIdAsync(int id, CancellationToken cancellationToken = default);

	// Tìm người theo dõi bằng email: GetSubscriberByEmailAsync(email)
	Task<Subscriber> GetSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default);

	Task<Subscriber> GetCachedSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default);

	// Tìm danh sách người theo dõi theo nhiều tiêu chí khác nhau, kết quả
	// được phân trang: Task<IPagedList<Subscriber>>
	// SearchSubscribersAsync(pagingParams, keyword, unsubscribed,
	// involuntary)
	Task<IPagedList<Subscriber>> GetSubscriberByQueryAsync(SubscriberQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

	Task<IPagedList<Subscriber>> GetSubscriberByQueryAsync(SubscriberQuery query, IPagingParams pagingParams, CancellationToken cancellationToken = default);

	Task<IPagedList<T>> GetSubscriberByQueryAsync<T>(SubscriberQuery query, IPagingParams pagingParams, Func<IQueryable<Subscriber>, IQueryable<T>> mapper, CancellationToken cancellationToken = default);

	// Đăng ký theo dõi: SubscribeAsync(email)
	Task<bool> SubscribeAsync(string email, CancellationToken cancellationToken = default);

	// Xóa một người theo dõi: DeleteSubscriberAsync(id)
	Task<bool> DeleteSubscriberAsync(int id, CancellationToken cancellationToken = default);

	// Chặn một người theo dõi: BlockSubscriberAsync(id, reason, notes)
	Task<bool> BlockSubscriberAsync(int id, string reason, string notes, CancellationToken cancellationToken = default);

	// Hủy đăng ký: UnsubscribeAsync(email, reason, voluntary)
	Task<bool> UnsubscribeAsync(string email, string reason, bool voluntary, CancellationToken cancellationToken = default);
}