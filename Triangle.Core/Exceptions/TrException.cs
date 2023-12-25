namespace Triangle.Core.Exceptions;

public class TrException : Exception
{
    public TrException()
    {
    }

    public TrException(string message) : base(message)
    {
    }

    public TrException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
