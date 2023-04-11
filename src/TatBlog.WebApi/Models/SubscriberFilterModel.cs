namespace TatBlog.WebApi.Models;

public class SubscriberFilterModel : PagingModel
{
  public string Keyword { get; set; }
  public string Email { get; set; }
  public bool ForceLock { get; set; }
  public bool UnsubscribeVoluntary { get; set; }
}
