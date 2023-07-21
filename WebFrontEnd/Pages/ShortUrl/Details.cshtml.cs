using Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace WebFrontEnd.Pages.ShortUrl
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<AppSettings> appSettings;

        public DetailsModel(IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings)
        {
            _httpClientFactory = httpClientFactory;
            this.appSettings = appSettings;
        }

        [BindProperty]
        public Models.ShortUrl ShortUrl { get; set; } = null!;

        public async Task<IActionResult> OnPost()
        {
            if (ShortUrl.Path == null)
            {
                TempData["ErrorMessage"] = "You must complete the path.";

                return Redirect("~/");
            }

            var httpClient = _httpClientFactory.CreateClient();            
            var response = await httpClient.PostAsync(new Uri($"{appSettings.Value.ApiUrl}/get-path"), new StringContent(JsonConvert.SerializeObject(ShortUrl), Encoding.UTF8, MediaTypeNames.Application.Json));

            if (!response.IsSuccessStatusCode)
            {
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        TempData["ErrorMessage"] = "URL not found.";

                        return Redirect("~/");
                    default:
                        TempData["ErrorMessage"] = (await response.Content.ReadFromJsonAsync<ErrorDetails>())?.Message ?? "An unexpected error has occurred.";

                        return Redirect("~/");
                }
            }

            var shorturl = await response.Content.ReadFromJsonAsync<Models.ShortUrl>();

            if (shorturl == null)
            {
                TempData["ErrorMessage"] = "Unknown URL.";

                return Redirect("~/");
            }
            else
            {
                ShortUrl = shorturl;
            }

            TempData["InfoMessage"] = $"URL found: {JsonConvert.SerializeObject(ShortUrl)}";

            return Redirect("~/");
        }
    }
}
