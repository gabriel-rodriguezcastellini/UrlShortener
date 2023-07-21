namespace Common.Exceptions;

public class UnableToDeleteUrlException : Exception
{
	public UnableToDeleteUrlException()
	{

	}

	public UnableToDeleteUrlException(string message) : base(message)
	{

	}

	public UnableToDeleteUrlException(string message, Exception exception) : base(message, exception)
	{

	}
}
