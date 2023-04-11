namespace TatBlog.WebApi.Models;

public class CategoryFilterModel : PagingModel
{
    public string Keyword { get; set; }
    public string UrlSlug { get; set; }
    public bool ShowOnMenu { get; set; }
}
