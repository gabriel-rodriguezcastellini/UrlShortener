using Microsoft.Build.Framework;

namespace WebAPI.ViewModels.ShortUrlController.Get.Input;

public class GetViewModel
{
    /// <summary>
    /// Short URL
    /// </summary>
    [Required]
    public string Path { get; set; } = null!;
}
