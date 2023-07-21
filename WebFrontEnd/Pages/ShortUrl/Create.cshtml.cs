using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace WebFrontEnd.Pages.ShortUrl
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IOptions<AppSettings> appSettings;

        public CreateModel(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings)
        {
            this.httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
        }

        [BindProperty]
        public Models.ShortUrl ShortUrl { get; set; } = null!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (ShortUrl?.Path?.Length > 10)
            {
                TempData["ErrorMessage"] = "Path cannot be longer than 10 characters.";

                return Redirect("~/");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data.";

                return Redirect("~/");
            }
            
            var httpClient = httpClientFactory.CreateClient();            
            var response = await httpClient.PostAsync(appSettings.Value.ApiUrl, new StringContent(JsonConvert.SerializeObject(ShortUrl), Encoding.UTF8, MediaTypeNames.Application.Json));

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = (await response.Content.ReadFromJsonAsync<ErrorDetails>())?.Message ?? "An unexpected error has occurred.";

                return Redirect("~/");
            }

            TempData["InfoMessage"] = $"URL created successfully: {await response.Content.ReadAsStringAsync()}.";

            return Redirect("~/");
        }
    }
}
