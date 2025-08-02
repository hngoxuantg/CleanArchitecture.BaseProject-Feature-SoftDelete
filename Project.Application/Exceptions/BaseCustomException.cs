using System.Net;

namespace Project.Application.Exceptions
{
    public abstract class BaseCustomException : Exception
    {
        public abstract HttpStatusCode HttpStatusCode { get; }
        public abstract string ErrorCode { get; }
        public virtual string ErrorType => GetType().Name;
        protected BaseCustomException() { }
        protected BaseCustomException(string? message) : base(message) { }
        protected BaseCustomException(string? message, Exception? innerException) : base(message, innerException) { }
    }
}