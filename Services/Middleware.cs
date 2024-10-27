namespace Services;

public class Middleware
{
    private readonly RequestDelegate _next;

    public Middleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.HasValue && context.Request.Path.Value == "api/User/register" || context.Request.Path.Value == "api/User/login" || context.Request.Path.Value == "api/MapSpot/getSpots")
        {
            await _next(context);
        }
        else
            {
                if (!context.Request.Headers.ContainsKey("Authorization"))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var token = context.Request.Headers["Authorization"].ToString().Split(" ")[1];

                var applicationContext = context.RequestServices.GetService<ApplicationContext>();
                if(applicationContext != null)
                {
                    if (!AuthService.ValidateToken(token, applicationContext))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized token");
                        return;
                    }
                }
                else
                {
                    throw new Exception("Application context not found");
                }

                await _next(context);
            }
    }
}