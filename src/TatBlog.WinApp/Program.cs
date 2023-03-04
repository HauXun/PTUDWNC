using TatBlog.Core.Entities;
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
goto execute;
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

// var categories = await blogRepository.GetCategoriesAsync();

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

// WriteLine("{0, -5}{1, -50}{2, 10}", "ID", "Name", "Count");

// foreach (var item in categories)
// {
//   WriteLine("{0, -5}{1, -50}{2, 10}", item.Id, item.Name, item.PostCount);
// }


// a. Tìm một thẻ (Tag) theo tên định danh (slug) 
WriteLine("\nTìm một thẻ (Tag) theo tên định danh (slug)");
var tagBySlug = await blogRepository.GetTagBySlugAsync("google");
WriteLine("{0, -20}{1, -50}{2, 10}", "Name", "Description", "Slug");
WriteLine("{0, -20}{1, -50}{2, 10}", tagBySlug.Name, tagBySlug.Description, tagBySlug.UrlSlug);

// c. Lấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó. Kết
// quả trả về kiểu IList<TagItem>.
WriteLine("\nLấy danh sách tất cả các thẻ (Tag) kèm theo số bài viết chứa thẻ đó. Kết quả trả về kiểu IList<TagItem>.");
var tagListWithPostCount = await blogRepository.GetTagListWithPostCountAsync();
WriteLine("{0, -5}{1, -50}{2, 10}", "ID", "Name", "Count");
foreach (var tag in tagListWithPostCount)
{
  WriteLine("{0, -5}{1, -50}{2, 10}", tag.Id, tag.Name, tag.PostCount);
}

// d. Xóa một thẻ theo mã cho trước. 
// await blogRepository.DeleteTagByIdAsync(null);

// e. Tìm một chuyên mục (Category) theo tên định danh (slug)
WriteLine("\nTìm một chuyên mục (Category) theo tên định danh (slug)");
var categoryBySlug = await blogRepository.GetCategoryBySlugAsync("net-core");
WriteLine("{0, -20}{1, -50}{2, 10}", "Name", "Description", "Slug");
WriteLine("{0, -20}{1, -50}{2, 10}", categoryBySlug.Name, categoryBySlug.Description, categoryBySlug.UrlSlug);

// f. Tìm một chuyên mục theo mã số cho trước
WriteLine("\nTìm một chuyên mục theo mã số cho trước");
var categoryById = await blogRepository.GetCategoryByIdAsync(10);
WriteLine("{0, -20}{1, -50}{2, 10}", "Name", "Description", "Slug");
WriteLine("{0, -20}{1, -50}{2, 10}", categoryById.Name, categoryById.Description, categoryById.UrlSlug);

// g. Thêm hoặc cập nhật một chuyên mục/chủ đề
#region Add/Edit không test

// WriteLine("\nThêm một chuyên mục/chủ đề");
// Category category = new Category
// {
//   Name = "ABC",
//   Description = "Des for ABC",
//   UrlSlug = "abc",
//   ShowOnMenu = true
// };
// await blogRepository.AddCategoryAsync(category);
// WriteLine("\nChuyên mục/chủ đề sau khi thêm");
// WriteLine("{0, -20}{1, -50}{2, 10}", "Name", "Description", "Slug");
// WriteLine("{0, -20}{1, -50}{2, 10}", category.Name, category.Description, category.UrlSlug);

// WriteLine("\nSửa một chuyên mục/chủ đề");
// category = new Category
// {
//   Name = "ABC edit",
//   Description = "Des for ABC edit",
//   UrlSlug = "abc edit",
//   ShowOnMenu = false
// };
// await blogRepository.EditCategoryAsync(category);
// WriteLine("\nChuyên mục/chủ đề sau khi thêm");
// WriteLine("{0, -20}{1, -50}{2, 10}", "Name", "Description", "Slug");
// WriteLine("{0, -20}{1, -50}{2, 10}", category.Name, category.Description, category.UrlSlug);

#endregion

// h. Xóa một chuyên mục theo mã số cho trước. 
// await blogRepository.DeleteCategoryByIdAsync(null);

// i. Kiểm tra tên định danh (slug) của một chuyên mục đã tồn tại hay chưa. 
WriteLine("\nKiểm tra tên định danh (slug) của một chuyên mục đã tồn tại hay chưa");
var existedSlug = await blogRepository.CheckCategorySlugExisted("net-core");
WriteLine("\nSlug net-core is " + (existedSlug ? "exists" : "does not exist"));

// j. Lấy và phân trang danh sách chuyên mục, kết quả trả về kiểu
// IPagedList<CategoryItem>.

WriteLine("\nLấy và phân trang danh sách chuyên mục, kết quả trả về kiểu IPagedList<CategoryItem>.");
// Tạo đối tượng chứa tham số phân trang
var pagingParams = new PagingParams
{
  PageNumber = 1,         // Lây kết quả ở trang số 1
  PageSize = 5,           // Lấy 5 mẫu tin
  SortColumn = "Name",    // Sắp xếp theo tên
  SortOrder = "DESC"      // Theo chiều giảm dần
};

// Lấy danh sách từ khóa
var tagList = await blogRepository.GetPagedCategoriesAsync(pagingParams);

WriteLine("{0, -5}{1, -50}{2, 10}", "ID", "Name", "Post Count");

foreach (var item in tagList)
{
  WriteLine("{0, -5}{1, -50}{2, 10}", item.Id, item.Name, item.PostCount);
}

// k. Đếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào. Kết
// quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số
// bài viết.

WriteLine("\nĐếm số lượng bài viết trong N tháng gần nhất. N là tham số đầu vào. Kết quả là một danh sách các đối tượng chứa các thông tin sau: Năm, Tháng, Số bài viết.");
var postCount = await blogRepository.CountPostInMonthAsync(2);

WriteLine("{0, -30}{1, -20}{2, 20}", "Post Count", "Year", "Month");

foreach (var item in postCount)
{
  WriteLine("{0, -30}{1, -20}{2, 20}", item.Count, item.Year, item.Month);
}

// l. Tìm một bài viết theo mã số. 

WriteLine("\nTìm một bài viết theo mã số. ");
var postById = await blogRepository.GetPostByIdAsync(10);
WriteLine("{0, -20}{1, -50}{2, 10}", "Name", "Description", "Slug");
WriteLine("{0, -20}{1, -50}{2, 10}", postById.Title, postById.Description, postById.UrlSlug);

// m. Thêm hay cập nhật một bài viết. 

// ---------------

execute:
WriteLine("");