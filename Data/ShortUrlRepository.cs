using Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Data;

public sealed class ShortUrlRepository
{       
    private readonly Context _context;

    public ShortUrlRepository(Context context)
    {
        _context = context;
    }    

    public async Task CreateAsync(ShortUrl shortUrl)
    {
        if (await ExistsAsync(shortUrl.Path) || await ExistsDestinationAsync(shortUrl.Destination))
        {
            throw new ShortUrlExistsException($"Shortened URL with path '{shortUrl.Path}' already exists.");
        }
        
        await _context.ShortUrls.AddAsync(shortUrl);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string path)
    {
        if (await ExistsAsync(path) == false)
        {
            throw new UnableToDeleteUrlException($"Shortened URL with path '{path}' does not exist.");            
        }
        
        var url = await _context.ShortUrls.FirstAsync(x => x.Path == path);
        _context.ShortUrls.Remove(url);
        await _context.SaveChangesAsync();

        if (await GetAsync(path) != null)
        {
            throw new UnableToDeleteUrlException("Failed to delete shortened URL.");            
        }
    }

    public async Task<ShortUrl?> GetAsync(string path) => await _context.ShortUrls.AsNoTracking().FirstOrDefaultAsync(x => ShouldSearchByDestination(path) ? x.Destination == path : x.Path == path);

    public async Task<bool> ExistsAsync(string path) => await _context.ShortUrls.AnyAsync(x=> x.Path == path);

    private async Task<bool> ExistsDestinationAsync(string destination) => await _context.ShortUrls.AnyAsync(x => x.Destination == destination);

    private bool ShouldSearchByDestination(string path) => path.Contains('.');    
}
