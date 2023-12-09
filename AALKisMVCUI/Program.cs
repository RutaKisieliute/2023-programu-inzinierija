using AALKisMVCUI.Utility;

namespace AALKisMVCUI;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public class Program
{
    public static readonly string LogFileName = "./AALKisMVCUI.log";
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient<APIClient>();
        builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddFile(LogFileName, append: false));


        var app = builder.Build();

        // Configure the HTTP request pipeline.
#if !DEBUG
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
#endif

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.UseFallbackRedirection(target: "/");

        app.Run();
    }

}
