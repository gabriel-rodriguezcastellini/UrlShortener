using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebFrontEnd.Pages
{
    public class IndexModel : PageModel
    {        
        public Models.ShortUrl ShortUrl { get; set; } = null!;

        public Task OnGet() => Task.CompletedTask;
    }
}