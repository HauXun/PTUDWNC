namespace TatBlog.WebApp.Middlewares;

public class UserActivityMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<UserActivityMiddleware> _logger;

  public UserActivityMiddleware(RequestDelegate next, ILogger<UserActivityMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task Invoke(HttpContext context)
  {
    _logger.LogInformation(
      $"{DateTime.Now} - IP: {context.Connection.RemoteIpAddress} - Path: {context.Request.Path}"
    );

    await _next(context);
  }
}