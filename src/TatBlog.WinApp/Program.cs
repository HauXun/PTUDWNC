using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.WinApp;
using static System.Console;

// Tạo đối tượng DbContext để quản lý phiên làm việc
// với CSDL và trạng thái của các đối tượng
var context = new BlogDbContext();

// Tạo đối tượng khởi tạo dữ liệu mẫu
// var seeder = new DataSeeder(context);

// Gọi hàm Initialize để nhập dữ liệu mẫu
// seeder.Initialize();

// Đọc danh sách tác giả từ cơ sở dữ liệu
// var authors = context.Authors.ToList();

// Xuất danh sách tác giả ra màn hình
// WriteLine("{0, -4}{1, -30}{2, -30}{3, 12}", "ID", "ID", "Full Name", " Email", "Joined Date");

// foreach (var author in authors)
// {
//   WriteLine("{0, -4}{1, -30}{2, -30}{3, 12:MM/dd/yyyy}", author.Id, author.FullName, author.Email, author.JoinedDate);
// }

// Đọc danh sách bài viết từ csdl
// Lấy kèm tên tác giả và chuyên mục
// var posts = context.Posts
//                    .Where(p => p.Published)
//                    .OrderBy(p => p.Title)
//                    .Select(p => new
//                    {
//                      Id = p.Id,
//                      Title = p.Title,
//                      ViewCount = p.ViewCount,
//                      PostedDate = p.PostedDate,
//                      Author = p.Author.FullName,
//                      Category = p.Category.Name
//                    })
//                    .ToList();

// Tạo đối tượng BlogRepository
IBlogRepository blogRepository = new BlogRepository(context);

// Tìm 3 bài viết được xem/ đọc nhiều nhất
// var posts = await blogRepository.GetPopularArticlesAsync(3);

// Xuất danh sách bài viết ra màn hình
// foreach (var post in posts)
// {
//   WriteLine($"ID        : {post.Id}");
//   WriteLine($"Title     : {post.Title}");
//   WriteLine($"View      : {post.ViewCount}");
//   WriteLine("Date       : {0:MM/dd/yyyy}", post.PostedDate);
//   WriteLine($"Author    : {post.Author.FullName}");
//   WriteLine($"Category  : {post.Category.Name}");
//   WriteLine("".PadRight(80, '-'));
// }

var categories = await blogRepository.GetCategoriesAsync();

// Tạo đối tượng chứa tham số phân trang
// var pagingParams = new PagingParams
// {
//   PageNumber = 1,         // Lây kết quả ở trang số 1
//   PageSize = 5,           // Lấy 5 mẫu tin
//   SortColumn = "Name",    // Sắp xếp theo tên
//   SortOrder = "DESC"      // Theo chiều giảm dần
// };

// Lấy danh sách từ khóa
// var tagList = await blogRepository.GetPagedTagsAsync(pagingParams);

WriteLine("{0, -5}{1, -50}{2, 10}", "ID", "Name", "Count");

foreach (var item in categories)
{
  WriteLine("{0, -5}{1, -50}{2, 10}", item.Id, item.Name, item.PostCount);
}