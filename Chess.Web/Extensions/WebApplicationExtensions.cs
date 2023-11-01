namespace Chess.Web.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureMiddleware(this WebApplication webapp)
    {
        if (!webapp.Environment.IsDevelopment())
        {
            //Enable compression
            webapp.UseResponseCompression();
            webapp.UseExceptionHandler("/Error");
            webapp.UseHsts();
        }

        webapp.UseHttpsRedirection();
        webapp.UseStaticFiles();
        webapp.UseRouting();

        return webapp;
    }
}