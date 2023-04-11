using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace TatBlog.WebApi.Models;

public class CommentFilterModel : PagingModel
{
  public string Keyword { get; set; }
  public string UserName { get; set; }
  public int? Year { get; set; }
  public int? Month { get; set; }
  public int? Day { get; set; }
  public string PostTitle { get; set; }
  public bool Censored { get; set; }

  public IEnumerable<SelectListItem> MonthList { get; set; }

  public CommentFilterModel()
  {
    MonthList = Enumerable.Range(1, 12).Select(m => new SelectListItem() { Value = m.ToString(), Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m) }).ToList();
  }
}
