using Data;
using Microsoft.AspNetCore.Mvc;
using WebAPI.ViewModels.ShortUrlController.Create.Input;
using WebAPI.ViewModels.ShortUrlController.Create.Output;
using WebAPI.ViewModels.ShortUrlController.Get.Input;
using WebAPI.ViewModels.ShortUrlController.Get.Output;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebAPI.Controllers;

[Route("[controller]")]
[ApiController]
[Produces("application/json")]
[ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
public class ShortUrlController : ControllerBase
{
    private readonly ShortUrlRepository ShortUrlRepository;
    private readonly RandomUrlGenerator ShortUrlGenerator;

    public ShortUrlController(ShortUrlRepository shortUrlRepository, RandomUrlGenerator shortUrlGenerator)
    {
        ShortUrlRepository = shortUrlRepository;
        ShortUrlGenerator = shortUrlGenerator;
    }

    /// <summary>
    /// Redirects to long URL
    /// </summary>
    /// <param name="shortUrl"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedirectAsync([FromQuery] string shortUrl)
    {
        if (ShortUrlValidator.ValidatePath(shortUrl, out _))
        {
            return BadRequest();
        }

        var shortUrlDb = await ShortUrlRepository.GetAsync(shortUrl);

        if (shortUrlDb == null || string.IsNullOrWhiteSpace(shortUrlDb.Destination))
        {
            return NotFound();
        }

        return Redirect(shortUrlDb.Destination);
    }

    /// <summary>
    /// Gets a specified URL
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>    
    [HttpPost("/[controller]/get-path")]
    [ActionName(nameof(GetAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<GetViewModelResponse>> GetAsync([FromBody] GetViewModel model)
    {
        var shortUrl = await ShortUrlRepository.GetAsync(model.Path);
        if (shortUrl == null)
        {
            return NotFound();
        }
#pragma warning disable CS8601 // Possible null reference assignment.
        var response = new GetViewModelResponse()
        {
            Path = shortUrl?.Path,
            Destination = shortUrl?.Destination
        };
#pragma warning restore CS8601 // Possible null reference assignment.
        return Ok(response);
    }

    /// <summary>
    /// Creates a short URL for the large specified one
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>    
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /ShortUrl
    ///     {
    ///        "destination": "https://www.google.com",
    ///        "path": "google"    
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created URL</response>
    /// <response code="400">If the URL already exists</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateViewModelResponse>> CreateAsync([FromBody] CreateViewModel model)
    {
        var shortUrl = new ShortUrl
        {
            Destination = model.Destination,
            Path = model.Path ?? ShortUrlGenerator.Generate()
        };

        if (shortUrl.Validate(out var validationResults) == false)
        {
            return BadRequest(Results.ValidationProblem(validationResults));
        }

        await ShortUrlRepository.CreateAsync(shortUrl);

        var response = new CreateViewModelResponse
        {
            Destination = shortUrl.Destination,
            Path = shortUrl.Path
        };

        return CreatedAtAction(nameof(GetAsync), new { path = shortUrl.Path }, response);
    }

    /// <summary>
    /// Deletes a specified URL
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>    
    [HttpDelete("{path}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(string path)
    {
        var shortUrl = await ShortUrlRepository.GetAsync(path);
        if (shortUrl == null)
        {
            return NotFound();
        }
        await ShortUrlRepository.DeleteAsync(path);
        return NoContent();
    }
}
