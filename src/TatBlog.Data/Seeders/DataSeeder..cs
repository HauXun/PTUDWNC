using TatBlog.Core.Entities;
using TatBlog.Data.Contexts;

namespace TatBlog.Data.Seeders;

public class DataSeeder : IDataSeeder
{
  private readonly BlogDbContext _dbContext;

  public DataSeeder(BlogDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public void Initialize()
  {
    _dbContext.Database.EnsureCreated();

    if (_dbContext.Posts.Any()) return;

    var authors = AddAuthors();
    var categories = AddCategories();
    var tags = AddTags();
    var posts = AddPosts(authors, categories, tags);
  }

  private IList<Author> AddAuthors()
  {
    var authors = new List<Author>()
    {
      new()
      {
        FullName = "Jason Mouth",
        UrlSlug = "jason-mouth",
        Email = "jason@gmail.com",
        JoinedDate = new DateTime(2022, 10, 21)
      },
      new()
      {
        FullName = "Jessica Wonder",
        UrlSlug = "jessica-wonder",
        Email = "jessica@gmail.com",
        JoinedDate = new DateTime(2022, 10, 21)
      },
    };

    _dbContext.Authors.AddRange(authors);
    _dbContext.SaveChanges();

    return authors;
  }

  private IList<Category> AddCategories()
  {
    var categories = new List<Category>()
    {
      new()
      {
        Name = ".NET Core",
        UrlSlug = "net-core",
        Description = "MS Net Core",
        ShowOnMenu = true,
      },
      new()
      {
        Name = "Architecture",
        UrlSlug = "architecture",
        Description = "Architecture design",
        ShowOnMenu = true,
      },
      new()
      {
        Name = "Messaging",
        UrlSlug = "messaging",
        Description = "Messaging design on chat",
        ShowOnMenu = true,
      },
      new()
      {
        Name = "OOP",
        UrlSlug = "oop",
        Description = "Object-Oriented-Programming",
        ShowOnMenu = true,
      },
      new()
      {
        Name = "Design Patterns",
        UrlSlug = "design-patterns",
        Description = "Design patterns",
        ShowOnMenu = true,
      },
    };

    _dbContext.Categories.AddRange(categories);
    _dbContext.SaveChanges();

    return categories;
  }

  private IList<Tag> AddTags()
  {
    var tags = new List<Tag>()
    {
      new()
      {
        Name = "Google",
        UrlSlug = "google-application",
        Description = "Google applications architecture design",
      },
      new()
      {
        Name = "ASP .NET MVC",
        UrlSlug = "asp-mvc-application",
        Description = "ASP .NET MVC application architecture design",
      },
      new()
      {
        Name = "Razor Page",
        UrlSlug = "razor-page",
        Description = "Razor Page application architecture design",
      },
      new()
      {
        Name = "Blazor",
        UrlSlug = "blazor-application",
        Description = "Blazor application architecture design",
      },
      new()
      {
        Name = "Deep Learning",
        UrlSlug = "deep-learning",
        Description = "Deeplearning application architecture design",
      },
      new()
      {
        Name = "Neural Network",
        UrlSlug = "neural-networking",
        Description = "Neural Network architecture design",
      },
    };

    _dbContext.Tags.AddRange(tags);
    _dbContext.SaveChanges();

    return tags;
  }

  private IList<Post> AddPosts(IList<Author> authors, IList<Category> categories, IList<Tag> tags)
  {
    var posts = new List<Post>()
    {
      new()
      {
        Title = "ASP .NET Core Diagnostic Scenarios",
        ShortDescription = "David and friends has a great repository filled.",
        Description = "Here's a few great DON'T and DO example posts.",
        Meta = "David and friends has a great repository filled.",
        UrlSlug = "aspnet-core-diagnostic-scenarios",
        Published = true,
        PostedDate = new DateTime(2021, 9, 30, 10, 20, 0),
        ModifiedDate = null,
        ViewCount = 10,
        Author = authors[0],
        Category = categories[0],
        Tags = new List<Tag>()
        {
          tags[0],
        }
      }
    };

    _dbContext.AddRange(posts);
    _dbContext.SaveChanges();

    return posts;
  }
}