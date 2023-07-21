using System.ComponentModel.DataAnnotations;

namespace WebFrontEnd.Models;

public class ShortUrl
{
    [Required]
    public string Destination { get; set; } = null!;
        
    [RegularExpression("^[a-zA-Z0-9_-]*$")]
    public string? Path { get; set; }
}
