namespace WebAPI.ViewModels.ShortUrlController.Create.Output;

public class CreateViewModelResponse
{    
    /// <summary>
    /// Destination URL
    /// </summary>
    public string Destination { get; set; } = null!;

    /// <summary>
    /// Short URL
    /// </summary>
    public string Path { get; set; } = null!;
}
