namespace AALKisMVCUI.Utility;

public static class RedirectionOn404Middleware
{
    public static WebApplication UseFallbackRedirection(this WebApplication app, string target)
    {
        app.Use(async (context, next) => {
            if(context.GetEndpoint() == null)
            {
                context.Response.Redirect(target);
                await context.Response.StartAsync();
            }
            else
            {
                await next(context);
            }
        });

        return app;
    }
}
