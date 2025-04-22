namespace ECommerce.Core.Exceptions
{
    public class DataConflictException : Exception
    {
        public DataConflictException() : base("Data conflicts") { }
        public DataConflictException(string message) : base(message) { }
        public DataConflictException(string message, Exception innerException) : base(message, innerException) { }
    }
}
