namespace TatBlog.WebApi.Models;

public class CategoryEditModel
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string UrlSlug { get; set; }
  public string Description { get; set; }
  public bool ShowOnMenu { get; set; }

    public static async ValueTask<CategoryEditModel> BindAsync(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();
        return new CategoryEditModel()
        {
            Id = int.Parse(form["Id"]),
            Name = form["Name"],
            Description = form["Description"],
            ShowOnMenu = form["ShowOnMenu"] != "false",
        };
    }
}
