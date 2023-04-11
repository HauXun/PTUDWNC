namespace TatBlog.WebApi.Models;

public class AuthorEditModel
{
    public int Id { get; set; }
    public string FullName { get; set; }
	public string UrlSlug { get; set; }
    public IFormFile ImageFile { get; set; }
    public string ImageUrl { get; set; }
    public DateTime JoinedDate { get; set; }
	public string Email { get; set; }
	public string Notes { get; set; }

    public static async ValueTask<AuthorEditModel> BindAsync(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();
        return new AuthorEditModel()
        {
            ImageFile = form.Files["ImageFile"],
            Id = int.Parse(form["Id"]),
            FullName = form["FullName"],
            Email = form["Email"],
            Notes = form["Notes"],
        };
    }
}
