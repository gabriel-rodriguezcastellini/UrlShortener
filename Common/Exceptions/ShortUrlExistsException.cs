namespace Common.Exceptions;

public class ShortUrlExistsException : Exception
{
    public ShortUrlExistsException()
    {

    }

    public ShortUrlExistsException(string message) : base(message)
    {

    }

    public ShortUrlExistsException(string message, Exception inner) : base(message, inner)
    {

    }
}
