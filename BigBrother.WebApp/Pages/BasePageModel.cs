using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BigBrother.WebApp.Pages;

public class BasePageModel: PageModel
{
    public string? Error { get; protected set; }

    protected virtual Task SafeInvokeAsync(Func<Task> func)
    {
        try
        {
            return func();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
            
            return Task.CompletedTask;
        }
    }
}