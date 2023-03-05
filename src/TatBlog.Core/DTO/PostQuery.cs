public class PostQuery
{
  public string Title { get; set; }
  public bool? PublishedOnly { get; set; }
  public string Year { get; set; }
  public string Month { get; set; }
  public string Day { get; set; }
  public string AuthorId { get; set; }
  public string CategoryId { get; set; }
  public string Tags { get; set; }
  public string PostSlug { get; set; }
  public string AuthorSlug { get; set; }
  public string CategorySlug { get; set; }
  public string TagSlug { get; set; }
  public IList<string> SelectedTag { get; set; }
  public IEnumerable<string> SelectedAuthor { get; set; }
  public IEnumerable<string> SelectedCategory { get; set; }

  public void GetTagListAsync()
  {
    SelectedTag = (Tags ?? "").Split(new[] { ",", ";", ".", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
  }
}