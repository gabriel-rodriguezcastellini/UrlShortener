namespace WebAPI.ViewModels.ShortUrlController.Get.Output;

public class GetViewModelResponse
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
