namespace TatBlog.WebApi.Models;

public class TagFilterModel : PagingModel
{
    public string Keyword { get; set; }
    public string Name { get; set; }
}
