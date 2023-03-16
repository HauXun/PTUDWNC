using Microsoft.EntityFrameworkCore;
using TatBlog.Core.Constants;
using TatBlog.Core.DTO;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;
using TatBlog.Services.Blogs;
using TatBlog.Services.Extensions;
using TatBlog.Services.Media;

public class SubscriberRepository : ISubscriberRepository
{
  private readonly BlogDbContext _blogContext;
  private readonly SendMailService _sendMailService;

  public SubscriberRepository(BlogDbContext dbContext, SendMailService sendMailService)
  {
    _blogContext = dbContext;
    _sendMailService = sendMailService;
  }

  public async Task<bool> BlockSubscriberAsync(int id, string reason, string notes, CancellationToken cancellationToken = default)
  {
    var subscriber = await GetSubscriberByIdAsync(id);
    if (subscriber == null)
    {
      Console.WriteLine("Không có người đăng ký nào để chặn");
      return await Task.FromResult(false);
    }

    subscriber.CancelReason = reason;
    subscriber.AdminNotes = notes;
    subscriber.ForceLock = true;

    _blogContext.Attach(subscriber).State = EntityState.Modified;
    var affects = await _blogContext.SaveChangesAsync(cancellationToken);

    return affects > 0;
  }

  public async Task<bool> DeleteSubscriberAsync(int id, CancellationToken cancellationToken = default)
  {
    var subscriber = await _blogContext.Set<Subscriber>().FindAsync(id);

    if (subscriber is null) 
      return await Task.FromResult(false);

    _blogContext.Set<Subscriber>().Remove(subscriber);
    var affects = await _blogContext.SaveChangesAsync(cancellationToken);

    return affects > 0;
  }

  public async Task<Subscriber> GetSubscriberByEmailAsync(string email, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Subscriber>()
                             .Where(s => s.SubscribeEmail.Equals(email))
                             .FirstOrDefaultAsync(cancellationToken);
  }

  public async Task<Subscriber> GetSubscriberByIdAsync(int id, CancellationToken cancellationToken = default)
  {
    return await _blogContext.Set<Subscriber>().FindAsync(id, cancellationToken);
  }

  public async Task<IPagedList<Subscriber>> SearchSubscribersAsync(IPagingParams pagingParams, string keyword, bool unsubscribed, bool involuntary, CancellationToken cancellationToken = default)
  {
    var subscriberQuery = _blogContext.Set<Subscriber>();

    return await subscriberQuery.ToPagedListAsync(pagingParams, cancellationToken);
  }

  public async Task<bool> SubscribeAsync(string email, CancellationToken cancellationToken = default)
  {
    var subscriberExisted = await GetSubscriberByEmailAsync(email);
    
    if (subscriberExisted != null)
    {
      if (subscriberExisted.UnSubDated == null)
        return await Task.FromResult(false);

      subscriberExisted.UnSubDated = null;
      _blogContext.Attach(subscriberExisted).State = EntityState.Modified;
      await _blogContext.SaveChangesAsync(cancellationToken);
    }

    MailContent mailContent = new MailContent
    {
      To = email,
      Subject = "Đăng ký theo dõi blog",
      Body = "<h1>Đăng ký thành công</h1><i>Cảm ơn bạn đã đăng ký theo dõi blog</i>"
    };

    Subscriber subscriber = new Subscriber
    {
      SubscribeEmail = email,
      SubDated = DateTime.Now
    };

    _blogContext.Add(subscriber);

    await _sendMailService.SendEmailAsync(mailContent);
    var affects = await _blogContext.SaveChangesAsync(cancellationToken);

    return affects > 0;
  }

  public async Task UnsubscribeAsync(string email, string reason, bool voluntary, CancellationToken cancellationToken = default)
  {
    var subscriber = await GetSubscriberByEmailAsync(email);
    if (subscriber == null)
    {
      Console.WriteLine("Không có người đăng ký này để hủy đăng ký");
      return;
    }

    subscriber.CancelReason = reason;
    subscriber.UnsubscribeVoluntary = true;
    subscriber.UnSubDated = DateTime.Now;

    _blogContext.Attach(subscriber).State = EntityState.Modified;
    await _blogContext.SaveChangesAsync(cancellationToken);
  }

  public async Task<IPagedList<Subscriber>> GetSubscriberByQueryAsync(SubscriberQuery query, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
  {
    return await FilterSubscribers(query).ToPagedListAsync(
                                          pageNumber,
                                          pageSize,
                                          nameof(Subscriber.SubDated),
                                          "DESC",
                                          cancellationToken);
  }

  private IQueryable<Subscriber> FilterSubscribers(SubscriberQuery query)
  {
    IQueryable<Subscriber> categoryQuery = _blogContext.Set<Subscriber>();

    if (!string.IsNullOrWhiteSpace(query.Email))
    {
      categoryQuery = categoryQuery.Where(x => x.SubscribeEmail.Equals(query.Email));
    }

    if (query.ForceLock)
    {
      categoryQuery = categoryQuery.Where(x => x.ForceLock);
    }

    if (query.UnsubscribeVoluntary)
    {
      categoryQuery = categoryQuery.Where(x => x.UnsubscribeVoluntary);
    }

    if (!string.IsNullOrWhiteSpace(query.Keyword))
    {
      categoryQuery = categoryQuery.Where(x => x.CancelReason.Contains(query.Keyword) ||
                   x.AdminNotes.Contains(query.Keyword));
    }

    return categoryQuery;
  }
}