using System.Text;

namespace Data;

public class RandomUrlGenerator
{    
    private readonly Random _random;

    public RandomUrlGenerator(Random random) => _random = random;

    public string Generate()
    {
        var passwordBuilder = new StringBuilder();

        // 4-Letters lower case   
        passwordBuilder.Append(RandomString(4, true));

        // 4-Digits between 1000 and 9999  
        passwordBuilder.Append(RandomNumber(1000, 9999));

        // 2-Letters upper case  
        passwordBuilder.Append(RandomString(2));
        return passwordBuilder.ToString();
    }

    // Generates a random string with a given size.    
    private string RandomString(int size, bool lowerCase = false)
    {
        var builder = new StringBuilder(size);        
        char offset = lowerCase ? 'a' : 'A';
        const int lettersOffset = 26;

        for (var i = 0; i < size; i++)
        {
            var @char = (char)_random.Next(offset, offset + lettersOffset);
            builder.Append(@char);
        }

        return lowerCase ? builder.ToString().ToLower() : builder.ToString();
    }

    // Generates a random number within a range.      
    private int RandomNumber(int min, int max) => _random.Next(min, max);
}
