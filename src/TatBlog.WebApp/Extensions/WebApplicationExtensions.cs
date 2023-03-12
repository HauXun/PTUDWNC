using Microsoft.EntityFrameworkCore;
using NLog.Web;
using TatBlog.Data.Contexts;
using TatBlog.Data.Seeders;
using TatBlog.Services.Blogs;
using TatBlog.Services.Media;
using TatBlog.WebApp.Middlewares;

namespace TatBlog.WebApp.Extensions;

public static class WebApplicationExtension 
{
  public static WebApplicationBuilder ConfigureMvc(this WebApplicationBuilder builder)
  {
    // Add services to the container.
    // Thêm các dịch vụ được yêu cầu bởi MVC Framwork
    builder.Services.AddControllersWithViews();
    builder.Services.AddResponseCompression();

    return builder;
  }

  // Cấu hình việc sử dụng NLog
  public static WebApplicationBuilder ConfigureNLog(this WebApplicationBuilder builder)
  {
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    return builder;
  }

  // Đăng ký các dịch vụ với DI Container
  public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
  {
    builder.Services.Configure<RouteOptions>(routeOptions =>
    {
      routeOptions.LowercaseUrls = true;
    });


    // Đăng ký các dịch vụ với DI Container
    builder.Services.AddDbContext<BlogDbContext>(options =>
    {
      options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddScoped<IBlogRepository, BlogRepository>();
    builder.Services.AddScoped<IDataSeeder, DataSeeder>();
    builder.Services.AddScoped<IMediaManager, LocalFileSystemMediaManager>();

    return builder;
  }

  // Cấu hình HTTP Request pipeline
  public static WebApplication UseRequestPipeline(this WebApplication app)
  {
    // Configure the HTTP request pipeline.

    // Thêm middware để hiển thị thông báo lỗi
    if (!app.Environment.IsDevelopment())
    {
      app.UseExceptionHandler("/Blog/Error");

      // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
      // Thêm middware cho việc áp dụng HSTS (thêm header
      // Strict-Transport-Security vào HTTP response)
      app.UseHsts();
    }

    // Thêm middware để chuyển hướng HTTP sang HTTPS
    app.UseHttpsRedirection();

    // Thêm middware phục vụ các yêu cầu liên quan
    // tới các tập tin nội dung tĩnh như hình ảnh, css, ...
    app.UseStaticFiles();

    // app.UseMiddleware<UserActivityMiddleware>();

    // Thêm middware lựa chọn endpoint phù hợp nhất
    // để xử lý một HTTP request.
    app.UseRouting();

    app.UseAuthorization();

    return app;
  }

  // Thêm dữ liệu mẫu vào CSDL
  public static IApplicationBuilder UseDataSeeder(this IApplicationBuilder app)
  {
    using var scope = app.ApplicationServices.CreateScope();

    try
    {
      scope.ServiceProvider.GetRequiredService<IDataSeeder>().Initialize();
    }
    catch (Exception ex)
    {
      scope.ServiceProvider.GetRequiredService<ILogger<Program>>().LogError(ex, "Could not insert data into database");
    }

    return app;
  }
}