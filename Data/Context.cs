using Microsoft.EntityFrameworkCore;

namespace Data;

public class Context : DbContext
{
	public Context(DbContextOptions<Context> options) : base(options)
	{

	}

	public DbSet<ShortUrl> ShortUrls { get; set; } = null!;
}
