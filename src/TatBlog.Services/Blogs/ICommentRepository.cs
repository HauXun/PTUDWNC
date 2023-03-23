using TatBlog.Core.Contracts;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;

namespace TatBlog.Services.Blogs;

public interface ICommentRepository
{
  Task<bool> AddCommentAsync(Comment comment, CancellationToken cancellationToken = default);

  Task ChangeCommentStatusAsync(int id, CancellationToken cancellationToken = default);

  Task<IPagedList<Comment>> GetCommentByPostIdAsync(int postId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

  Task<IPagedList<Comment>> GetCommentByQueryAsync(CommentQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

  Task<bool> DeleteCommentByIdAsync(int? id, CancellationToken cancellationToken = default);
}