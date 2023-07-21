using System.ComponentModel.DataAnnotations;

namespace Data;

public class ShortUrl
{
    public int Id { get; set; }

    [MaxLength(2048)]
    public string Destination { get; set; } = null!;

    [MaxLength(10)]
    public string Path { get; set; } = null!;
}
