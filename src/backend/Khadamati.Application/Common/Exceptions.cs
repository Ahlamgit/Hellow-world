namespace Khadamati.Application.Common;

public class ApplicationException : Exception
{
    public ApplicationException(string message) : base(message) { }
}

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message) : base(message) { }
}

public class UnauthorizedException : ApplicationException
{
    public UnauthorizedException(string message) : base(message) { }
}

public class ForbiddenException : ApplicationException
{
    public ForbiddenException(string message) : base(message) { }
}

public class ValidationException : ApplicationException
{
    public IEnumerable<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors)
        : base("One or more validation failures have occurred.")
    {
        Errors = errors;
    }
}

public class ConflictException : ApplicationException
{
    public ConflictException(string message) : base(message) { }
}
