using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebStatus.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet() => Redirect("/hc-ui");
}