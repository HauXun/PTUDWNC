using System.Text.RegularExpressions;
using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Services.Extensions;

public static class BlogExtensions
{
  public static IEnumerable<string> SplitCamelCase(this string input)
  {
      return Regex.Split(input, @"([A-Z]?[a-z]+)").Where(str => !string.IsNullOrEmpty(str));
  }

  public static string FirstCharUppercase(this string input)
  {
      return $"{char.ToUpper(input[0])}{input.Substring(1)}";
  }

  public static string GenerateSlug(this string slug)
  {
    var splitToValidFormat = slug.Split(new[] { " ", ",", ";", ".", "-" ,"\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    var refixAlphabet = splitToValidFormat.ToList().Select(s => $"{s.FirstCharUppercase}");
    var slugFormat = string.Join("", refixAlphabet);
    var reflectionSlug = String.Join("-", slugFormat.SplitCamelCase());

    return reflectionSlug;
  }
}
