using System.ComponentModel.DataAnnotations;

namespace WebAPI.ViewModels.ShortUrlController.Create.Input;

public class CreateViewModel
{
    /// <summary>
    /// Destination URL
    /// </summary>
    [Required]
    public string Destination { get; set; } = null!;

    /// <summary>
    /// Optional short URL
    /// </summary>
    [StringLength(10)]
    [RegularExpression("^[a-zA-Z0-9_-]*$")]
    public string? Path { get; set; }
}
