namespace TatBlog.WebApi.Models;

public class TagEditModel
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string UrlSlug { get; set; }
  public string Description { get; set; }

    public static async ValueTask<TagEditModel> BindAsync(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();
        return new TagEditModel()
        {
            Id = int.Parse(form["Id"]),
            Name = form["Name"],
            Description = form["Description"],
        };
    }
}
