namespace Backend.Common
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class BadRequestException : Exception
    {
        public List<string> Errors { get; }

        public BadRequestException(string message) : base(message)
        {
            Errors = new List<string> { message };
        }
        public BadRequestException(List<string> errors) : base("Validation failed.")
        {
            Errors = errors;
        }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message) : base(message) { }
    }
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
}
